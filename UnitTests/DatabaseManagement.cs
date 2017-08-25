using LPCloudCore.DataAccess;
using LPCloudCore.Models.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnitTests
{
    [TestClass]

    public class DatabaseManagement
    {
        [TestMethod]
        public void ClearTestDatabase()
        {
            var client = new MongoClient();
            client.DropDatabase("TEST");
        }


        [TestMethod]
        public void AddFileData()
        {
            var repo = new MongoRepository<FileItem>();
            var folder = @"E:\Movies";
            var files = new List<FileInfo>();
            var folders = new Stack<DirectoryInfo>();
            folders.Push(new DirectoryInfo(folder));

            while (folders.Count > 0)
            {
                var ff = folders.Pop();
                files.AddRange(ff.GetFiles());

                foreach (var f in ff.GetDirectories())
                    folders.Push(f);
            }

            var items = files.Select(x => new FileItem() { Fullpath = x.FullName, Name = x.Name });
            repo.Add(items);

        }
    }
}
