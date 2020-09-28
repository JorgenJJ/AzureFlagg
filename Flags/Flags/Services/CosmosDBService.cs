using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flags.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

namespace Flags.Services
{
    public class CosmosDBService : ICosmosDBService
    {
        private Container _container;

        public CosmosDBService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync(Quiz item)
        {
            await this._container.CreateItemAsync<Quiz>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<Quiz>(id, new PartitionKey(id));
        }

        public async Task<Quiz> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Quiz> response = await this._container.ReadItemAsync<Quiz>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Quiz>> GetItemsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Quiz>(new QueryDefinition(queryString));
            List<Quiz> results = new List<Quiz>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, Quiz item)
        {
            await this._container.UpsertItemAsync<Quiz>(item, new PartitionKey(id));
        }
    }
}
