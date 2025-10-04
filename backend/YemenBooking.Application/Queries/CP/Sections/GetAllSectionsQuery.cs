using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Sections;

namespace YemenBooking.Application.Queries.CP.Sections
{
    public class GetAllSectionsQuery : IRequest<IEnumerable<SectionDto>>
    {
    }
}

