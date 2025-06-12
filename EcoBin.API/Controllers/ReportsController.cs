using EcoBin.API.Common;
using EcoBin.API.Extensions;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.ReportContainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoBin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController(ReportService reportService) : BaseController
    {
        /// <summary>
        /// Create a new report - Worker only
        /// </summary>
        /// <returns>Action result indicating success or failure</returns>
        [HttpPost]
        [Authorize(Policy = Policies.Worker)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReportResponseDto))]
        public async Task<IActionResult> Create(ReportRequestDto request)
        {
            return BuildResponse(await reportService.CreateReportAsync(request));
        }

        /// <summary>
        /// Get a report by ID - For All
        /// </summary>
        /// <param name="id">The report ID</param>
        /// <returns>The report with the specified ID</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = Policies.AllUsers)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReportResponseDto))]
        public async Task<IActionResult> Getitem([FromRoute] int id)
        {
            return BuildResponse(await reportService.GetReportByIdAsync(id));
        }

        /// <summary>
        /// Get all reports - For All
        /// </summary>
        [HttpGet]
        [Authorize(Policy = Policies.AllUsers)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<ReportResponseDto>))]
        public IActionResult GetItems([FromQuery] PaginationFilter<ReportResponseDto> filter)
        {
            return BuildResponse(reportService.GetAllReports(filter));
        }

        /// <summary>
        /// Delete a report by ID - For All
        /// </summary>
        /// <param name="id">The report ID to delete</param>
        /// <returns>Action result indicating success or failure</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.AllUsers)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            return BuildResponse(await reportService.DeleteReportAsync(id));
        }
    }
}
