using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DShop.Messages.Entities;
using MongoDB.Driver;

namespace DShop.Common.Mongo
{
    public abstract class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IIdentifiable
    {
        protected IMongoCollection<TDocument> Collection { get; }

		protected MongoRepository(IMongoDatabase database, string collectionName)
		{
			Collection = database.GetCollection<TDocument>(collectionName);
		}

        public async Task<TDocument> GetByIdAsync(Guid id)
            => await Collection.Find(e => e.Id == id).SingleOrDefaultAsync();

		public async Task CreateAsync(TDocument entity)
			=> await Collection.InsertOneAsync(entity);

		public async Task UpdateAsync(TDocument entity)
			=> await Collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);

		public async Task DeleteAsync(Guid id)
		=> await Collection.DeleteOneAsync(e => e.Id == id);

		public async Task<bool> ExistsAsync(Expression<Func<TDocument, bool>> predicate)
		=> await Collection.Find(predicate).AnyAsync();
    }
}