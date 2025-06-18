using EcoBin.API.Common;
using EcoBin.API.Controllers;
using EcoBin.API.Extensions;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.NotificationContainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoNotification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController(
      NotificationService notificationService
        ) : BaseController
    {

        /// <summary>
        /// Get a notification by ID - For All
        /// </summary>
        /// <param name="id">The notification ID</param>
        /// <returns>The notification with the given ID</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotificationResponseDto))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> Getitem([FromRoute] int id)
        {
            return BuildResponse(await notificationService.GetNotificationByIdAsync(id));
        }

        /// <summary>
        /// Get a paginated list of notifications - For All
        /// </summary>
        /// <param name="filter">Pagination and filtering options</param>
        /// <returns>A paginated list of notifications</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<NotificationResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public IActionResult GetItems([FromQuery] PaginationFilter<NotificationResponseDto> filter)
        {
            return BuildResponse(notificationService.GetAllNotifications(filter));
        }
    }
}
