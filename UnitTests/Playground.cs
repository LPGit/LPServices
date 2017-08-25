using LPCloudCore.DataAccess;
using LPCloudCore.Helpers;
using LPCloudCore.Models.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void MiscTest()
        {
            var repo = new MongoRepository<MovieItem>();
            var m = new MovieItem() { Title = "War on Teror" };
            repo.Add(m);

            var repo2 = new MongoRepository<TodoItem>();
            var t = new TodoItem() { Task = "more testing" };
            repo2.Add(t);

        }

        [TestMethod]
        public async Task IOSearchTestAsync()
        {
            double numRuns = 1000;
            var sw = new Stopwatch();
            sw.Start();
            var dir = new DirectoryInfo(@"");
            var dir2 = new DirectoryInfo(@"");

            var pattern = $"*prince*";

            for (int i = 0; i < numRuns; i++)
            {
                var tasks = new List<Task<List<FileInfo>>>()
            {
                IOTools.GetAllFiles(dir, pattern),
                IOTools.GetAllFiles(dir2, pattern,  false)
            };

                await Task.WhenAll(tasks);
            }
            sw.Stop();
            Debug.WriteLine("Scan Time: " + sw.Elapsed / numRuns);
        }

        [TestMethod]
        public async Task IOSearchTestAsync2()
        {
            double numRuns = 1000;
            var sw = new Stopwatch();
            sw.Start();
            var dir = new DirectoryInfo(@"");
            var dir2 = new DirectoryInfo(@"");

            var pattern = $"*prince*";

            for (int i = 0; i < numRuns; i++)
            {
                var tasks = new List<Task<FileInfo[]>>()
                {
                    Task.Run(()=>{return  dir.GetFiles(pattern,SearchOption.AllDirectories); } ),
                    Task.Run(()=>{return  dir2.GetFiles(pattern,SearchOption.TopDirectoryOnly); }),
                };

                await Task.WhenAll(tasks);
            }

            sw.Stop();
            Debug.WriteLine("Scan Time: " + sw.Elapsed / numRuns);
        }

        [TestMethod]
        public void IOSearchTestAsync3()
        {
            double numRuns = 1000;
            var sw = new Stopwatch();
            sw.Start();
            var dir = new DirectoryInfo(@"");
            var dir2 = new DirectoryInfo(@"");

            var pattern = $"*prince*";

            var newTuple1 = (Dire: dir, toponly: false);
            var newTuple2 = (Dire: dir2, toponly: true);

            var searchs = new List<(DirectoryInfo Dire, bool toponly)>() { newTuple1, newTuple2 };

            for (int i = 0; i < numRuns; i++)
            {
                var results = new ConcurrentBag<FileInfo>();


                Parallel.ForEach(searchs, s =>
                {
                    Scan((s.Dire, s.toponly, results));
                });

                //foreach (var n in results)
                //    Debug.WriteLine(n.Name);
            }

            sw.Stop();
            Debug.WriteLine("Scan Time: " + sw.Elapsed / numRuns);



            void Scan((DirectoryInfo Dire, bool toponly, ConcurrentBag<FileInfo> results) e)
            {
                if (!e.toponly)
                    Parallel.ForEach(e.Dire.GetDirectories(), d =>
                    {
                        Scan((d, false, e.results));
                    });


                foreach (var f in e.Dire.GetFiles(pattern))
                    e.results.Add(f);
            }
        }
    }
}
