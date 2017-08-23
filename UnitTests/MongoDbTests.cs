using LPCloudCore.DataAccess;
using LPCloudCore.Models;
using LPCloudCore.Models.Core;
using LPCloudCore.Models.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class MongoDbTests
    {

        [TestMethod]
        public void GetByIdTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "GetByIdTest" };

            repo.Add(todo);

            var id = todo.Id;

            var item = repo.GetById(id);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        public async Task GetByIdTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "GetByIdTest" };

            repo.Add(todo);

            var id = todo.Id;

            var item = await repo.GetByIdAsync(id);

            Assert.IsNotNull(item);
        }


        [TestMethod]
        public void AddItemTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "AddItemCheckIdTest" };

            var precount = repo.Count();

            repo.Add(todo);

            var postcount = repo.Count();

            Assert.AreEqual(precount + 1, postcount);
        }

        [TestMethod]
        public void AddItemCheckIdTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "Id check" };

            var pre = todo.Id;

            repo.Add(todo);

            var post = todo.Id;

            Assert.AreNotEqual(pre, post);
        }

        [TestMethod]
        public async Task AddItemTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "AddItemCheckIdTest" };

            var precount = await repo.CountAsync();

            await repo.AddAsync(todo);

            var postcount = await repo.CountAsync();

            Assert.AreEqual(precount + 1, postcount);
        }

        [TestMethod]
        public async Task AddItemCheckIdTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "Id check" };

            var pre = todo.Id;

            await repo.AddAsync(todo);

            var post = todo.Id;

            Assert.AreNotEqual(pre, post);
        }

        [TestMethod]
        public void AddItemsTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo00 = new TodoItem() { Task = "AddItemsCheckIdTest00" };
            var todo01 = new TodoItem() { Task = "AddItemsCheckIdTest01" };
            var todo02 = new TodoItem() { Task = "AddItemsCheckIdTest02" };
            var todo03 = new TodoItem() { Task = "AddItemsCheckIdTest03" };
            var items = new List<TodoItem>() { todo00, todo01, todo02, todo03 };

            var precount = repo.Count();

            repo.Add(items);

            var postcount = repo.Count();

            Assert.AreEqual(precount + 4, postcount);
        }

        [TestMethod]
        public async Task AddItemsTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo00 = new TodoItem() { Task = "AddItemsCheckIdTestAsync00" };
            var todo01 = new TodoItem() { Task = "AddItemsCheckIdTestAsync01" };
            var todo02 = new TodoItem() { Task = "AddItemsCheckIdTestAsync02" };
            var todo03 = new TodoItem() { Task = "AddItemsCheckIdTestAsync03" };
            var items = new List<TodoItem>() { todo00, todo01, todo02, todo03 };

            var precount = await repo.CountAsync();

            await repo.AddAsync(items);

            var postcount = await repo.CountAsync();

            Assert.AreEqual(precount + 4, postcount);
        }

        [TestMethod]
        public void UpdateTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "AddItems..." };
            repo.Add(todo);

            todo.Task = "UpdateTest";
            var preId = todo.Id;

            repo.Update(todo);

            var item = repo.GetById(preId);

            Assert.AreEqual(item.Id, todo.Id);
            Assert.AreEqual(item.Task, "UpdateTest");
        }


        [TestMethod]
        public async Task UpdateTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "AddItems..." };
            repo.Add(todo);

            todo.Task = "UpdateTest";
            var preId = todo.Id;

            await repo.UpdateAsync(todo);

            var item = repo.GetById(preId);

            Assert.AreEqual(item.Id, todo.Id);
            Assert.AreEqual(item.Task, "UpdateTest");
        }


        [TestMethod]
        public void UpdateManyTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo00 = new TodoItem() { Task = "AddItemsCheckIdTestAsync00" };
            var todo01 = new TodoItem() { Task = "AddItemsCheckIdTestAsync01" };
            var todo02 = new TodoItem() { Task = "AddItemsCheckIdTestAsync02" };
            var todo03 = new TodoItem() { Task = "AddItemsCheckIdTestAsync03" };
            var items = new List<TodoItem>() { todo00, todo01, todo02, todo03 };

            repo.Add(items);
            var ids = new List<ObjectId>();
            foreach (var t in items)
            {
                ids.Add(t.Id);
                t.Task = t.Id.ToString();
            }

            repo.Update(items);

            foreach (var i in ids)
            {
                var item = repo.GetById(i);
                Assert.AreEqual(item.Task, i.ToString());
            }

        }


        [TestMethod]
        public async Task UpdateManyTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo00 = new TodoItem() { Task = "AddItemsCheckIdTestAsync00" };
            var todo01 = new TodoItem() { Task = "AddItemsCheckIdTestAsync01" };
            var todo02 = new TodoItem() { Task = "AddItemsCheckIdTestAsync02" };
            var todo03 = new TodoItem() { Task = "AddItemsCheckIdTestAsync03" };
            var items = new List<TodoItem>() { todo00, todo01, todo02, todo03 };

            repo.Add(items);
            var ids = new List<ObjectId>();
            foreach (var t in items)
            {
                ids.Add(t.Id);
                t.Task = t.Id.ToString();
            }

            await repo.UpdateAsync(items);

            foreach (var i in ids)
            {
                var item = repo.GetById(i);
                Assert.AreEqual(item.Task, i.ToString());
            }
        }

        [TestMethod]
        public void DeleteByIdTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "DeleteByIdTest" };

            repo.Add(todo);

            var id = todo.Id;

            repo.Delete(id);

            var find = repo.GetById(id);

            Assert.IsNull(find);

        }

        [TestMethod]
        public void DeleteTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "DeleteByIdTest" };

            repo.Add(todo);

            var id = todo.Id;

            repo.Delete(todo);

            var find = repo.GetById(id);

            Assert.IsNull(find);

        }


        [TestMethod]
        public void DeleteByExpTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var str = "agagag5774";

            var todo = new TodoItem() { Task = str };

            repo.Add(todo);

            var id = todo.Id;

            repo.DeleteMany(x => x.Task == str);

            var find = repo.GetById(id);

            Assert.IsNull(find);

        }

        [TestMethod]
        public async Task DeleteByIdTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "DeleteByIdTest" };

            repo.Add(todo);

            var id = todo.Id;

            await repo.DeleteAsync(id);

            var find = repo.GetById(id);

            Assert.IsNull(find);

        }

        [TestMethod]
        public async Task DeleteTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var todo = new TodoItem() { Task = "DeleteByIdTest" };

            repo.Add(todo);

            var id = todo.Id;

            await repo.DeleteAsync(todo);

            var find = repo.GetById(id);

            Assert.IsNull(find);

        }


        [TestMethod]
        public async Task DeleteByExpTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var str = "agagag5774";

            var todo = new TodoItem() { Task = str };

            repo.Add(todo);

            var id = todo.Id;

            await repo.DeleteManyAsync(x => x.Task == str);

            var find = repo.GetById(id);

            Assert.IsNull(find);

        }

        [TestMethod]
        public void ExistsTest()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var str = new Random(42242).Next().ToString();
            var todo = new TodoItem() { Task = str };

            repo.Add(todo);

            var exists = repo.Exists(x => x.Task == str);
            var existsnot = repo.Exists(x => x.Task == str + "$ß%^##");

            Assert.IsTrue(exists);
            Assert.IsFalse(existsnot);
        }

        [TestMethod]
        public async Task ExistsTestAsync()
        {
            IRepository<TodoItem> repo = new MongoRepository<TodoItem>();

            var str = new Random(42242).Next().ToString();
            var todo = new TodoItem() { Task = str };

            repo.Add(todo);

            var exists = await repo.ExistsAsync(x => x.Task == str);
            var existsnot = await repo.ExistsAsync(x => x.Task == str + "$ß%^##");

            Assert.IsTrue(exists);
            Assert.IsFalse(existsnot);
        }


    }
}
