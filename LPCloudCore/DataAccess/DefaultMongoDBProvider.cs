using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace LPCloudCore.DataAccess
{
    public class DefaultMongoDBProvider : IDatabaseProvider
    {

        /// <summary>
        /// The name used if the config files don't work.
        /// </summary>
        public static string DefaultDatabaseName { get { return defaultDatabaseName; } set { defaultDatabaseName = value; } }
        private static string defaultDatabaseName = "TEST";

        public static string DefaultConnectionString { get { return defaultConnectionString; } set { defaultConnectionString = value; } }
        private static string defaultConnectionString;

        /// <summary>
        /// If not null used as DatabaseProvider.
        /// </summary>
        public static IDatabaseProvider RegisteredDatabaseProvider { get; set; }

        public IMongoDatabase Database => getDatabase();





        private IMongoDatabase dB
        {
            get
            {
                if (_dB == null)
                    _dB = dbClient.GetDatabase(defaultDatabaseName);
                return _dB;
            }
        }
        private IMongoDatabase _dB;


        private static IMongoClient dbClient
        {
            get
            {
                if (_dbClient == null)
                    _dbClient = new MongoClient();

                return _dbClient;
            }
        }
        private static IMongoClient _dbClient;



        private IMongoDatabase getDatabase()
        {
            if (RegisteredDatabaseProvider != null)
                return RegisteredDatabaseProvider.Database;

            return dB;

        }
    }
}
