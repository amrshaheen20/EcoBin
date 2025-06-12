using EcoBin.API.Common;
using EcoBin.API.Extensions;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.BinContainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoBin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BinsController(BinService binService) : BaseController
    {
        /// <summary>
        /// Create a new bin - Manager only
        /// </summary>
        /// <param name="request">The bin data</param>
        /// <returns>The created bin</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BinResponseDto))]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> Create([FromBody] BinRequestDto request)
        {
            return BuildResponse(await binService.CreateBinAsync(request));
        }

        /// <summary>
        /// Get a bin by ID - For All
        /// </summary>
        /// <param name="id">The bin ID</param>
        /// <returns>The bin with the given ID</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BinResponseDto))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> Getitem([FromRoute] int id)
        {
            return BuildResponse(await binService.GetBinByIdAsync(id));
        }

        /// <summary>
        /// Get a paginated list of bins - For All
        /// </summary>
        /// <param name="filter">Pagination and filtering options</param>
        /// <returns>A paginated list of bins</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<BinResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public IActionResult GetItems([FromQuery] PaginationFilter<BinResponseDto> filter)
        {
            return BuildResponse(binService.GetAllBins(filter));
        }

        /// <summary>
        /// Update a bin - Manager only
        /// </summary>
        /// <param name="id">The ID of the bin to update</param>
        /// <param name="item">The updated bin data</param>
        /// <returns>No content</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> UpdateItem(int id, BinRequestDto item)
        {
            return BuildResponse(await binService.UpdateBinAsync(id, item));
        }

        /// <summary>
        /// Delete a bin - Manager only
        /// </summary>
        /// <param name="id">The ID of the bin to delete</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            return BuildResponse(await binService.DeleteBinAsync(id));
        }

        /// <summary>
        /// Empty a bin - For All
        /// </summary>
        /// <param name="id">The ID of the bin to empty</param>
        /// <returns>Status of the operation</returns>
        [HttpPut("{id}/empty")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> EmptyBin(int id)
        {
            return BuildResponse(await binService.EmptyBinAsync(id));
        }

        /// <summary>
        /// Change Lid State - For All
        /// </summary>
        /// <param name="id">The ID of the bin to empty</param>
        /// <param name="request"></param>
        /// 
        [HttpPut("{id}/lid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> ChangeLidState(int id, [FromBody] LidStateRequestDto request)
        {
            return BuildResponse(await binService.ChangeLidStateAsync(id, request));

        }

        /// <summary>
        /// Change Bin Current Capacity - For All
        /// </summary>
        /// 
        [HttpPut("{id}/capacity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> ChangeCurrentCapacity(int id, [FromBody] CurrentCapacityRequestDto request)
        {
            return BuildResponse(await binService.ChangeCurrentCapacityAsync(id, request));
        }


        /// <summary>
        /// Create Token For Bin System - Manager only
        /// </summary>
        /// 
        [HttpPost("{id}/token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BinTokenResponseDto))]
        [Authorize(Policy = Policies.Manger)]
        public async Task<IActionResult> CreateToken(int id)
        {
            return BuildResponse(await binService.CreateBinTokenAsync(id));
        }

        /// <summary>
        /// Set Bin Status - Bin System
        /// </summary>

        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> SetBinStatus(int id, [FromBody] BinStatusRequestDto request)
        {
            return BuildResponse(await binService.SetBinStatusAsync(id, request));
        }
    }
}
