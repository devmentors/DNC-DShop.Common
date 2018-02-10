using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DShop.Messages.Entities;
using MongoDB.Driver;

namespace DShop.Common.Databases.Mongo
{
    public abstract class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
    {
        protected IMongoCollection<TEntity> Collection { get; }

		protected MongoRepository(IMongoDatabase database, string collectionName)
		{
			Collection = database.GetCollection<TEntity>(collectionName);
		}

		public async Task CreateAsync(TEntity entity)
			=> await Collection.InsertOneAsync(entity);

		public async Task UpdateAsync(TEntity entity)
			=> await Collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);

		public async Task DeleteAsync(Guid id)
		=> await Collection.DeleteOneAsync(e => e.Id == id);

		public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
		=> await Collection.Find(predicate).AnyAsync();
    }
}