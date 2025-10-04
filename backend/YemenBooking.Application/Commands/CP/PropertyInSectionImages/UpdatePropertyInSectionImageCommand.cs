using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.CP.PropertyInSectionImages
{
    public class UpdatePropertyInSectionImageCommand : IRequest<ResultDto<ImageDto>>
    {
        public Guid ImageId { get; set; }
        public Guid? PropertyInSectionId { get; set; }
        public string? TempKey { get; set; }
        public string? Alt { get; set; }
        public bool? IsPrimary { get; set; }
        public int? Order { get; set; }
        public List<string>? Tags { get; set; }
        public ImageCategory? Category { get; set; }
    }
}

