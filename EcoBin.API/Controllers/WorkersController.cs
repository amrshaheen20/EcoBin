using EcoBin.API.Common;
using EcoBin.API.Extensions;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.WorkerContainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcoBin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController(WorkerService workerService) : BaseController
    {
        /// <summary>
        /// Create a new worker - Manager only
        /// </summary>
        /// <param name="request">The worker data</param>
        /// <returns>The created worker</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WorkerResponseDto))]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> Create([FromBody] WorkerRequestDto request)
        {
            return BuildResponse(await workerService.CreateWorkerAsync(request));
        }

        /// <summary>
        /// Get a worker by ID - For All
        /// </summary>
        /// <param name="id">The ID of the worker</param>
        /// <returns>The worker details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkerResponseDto))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> Getitem([FromRoute] int id)
        {
            return BuildResponse(await workerService.GetWorkerByIdAsync(id));
        }

        /// <summary>
        /// Get a list of workers - For All
        /// </summary>
        /// <param name="filter">Pagination filter</param>
        /// <returns>Paginated list of workers</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<WorkerResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public IActionResult GetItems([FromQuery] PaginationFilter<WorkerResponseDto> filter)
        {
            return BuildResponse(workerService.GetAllWorkers(filter));
        }

        /// <summary>
        /// Update a worker by ID - Manager only
        /// </summary>
        /// <param name="id">The ID of the worker</param>
        /// <param name="item">Updated worker data</param>
        /// <returns>No content</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> UpdateItem(int id, WorkerRequestDto item)
        {
            return BuildResponse(await workerService.UpdateWorkerAsync(id, item));
        }

        /// <summary>
        /// Delete a worker by ID - Manager only
        /// </summary>
        /// <param name="id">The ID of the worker</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            return BuildResponse(await workerService.DeleteWorkerAsync(id));
        }
    }
}
