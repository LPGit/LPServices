using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using LPCloudCore.Models.Core;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace LPCloudCore.DataAccess
{
    public class MongoRepository<T, TKey> : IRepository<T, TKey> where T : IEntity<TKey>
    {
        public IMongoCollection<T> Collection => this.collection;
        protected internal IMongoCollection<T> collection;

        private IDatabaseProvider dbProvider;
        private string collectionName;

        /// <summary>
        /// Uses the default MongoDB database provider.
        /// </summary>
        public MongoRepository() : this(new DefaultMongoDBProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="dbProvider">Interface which provides a MongoDB database..</param>
        public MongoRepository(IDatabaseProvider dbProvider)
        {
            this.dbProvider = dbProvider;
            this.collectionName = MongoDBHelpers<TKey>.GetCollectionName<T>();
            this.collection = this.dbProvider.Database.GetCollection<T>(collectionName);

            var attribute = typeof(Entity).GetProperty(nameof(Entity.Id)).GetCustomAttributes(typeof(BsonRepresentationAttribute), false).FirstOrDefault() as BsonRepresentationAttribute;

            this.forceObjectIdKey =
                typeof(TKey) == typeof(string)
                && typeof(T).IsSubclassOf(typeof(Entity))
                && attribute != null
                && attribute.Representation == BsonType.ObjectId;
        }

        // helper stuff because internally Entity is using string as key but mongodb is using objecid
        private bool forceObjectIdKey = false;

        private object createBackendId(TKey id)
        {
            if (forceObjectIdKey)
                return ObjectId.Parse(id as string);

            return id;
        }



        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public virtual T GetById(TKey id)
        {
            var filter = Builders<T>.Filter.Eq("_id", createBackendId(id));
            return this.collection.Find(filter).FirstOrDefault();
        }



        public virtual async Task<T> GetByIdAsync(TKey id)
        {
            var filter = Builders<T>.Filter.Eq("_id", createBackendId(id));
            var item = await this.collection.FindAsync(filter).ConfigureAwait(false);

            return item.FirstOrDefault();
        }

        public Task<List<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate)
        {
            return this.collection.AsQueryable().Where(predicate).ToListAsync();
        }

        public virtual T Add(T entity)
        {
            this.collection.InsertOne(entity);

            return entity;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await this.collection.InsertOneAsync(entity).ConfigureAwait(false);
            return entity;
        }

        public virtual void Add(IEnumerable<T> entities)
        {
            this.collection.InsertMany(entities);
        }

        public virtual Task AddAsync(IEnumerable<T> entities)
        {
            return this.collection.InsertManyAsync(entities);
        }


        public virtual T Update(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", createBackendId(entity.Id));
            this.collection.ReplaceOne(filter, entity, new UpdateOptions() { IsUpsert = true });

            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", createBackendId(entity.Id));
            await this.collection.ReplaceOneAsync(filter, entity, new UpdateOptions() { IsUpsert = true }).ConfigureAwait(false);

            return entity;
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                this.Update(entity);
        }

        public virtual Task UpdateAsync(IEnumerable<T> entities)
        {
            return Task.WhenAll(entities.Select(e => this.UpdateAsync(e)));
        }

        public virtual void Delete(TKey id)
        {
            this.collection.DeleteOne(Builders<T>.Filter.Eq("_id", createBackendId(id)));
        }

        public virtual void Delete(T entity)
        {
            this.Delete(entity.Id);
        }

        public virtual void DeleteMany(Expression<Func<T, bool>> predicate)
        {
            this.collection.DeleteMany(predicate);
        }

        public virtual Task DeleteAsync(TKey id)
        {
            return this.collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", createBackendId(id)));
        }

        public virtual Task DeleteAsync(T entity)
        {
            return this.DeleteAsync(entity.Id);
        }

        public virtual Task DeleteManyAsync(Expression<Func<T, bool>> predicate)
        {
            return this.collection.DeleteManyAsync(predicate);
        }

        public virtual long Count()
        {
            return this.collection.Count(new BsonDocument());
        }

        public virtual Task<long> CountAsync()
        {
            return this.collection.CountAsync(new BsonDocument());
        }


        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return this.collection.Find(predicate).Any();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var t = await this.collection.FindAsync(predicate).ConfigureAwait(false);
            return t.Any();
        }

        #region IQueryable<T>
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }


        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get { return this.collection.AsQueryable<T>().ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { return this.collection.AsQueryable<T>().Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get { return this.collection.AsQueryable<T>().Provider; }
        }
        #endregion

    }

    public class MongoRepository<T> : MongoRepository<T, string>, IRepository<T> where T : IEntity
    {
        /// <summary>
        /// Uses the default MongoDB database provider.
        /// </summary>
        public MongoRepository() : this(new DefaultMongoDBProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="dbProvider">Interface which provides a MongoDB database..</param>
        public MongoRepository(IDatabaseProvider dbProvider) : base(dbProvider)
        {
        }
    }

}
