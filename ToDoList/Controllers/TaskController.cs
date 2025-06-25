using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ToDoList.Controllers
{
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ToDoService _toDoService;

        public TaskController(IConfiguration configuration, ToDoService toDoService)
        {
            _configuration = configuration;
            _toDoService = toDoService;
        }

        [HttpPost("task")]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var task = _toDoService.Create(model);

            if (task == null)
            {
                return BadRequest();
            }

            return Ok(task);
        }

        [HttpGet("task/{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var task = await _toDoService.Get(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPut("task/{id}")]
        public async Task<IActionResult> EditTask([FromBody] TaskModel model, Guid id)
        {
            var task = _toDoService.Edit(model, id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpDelete("task/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var success = _toDoService.Delete(id);

            if (success == false)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _toDoService.GetAll();

            return Ok(tasks);
        }

        [HttpPut("task/complete/{id}")]
        public async Task<IActionResult> CompleteTask(Guid id)
        {
            var status = _toDoService.Complete(id);

            if (status == null)
            {
                return Ok();
            }

            return Ok(status);
        }

        


    }
}

