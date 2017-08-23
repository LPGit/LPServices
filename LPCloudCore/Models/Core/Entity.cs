using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace LPCloudCore.Models.Core
{
    public abstract class Entity : IEntity
    {
        public ObjectId Id { get; set; }

    }
}
