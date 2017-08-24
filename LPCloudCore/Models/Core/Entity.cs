using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LPCloudCore.Models.Core
{
    [BsonDiscriminator(Required = true)]
    public abstract class Entity : IEntity
    {
        public ObjectId Id { get; set; }

    }
}
