using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DShop.Messages.Entities;

namespace DShop.Common.Mongo
{
    public interface IMongoRepository<TEntity> where TEntity : IIdentifiable
    {
         Task<TEntity> GetByIdAsync(Guid id);
         Task CreateAsync(TEntity entity);
         Task UpdateAsync(TEntity entity);
         Task DeleteAsync(Guid id); 
         Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    }
}