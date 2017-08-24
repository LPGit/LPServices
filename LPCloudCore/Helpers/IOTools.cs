using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPCloudCore.Helpers
{
    public static class IOTools
    {
        public static async Task<List<FileInfo>> GetAllFiles(DirectoryInfo directory, string pattern, bool searchSubdirectories = true)
        {
            ConcurrentBag<FileInfo> results = new ConcurrentBag<FileInfo>();
            await internalGetAllFiles(new DirectoryInfo[] { directory }, pattern, results, searchSubdirectories);

            return results.ToList();
        }

        public static async Task<List<FileInfo>> GetAllFiles(IEnumerable<DirectoryInfo> directories, string pattern, bool searchSubdirectories = true)
        {
            ConcurrentBag<FileInfo> results = new ConcurrentBag<FileInfo>();
            await internalGetAllFiles(directories, pattern, results, searchSubdirectories);

            return results.ToList();
        }

        private static async Task internalGetAllFiles(IEnumerable<DirectoryInfo> directories, string pattern, ConcurrentBag<FileInfo> results, bool searchSubdirectories = true)
        {
            var tasks = new List<Task>();
            foreach (var d in directories)
            {
                // scan folders
                tasks.Add(Task.Run(() =>
                {
                    foreach (var f in d.GetFiles(pattern))
                        results.Add(f);
                }));

                // scan subfolder
                if (searchSubdirectories)
                {
                    foreach (var subdir in d.GetDirectories())
                    {
                        tasks.Add(Task.Run(() => internalGetAllFiles(new DirectoryInfo[] { subdir }, pattern, results)));
                    }
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
