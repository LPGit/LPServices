using LPCloudCore.DataAccess;
using LPCloudCore.Helpers;
using LPCloudCore.Models.Primitives;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Xml.Linq;

namespace DesktopGui.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public string SearchTerm { get { return this._SearchTerm; } set { this.RaiseAndSetIfChanged(ref _SearchTerm, value); } }
        private string _SearchTerm;

        public ReactiveCommand<string, List<FileResult>> ExecuteSearch { get; protected set; }

        public List<FileResult> SearchResult => _SearchResult.Value;
        private ObservableAsPropertyHelper<List<FileResult>> _SearchResult;

        public Visibility SpinnerVisibility => _SpinnerVisibility.Value;
        private ObservableAsPropertyHelper<Visibility> _SpinnerVisibility;

        public ReactiveCommand ExecuteCommand => this._ExecuteCommand;
        private ReactiveCommand<FileResult, Unit> _ExecuteCommand;

        public ReactiveCommand AddFolderCommand => this.addFolderCommand;
        private ReactiveCommand addFolderCommand;

        public ReactiveList<DirectoryVM> Folders { get; set; }

        public MainViewModel()
        {
            this.ExecuteSearch = ReactiveCommand.CreateFromTask<string, List<FileResult>>(x => this.getSearchResult(x));

            this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .InvokeCommand(ExecuteSearch);

            this.Folders = new ReactiveList<DirectoryVM>() { ChangeTrackingEnabled = true };


            var foldersChanged = Observable.Merge(
                this.Folders.Changed.Select(x => Unit.Default),
                this.Folders.ItemChanged.Select(x => Unit.Default)
                )
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
                .Select(x => this.SearchTerm?.Trim())
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .InvokeCommand(ExecuteSearch);



            this._SpinnerVisibility = ExecuteSearch.IsExecuting
                .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                .ToProperty(this, x => x.SpinnerVisibility, Visibility.Hidden);

            this.ExecuteSearch.ThrownExceptions.Subscribe(ex => { /*Debug.WriteLine(ex);*/ });

            this._SearchResult = this.ExecuteSearch.ToProperty(this, x => x.SearchResult, new List<FileResult>());

            this._ExecuteCommand = ReactiveCommand.Create<FileResult, Unit>(x => OpenFile(x));

            this.addFolderCommand = ReactiveCommand.Create(onAddFolderCommand);
        }

        private void onAddFolderCommand()
        {
            var d = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (d.ShowDialog() == true)
                this.Folders.Add(new DirectoryVM() { Fullpath = d.SelectedPath });
        }

        private Unit OpenFile(FileResult x)
        {
            try
            {
                Process.Start(x.FullName);
            }
            catch { }

            return Unit.Default;
        }

        private async Task<List<FileResult>> getSearchResult2(string searchTerm)
        {
            var doc = await Task.Run(() => XDocument.Load(String.Format(CultureInfo.InvariantCulture,
                   "http://api.flickr.com/services/feeds/photos_public.gne?tags={0}&format=rss_200",
                   HttpUtility.UrlEncode(searchTerm))));

            if (doc.Root == null)
                return null;

            var titles = doc.Root.Descendants("{http://search.yahoo.com/mrss/}title")
                .Select(x => x.Value);

            var tagRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            var descriptions = doc.Root.Descendants("{http://search.yahoo.com/mrss/}description")
                .Select(x => tagRegex.Replace(HttpUtility.HtmlDecode(x.Value), ""));

            var items = titles.Zip(descriptions,
                (t, d) => new FileResult { Name = t, FullName = d }).ToArray();

            var urls = doc.Root.Descendants("{http://search.yahoo.com/mrss/}thumbnail")
                .Select(x => x.Attributes("url").First().Value);

            var ret = items.Zip(urls, (item, url) => { item.FullName = url; return item; }).ToList();
            return ret;
        }



        private async Task<List<FileResult>> getSearchResult(string searchTerm)
        {

            var terms = searchTerm.Split(' ').Select(x => x.Trim().ToLower()).ToList();

            var pattern = $"*{terms.FirstOrDefault()}*";

            var tasks = new List<Task<List<FileInfo>>>();
            foreach (var d in Folders)
            {
                tasks.Add(IOTools.GetAllFiles(new DirectoryInfo(d.Fullpath), pattern, d.IncludeSubfolders));
            };

            await Task.WhenAll(tasks);

            var res = tasks.SelectMany(t => t.Result)
                .Select(x => new FileResult() { FullName = x.FullName, Name = x.Name })
                .ToList();

            if (terms.Count() > 1)
                return res.Where(x => x.Name.ToLower().Contains(terms)).ToList();

            return res;

        }

        private static IRepository<FileItem> repo = new MongoRepository<FileItem>();

        private async Task<List<FileResult>> getSearchResult3(string searchTerm)
        {

            var terms = searchTerm.Split(' ').Select(x => x.Trim().ToLower()).ToList();

            var pattern = $"{terms.FirstOrDefault()}";

            var res = await repo.GetFilteredAsync(x => x.Name.ToLower().Contains(pattern));


            return res
               .Select(x => new FileResult() { FullName = x.Fullpath, Name = x.Name })
               .ToList();

        }

    }
}
