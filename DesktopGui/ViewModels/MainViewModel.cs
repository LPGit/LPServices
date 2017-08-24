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

        public MainViewModel()
        {
            this.ExecuteSearch = ReactiveCommand.CreateFromTask<string, List<FileResult>>(x => this.getSearchResult(x));

            this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .InvokeCommand(ExecuteSearch);



            this._SpinnerVisibility = ExecuteSearch.IsExecuting
                .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                .ToProperty(this, x => x.SpinnerVisibility, Visibility.Hidden);

            this.ExecuteSearch.ThrownExceptions.Subscribe(ex => { /*Debug.WriteLine(ex);*/ });

            this._SearchResult = this.ExecuteSearch.ToProperty(this, x => x.SearchResult, new List<FileResult>());

            this._ExecuteCommand = ReactiveCommand.Create<FileResult, Unit>(x => OpenFile(x));
        }

        private Unit OpenFile(FileResult x)
        {
            Process.Start(x.Url);
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
                (t, d) => new FileResult { Title = t, Description = d }).ToArray();

            var urls = doc.Root.Descendants("{http://search.yahoo.com/mrss/}thumbnail")
                .Select(x => x.Attributes("url").First().Value);

            var ret = items.Zip(urls, (item, url) => { item.Url = url; return item; }).ToList();
            return ret;
        }



        private async Task<List<FileResult>> getSearchResult(string searchTerm)
        {
          

            var pattern = $"*{searchTerm}*";
            ConcurrentQueue<FileResult> queue = new ConcurrentQueue<FileResult>();

            List<Task> tasks = new List<Task>()
            {
                InternalCollectFiles(dir, pattern, queue),
                InternalCollectFiles(dir2, pattern, queue, false)
            };

            await Task.WhenAll(tasks);

            return queue.AsEnumerable().ToList();
        }

        private async Task InternalCollectFiles(DirectoryInfo directory, string pattern, ConcurrentQueue<FileResult> queue, bool searchSubdirectories = true)
        {
            foreach (var result in directory.GetFiles(pattern))
                queue.Enqueue(new FileResult() { Title = result.Name, Url = result.FullName });

            if (!searchSubdirectories)
                return;

            await Task.WhenAll(directory
                 .GetDirectories()
                 .Select(dir => Task.Run(() => InternalCollectFiles(dir, pattern, queue))).ToArray()).ConfigureAwait(false);
        }
    }
}
