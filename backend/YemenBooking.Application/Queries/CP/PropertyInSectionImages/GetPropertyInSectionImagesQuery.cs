using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.CP.PropertyInSectionImages
{
    public class GetPropertyInSectionImagesQuery : IRequest<ResultDto<PaginatedResultDto<ImageDto>>>
    {
        public Guid? PropertyInSectionId { get; set; }
        public string? TempKey { get; set; }
        public string? SortBy { get; set; } = "order";
        public string? SortOrder { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 50;
    }
}