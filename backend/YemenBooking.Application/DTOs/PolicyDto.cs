using System;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// DTO لبيانات سياسة الكيان
    /// DTO for property policy data
    /// </summary>
    public class PolicyDto
    {
        /// <summary>
        /// معرف السياسة
        /// Policy identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// معرف الكيان
        /// Property identifier
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// نوع السياسة
        /// Policy type
        /// </summary>
        public PolicyType PolicyType { get; set; }

        /// <summary>
        /// وصف السياسة
        /// Policy description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// قواعد السياسة (JSON)
        /// Policy rules (JSON)
        /// </summary>
        public string Rules { get; set; } = string.Empty;
    }
} 