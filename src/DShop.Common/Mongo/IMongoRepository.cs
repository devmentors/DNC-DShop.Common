using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DShop.Common.Types;
using DShop.Messages.Entities;

namespace DShop.Common.Mongo
{
    public interface IMongoRepository<TEntity> where TEntity : IIdentifiable
    {
         Task<TEntity> GetAsync(Guid id);
         Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
         Task<IEnumerable<TEntity>> FindAsync<TQuery>(Expression<Func<TEntity, bool>> predicate);
         Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate,
				TQuery query) where TQuery : PagedQueryBase;
         Task CreateAsync(TEntity entity);
         Task UpdateAsync(TEntity entity);
         Task DeleteAsync(Guid id); 
         Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    }
}