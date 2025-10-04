using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.Commands.Images;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.CP.PropertyInSectionImages
{
    public class ReorderPropertyInSectionImagesCommand : IRequest<ResultDto<bool>>
    {
        public List<ImageOrderAssignment> Assignments { get; set; } = new List<ImageOrderAssignment>();
    }
}

