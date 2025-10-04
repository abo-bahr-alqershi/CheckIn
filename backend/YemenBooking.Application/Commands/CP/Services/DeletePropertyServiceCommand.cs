using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Services
{
    /// <summary>
    /// أمر لحذف خدمة كيان
    /// Command to delete a property service
    /// </summary>
    public class DeletePropertyServiceCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف الخدمة
        /// Service identifier
        /// </summary>
        public Guid ServiceId { get; set; }
    }
} 