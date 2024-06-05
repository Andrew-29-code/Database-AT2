using FlowerSalesAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Globalization;
using System.Xml.Linq;

namespace FlowerSalesAPI.Services
{
    public class FlowerService
    {        
        private readonly IMongoCollection<Product> _products;

        public FlowerService(
            IOptions<FlowerSalesDatabaseSettings> flowerSalesDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                flowerSalesDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                flowerSalesDatabaseSettings.Value.DatabaseName);
            
            _products = mongoDatabase.GetCollection<Product>(
                flowerSalesDatabaseSettings.Value.ProductCollectionName);
        }

        public async Task<List<Product>> GetAsync() =>
            await _products.Find(_ => true).ToListAsync();

        public async Task<List<Product>> GetNamesAsync(string name, string sortBy, string sortOrder)
        {
            var filter = Builders<Product>.Filter.Regex(
                    "Name",
                    new MongoDB.Bson.BsonRegularExpression(
                        name,
                        "i")); // "i" for case-insensitive search

            var findOptions = new FindOptions<Product>();

            if (!string.IsNullOrEmpty(sortBy))
            {
                var sortDefinitionBuilder = Builders<Product>.Sort;

                SortDefinition<Product> sortDefinition;

                if (sortOrder == "desc")
                {
                    sortDefinition = sortDefinitionBuilder.Combine(
                        sortDefinitionBuilder.Descending(sortBy),
                        sortDefinitionBuilder.Ascending(sortBy));
                }
                else
                {
                    sortDefinition = sortDefinitionBuilder.Combine(
                        sortDefinitionBuilder.Ascending(sortBy),
                        sortDefinitionBuilder.Ascending("_id"));
                }

                findOptions.Sort = sortDefinition;
            }

            var list = await _products.Find(filter).ToListAsync();

            if (list.Count == 0) return null;

            return list;
        }

        public async Task<List<Product>> GetStores(string store, string sortBy, string sortOrder)
        {
            var filter = Builders<Product>.Filter.Regex(
                    "StoreLocation",
                    new MongoDB.Bson.BsonRegularExpression(
                        store,
                        "i")); // "i" for case-insensitive search

            var findOptions = new FindOptions<Product>();

            if (!string.IsNullOrEmpty(sortBy))
            {
                var sortDefinitionBuilder = Builders<Product>.Sort;

                SortDefinition<Product> sortDefinition;

                if (sortOrder == "desc")
                {
                    sortDefinition = sortDefinitionBuilder.Combine(
                        sortDefinitionBuilder.Descending(sortBy),
                        sortDefinitionBuilder.Ascending(sortBy));
                }
                else
                {
                    sortDefinition = sortDefinitionBuilder.Combine(
                        sortDefinitionBuilder.Ascending(sortBy),
                        sortDefinitionBuilder.Ascending("_id"));
                }

                findOptions.Sort = sortDefinition;
            }

            var list = await _products.Find(filter).ToListAsync();

            if (list.Count == 0) return null;

            return list;
        } 

        public async Task<List<Product>> GetPrices(double minPrice, double maxPrice) =>
            await _products.Find(shop => minPrice < shop.Price && shop.Price < maxPrice).ToListAsync();

        public async Task<Product?> GetAsync(string id) =>
            await _products.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Product newProduct) =>
            await _products.InsertOneAsync(newProduct);

        public async Task UpdateAsync(string id, Product updatedProduct) =>
            await _products.ReplaceOneAsync(x => x.Id == id, updatedProduct);

        public async Task RemoveAsync(string id) =>
            await _products.DeleteOneAsync(x => x.Id == id);
    }
}
