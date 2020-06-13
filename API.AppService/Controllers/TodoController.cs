using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TomHeijmans.BCWebinar.API.AppService.Services;
using TomHeijmans.BCWebinar.Models;

namespace TomHeijmans.BCWebinar.API.AppService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> logger;
        private readonly ITodoService todoService;

        public TodoController(ILogger<TodoController> logger
            , ITodoService todoService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
        }

        [HttpGet]
        public Task<IEnumerable<TodoItem>> Get()
        {
            return todoService.GetAll();
        }

        [HttpPost]
        public async Task<TodoItem> Add([FromBody]TodoItem item)
        {
            item.Id = Guid.NewGuid().ToString(); // Ensure id
            await todoService.AddItem(item);
            return item;
        }

        [HttpDelete("{id}")]
        public Task Add(string id)
        {
            return todoService.DeleteItem(id);
        }
    }
}
