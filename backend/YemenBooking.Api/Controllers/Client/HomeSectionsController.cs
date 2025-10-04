using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Api.Controllers.Client;



namespace YemenBooking.Api.Controllers.Client
{
    public class HomeSectionsController : BaseClientController
    {
        public HomeSectionsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Get dynamic home sections for mobile app
        /// </summary>

        /// <summary>
        /// Get home configuration
        /// </summary>



        /// <summary>
        /// Get city destinations
        /// </summary>



        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User?.FindFirst("sub")?.Value ?? User?.FindFirst("id")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string GetClientIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }


}