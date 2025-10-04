using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Api.Controllers.Admin;



namespace YemenBooking.Api.Controllers.Admin
{
    public class HomeSectionsController : BaseAdminController
    {
        public HomeSectionsController(IMediator mediator) : base(mediator) { }

        // Dynamic Home Sections






        // Dynamic Home Config




        // City Destinations





    }

    public class CityDestinationStatsDto
    {
        public int PropertyCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}