using LPCloudCore.Models.Core;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LPCloudCore.DataAccess
{
    public interface IRepository<TEntity, TKey> : IQueryable<TEntity> where TEntity : IEntity<TKey>
    {
        TEntity GetById(TKey id);
        Task<TEntity> GetByIdAsync(TKey id);

        Task<List<T>> GetFilteredAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity);
        void Add(IEnumerable<TEntity> entities);
        Task AddAsync(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        void Update(IEnumerable<TEntity> entities);
        Task UpdateAsync(IEnumerable<TEntity> entities);

        void Delete(TKey id);
        void Delete(TEntity entity);
        void DeleteMany(Expression<Func<TEntity, bool>> predicate);

        Task DeleteAsync(TKey id);
        Task DeleteAsync(TEntity entity);
        Task DeleteManyAsync(Expression<Func<TEntity, bool>> predicate);

        long Count();
        Task<long> CountAsync();

        bool Exists(Expression<Func<TEntity, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    }

    public interface IRepository<TEntity> : IRepository<TEntity, ObjectId> where TEntity : IEntity
    {
    }
}
