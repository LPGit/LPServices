﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LPCloudCore.Models.Core
{
    [BsonDiscriminator(Required = true)]
    public class ItemBase : Entity
    {
        public virtual DateTime CreatedDate { get => createdDateUtc.ToLocalTime(); set => this.createdDateUtc = value.ToUniversalTime(); }
        private DateTime createdDateUtc;

        public bool IsActive { get; set; } = true;

        public ICollection<string> Tags { get; set; }

        public ItemBase()
        {
            this.createdDateUtc = DateTime.UtcNow;
        }
    }
}
