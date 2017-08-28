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

        [TestMethod]
        public void AddTodoItems()
        {
            var repo = new MongoRepository<TodoItem>();

            var t = new TodoItem()
            {
                Task = "Transfer Moneys",
                Description = "Barclay needs my money..."
            };
            repo.Add(t);

            t = new TodoItem()
            {
                Task = "Transfer Moneys",
                Description = "Telekom needs my money..."
            };
            repo.Add(t);


            t = new TodoItem()
            {
                Task = "Transfer Moneys",
                Description = "Commdirect needs my money..."
            };
            repo.Add(t);


            t = new TodoItem()
            {
                Task = "Sky",
                Description = "Check the situation..."
            };
            repo.Add(t);


            t = new TodoItem()
            {
                Task = "Initialize Game Development",
                Description = "Talk with jochen..."
            };
            repo.Add(t);


            t = new TodoItem()
            {
                Task = "Develop a roadmap",
                Description = "Just do it!"
            };
            repo.Add(t);

        }
    }
}
