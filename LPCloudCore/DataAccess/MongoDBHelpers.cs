using System;
using System.Collections.Generic;
using System.Text;
using LPCloudCore.Models.Core;
using MongoDB.Driver;
using System.Reflection;

namespace LPCloudCore.DataAccess
{
    public static class MongoDBHelpers<TKey>
    {
        /// <summary>
        /// Determines the collectionname for T and assures it is not empty.
        /// </summary>
        /// <typeparam name="T">The type to determine the collectionname for.</typeparam>
        /// <returns>Returns the collectionname for T.</returns>
        public static string GetCollectionName<T>() where T : IEntity<TKey>
        {
            string collectionName;
            var type = typeof(T);

            var att = Attribute.GetCustomAttribute(typeof(T), typeof(MongoDBCollectionName));

            if (att != null)
            {
                collectionName = (att as MongoDBCollectionName).Name;
            }
            else
            {
                collectionName = typeof(T).Name;
            }

            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentException("Collection name cannot be empty for this entity.");
            }
            //return collectionName.ToLower() + "s";
            return collectionName + "s";
        }

    }
}
