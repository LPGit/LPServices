using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    public class DatabaseManagement
    {
        [TestMethod]
        public void ClearTestDatabase()
        {
            var client = new MongoClient();
            client.DropDatabase("TEST");
        }
    }
}
