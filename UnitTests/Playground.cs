using LPCloudCore.DataAccess;
using LPCloudCore.Models.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
