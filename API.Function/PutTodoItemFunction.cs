using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using TomHeijmans.BCWebinar.Models;

namespace TomHeijmans.BCWebinar.API.Function
{
    public static class PutTodoItemFunction
    {
        private const string DatabaseName = "TodoApp";
        private const string CollectionName = "Item";

        [FunctionName("PutTodoItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{id}")] HttpRequest req,

            [CosmosDB(databaseName: DatabaseName, collectionName: CollectionName,
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{id}",
                PartitionKey = "{id}")] TodoItem todoItemInput,

            [CosmosDB(databaseName: DatabaseName, collectionName: CollectionName,
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<TodoItem> todoItemsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (todoItemInput == null)
            {
                // Existing item not found
                return new BadRequestResult();
            }

            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                string requestBody = await streamReader.ReadToEndAsync();
                TodoItem updatedTodoItem = JsonConvert.DeserializeObject<TodoItem>(requestBody);
                updatedTodoItem.Id = todoItemInput.Id; // Make sure we don't save on a different id
                await todoItemsOut.AddAsync(updatedTodoItem);
            }
            return new OkResult();
        }
    }
}
