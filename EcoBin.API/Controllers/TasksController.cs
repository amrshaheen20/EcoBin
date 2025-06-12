using EcoBin.API.Common;
using EcoBin.API.Controllers;
using EcoBin.API.Extensions;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.TaskContainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoWorkerTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(TaskService taskService) : BaseController
    {
        /// <summary>
        /// Create a new worker task - Manager only
        /// </summary>
        /// <param name="request">The task details</param>
        /// <returns>The created task</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkerTaskResponseDto))]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> Create([FromBody] WorkerTaskRequestDto request)
        {
            return BuildResponse(await taskService.CreateTaskAsync(request));
        }

        /// <summary>
        /// Get a task by ID - For All
        /// </summary>
        /// <param name="id">The task ID</param>
        /// <returns>The task with the specified ID</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkerTaskResponseDto))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> Getitem([FromRoute] int id)
        {
            return BuildResponse(await taskService.GetTaskByIdAsync(id));
        }

        /// <summary>
        /// Get All tasks - For All
        /// </summary>
        /// <param name="filter">Pagination and filtering options</param>
        /// <returns>A paginated list of tasks</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<WorkerTaskResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public IActionResult GetItems([FromQuery] PaginationFilter<WorkerTaskResponseDto> filter)
        {
            return BuildResponse(taskService.GetAllTasks(filter));
        }

        /// <summary>
        /// Update an existing task - Manager only
        /// </summary>
        /// <param name="id">The ID of the task to update</param>
        /// <param name="item">The updated task data</param>
        /// <returns>No content</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> UpdateItem(int id, WorkerTaskRequestDto item)
        {
            return BuildResponse(await taskService.UpdateTaskAsync(id, item));
        }

        /// <summary>
        /// Delete a task by ID - Manager only
        /// </summary>
        /// <param name="id">The ID of the task to delete</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            return BuildResponse(await taskService.DeleteTaskAsync(id));
        }
    }
}
