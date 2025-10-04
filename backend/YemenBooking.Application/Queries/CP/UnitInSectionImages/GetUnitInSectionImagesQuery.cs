using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.CP.UnitInSectionImages
{
    public class GetUnitInSectionImagesQuery : IRequest<ResultDto<PaginatedResultDto<ImageDto>>>
    {
        public Guid? UnitInSectionId { get; set; }
        public string? TempKey { get; set; }
        public string? SortBy { get; set; } = "order";
        public string? SortOrder { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 50;
    }
}