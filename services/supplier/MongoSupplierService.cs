using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PeyulErp.Models;
using PeyulErp.Settings;

namespace PeyulErp.Services
{
    public class MongoSupplierService : ISupplierService
    {
        private readonly MongoDbSettings _mongoDbSettings;
        private readonly IMongoCollection<Supplier> _supplierCollection;
        private readonly FilterDefinitionBuilder<Supplier> _filterBuilder = Builders<Supplier>.Filter;

        public MongoSupplierService(IOptions<MongoDbSettings> mongoDbSettings, IMongoClient mongoClient)
        {
            _mongoDbSettings = mongoDbSettings.Value;
            var database = mongoClient.GetDatabase(_mongoDbSettings.DatabaseName);
            _supplierCollection = database.GetCollection<Supplier>(_mongoDbSettings.SupplierCollectionName);
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            var newSupplier = supplier with { Id = Guid.NewGuid(), CreateDate = DateTime.UtcNow };
            await _supplierCollection.InsertOneAsync(newSupplier);

            return newSupplier;
        }

        public async Task<bool> DeleteSupplierAsync(Guid id)
        {
            var existingSupplier = await _supplierCollection.Find(_filterBuilder.Eq(supplier => supplier.Id, id)).FirstOrDefaultAsync();

            if (existingSupplier is null)
            {
                  return false;
            }
            
            await _supplierCollection.DeleteOneAsync(_filterBuilder.Eq(supplier => supplier.Id, id));
            return true;
        }

        public async Task<Supplier> GetSupplierAsync(Guid id)
        {
            return await _supplierCollection.Find(_filterBuilder.Eq(supplier => supplier.Id, id)).FirstOrDefaultAsync();
        }

        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            return await _supplierCollection.Find(new BsonDocument()).ToListAsync();            
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _supplierCollection.Find(_filterBuilder.Eq(existingSupplier => existingSupplier.Id, supplier.Id)).FirstOrDefaultAsync();

            if (existingSupplier is null)
            {
                return null;
            }

            var newSupplier = supplier with { UpdateDate = DateTime.UtcNow, Id = existingSupplier.Id };
            await _supplierCollection.ReplaceOneAsync(_filterBuilder.Eq(existingSupplier => existingSupplier.Id, supplier.Id), newSupplier);

            return newSupplier;            
        }
    }
}