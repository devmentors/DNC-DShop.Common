using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DShop.Common.Types;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DShop.Common.Mongo
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : IIdentifiable
    {
        protected IMongoCollection<TEntity> Collection { get; }

		public MongoRepository(IMongoDatabase database, string collectionName)
		{
			Collection = database.GetCollection<TEntity>(collectionName);
		}

        public async Task<TEntity> GetAsync(Guid id)
            => await GetAsync(e => e.Id == id);

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
            => await Collection.Find(predicate).SingleOrDefaultAsync();

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
            => await Collection.Find(predicate).ToListAsync();

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate,
				TQuery query) where TQuery : PagedQueryBase
			=> await Collection.AsQueryable().Where(predicate).PaginateAsync(query);

		public async Task AddAsync(TEntity entity)
			=> await Collection.InsertOneAsync(entity);

		public async Task UpdateAsync(TEntity entity)
			=> await Collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);

		public async Task DeleteAsync(Guid id)
			=> await Collection.DeleteOneAsync(e => e.Id == id);

		public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
			=> await Collection.Find(predicate).AnyAsync();
    }
}