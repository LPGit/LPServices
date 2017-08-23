using LPCloudCore.Models;
using LPCloudCore.Models.Core;
using LPCloudCore.Models.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class MongoDbTests
    {
        private IMongoDatabase DB
        {
            get
            {
                if (_dB == null)
                    _dB = DbClient.GetDatabase(testDBName);
                return _dB;
            }
        }
        private IMongoDatabase _dB;
        private static IMongoClient DbClient
        {
            get
            {
                if (_dbClient == null)
                {
                    //BsonClassMap.RegisterClassMap<DBEntityBase>();
                    //BsonClassMap.RegisterClassMap<MovieItem>();
                    //BsonClassMap.RegisterClassMap<ItemBase>();
                    _dbClient = new MongoClient();
                }
                return _dbClient;
            }
        }
        private static IMongoClient _dbClient;

        private static string testDBName = "TEST";

        [TestMethod]
        public void RecreateTestDB()
        {
            DbClient.DropDatabase(testDBName);
        }

        //[TestMethod]
        //public void AddMovie()
        //{
        //    var genres = DB.GetCollection<ItemBase>("genres").AsQueryable<ItemBase>().ToList();

        //    var movie = new MovieItem() { Genres = new System.Collections.Generic.List<ItemBase>() { genres[0], genres[1] } };

        //    var collection = DB.GetCollection<MovieItem>("movies");
        //    collection.InsertOne(movie);
        //}

        //[TestMethod]
        //public void AddGenre()
        //{
        //    var item = new ItemBase("action");
        //    var item2 = new ItemBase("horror");


        //    var collection = DB.GetCollection<ItemBase>("genres");
        //    collection.InsertOne(item);
        //    collection.InsertOne(item2);
        //}

        [TestMethod]
        public void DumpItems()
        {
            var tt = new RawTextItem();
            
            //Debug.WriteLine("Dumping");

            //var col = DB.GetCollection<ItemBase>("genres").AsQueryable<ItemBase>().ToList();
            //var d = col.FirstOrDefault()?.CreatedDate;
            //foreach (var i in col)
            //    Debug.WriteLine(i?.NewField);


            //bool stop = true;
        }
    }
}
