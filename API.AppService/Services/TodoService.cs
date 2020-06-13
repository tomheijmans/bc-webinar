using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomHeijmans.BCWebinar.Models;

namespace TomHeijmans.BCWebinar.API.AppService.Services
{
    public interface ITodoService
    {
        Task AddItem(TodoItem item);
        Task DeleteItem(string id);
        Task<TodoItem> GetItem(string id);
        Task<IEnumerable<TodoItem>> GetAll();
    }

    public class TodoService : ITodoService
    {
        private readonly Container container;

        public TodoService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            container = dbClient.GetContainer(databaseName, containerName);
        }

        public Task AddItem(TodoItem item)
        {
            return container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public Task DeleteItem(string id)
        {
            return container.DeleteItemAsync<TodoItem>(id, new PartitionKey(id));
        }

        public async Task<TodoItem> GetItem(string id)
        {
            try
            {
                ItemResponse<TodoItem> response = await container.ReadItemAsync<TodoItem>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            FeedIterator<TodoItem> query = container.GetItemQueryIterator<TodoItem>(new QueryDefinition("SELECT * FROM c"));
            List<TodoItem> results = new List<TodoItem>();
            while (query.HasMoreResults)
            {
                FeedResponse<TodoItem> response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}
