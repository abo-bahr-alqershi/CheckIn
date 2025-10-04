using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Policies
{
    /// <summary>
    /// أمر لتحديث سياسة الكيان
    /// Command to update a property policy
    /// </summary>
    public class UpdatePropertyPolicyCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف السياسة
        /// </summary>
        public Guid PolicyId { get; set; }

        /// <summary>
        /// نوع السياسة (اختياري)
        /// </summary>
        public string? PolicyType { get; set; }

        /// <summary>
        /// الوصف (اختياري)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// القواعد (اختياري)
        /// </summary>
        public string? Rules { get; set; }
    }
} 