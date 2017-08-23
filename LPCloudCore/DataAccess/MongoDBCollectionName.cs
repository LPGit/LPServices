using System;
using System.Collections.Generic;
using System.Text;

namespace LPCloudCore.DataAccess
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MongoDBCollectionName : Attribute
    {
        public virtual string Name { get; private set; }

        public MongoDBCollectionName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Empty collectionname not allowed", "name");

            this.Name = name;
        }
    }
}
