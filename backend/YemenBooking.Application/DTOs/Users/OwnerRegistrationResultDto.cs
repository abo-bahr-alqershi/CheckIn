using System;

namespace YemenBooking.Application.DTOs.Users
{
    /// <summary>
    /// نتيجة تسجيل مالك الكيان
    /// Property owner registration result
    /// </summary>
    public class OwnerRegistrationResultDto
    {
        /// <summary>
        /// معرف المستخدم
        /// User identifier
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// معرف الكيان
        /// Property identifier
        /// </summary>
        public Guid PropertyId { get; set; }
    }
} 