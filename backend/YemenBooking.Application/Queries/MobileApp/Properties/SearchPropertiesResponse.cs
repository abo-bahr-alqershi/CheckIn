using System.Collections.Generic;
using YemenBooking.Application.DTOs.PropertySearch;

namespace YemenBooking.Application.Queries.MobileApp.Properties
{
    public class SearchPropertiesResponse
    {
        public List<PropertySearchResultDto> Properties { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}