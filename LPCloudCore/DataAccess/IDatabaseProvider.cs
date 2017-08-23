using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace LPCloudCore.DataAccess
{
    public interface IDatabaseProvider
    {
        IMongoDatabase Database { get; }
    }
}
