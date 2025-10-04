using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Application.Interfaces.Services
{
    public static class ValidationServiceExtensions
    {
        /// <summary>
        /// Alias for booking/user/room/hotel/payment validation.
        /// </summary>
        public static Task<ValidationResult> ValidateAsync(this IValidationService service, object data, CancellationToken cancellationToken = default)
            => service.ValidateBookingAsync(data, cancellationToken);

    }
} 