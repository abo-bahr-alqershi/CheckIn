using MediatR;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Commands.MobileApp.Properties;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Properties;

/// <summary>
/// معالج أمر تحديث عدد المشاهدات للعقار للعميل
/// Handler for client update property view count command
/// </summary>
public class ClientUpdatePropertyViewCountCommandHandler : IRequestHandler<ClientUpdatePropertyViewCountCommand, ResultDto<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientUpdatePropertyViewCountCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// معالجة أمر تحديث عدد المشاهدات للعقار
    /// Handle update property view count command
    /// </summary>
    /// <param name="request">الطلب</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<bool>> Handle(ClientUpdatePropertyViewCountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // التحقق من وجود العقار
            // Check if property exists
            var propertyRepo = _unitOfWork.Repository<Core.Entities.Property>();
            var property = await propertyRepo.GetByIdAsync(request.PropertyId);

            if (property == null)
            {
                return ResultDto<bool>.Failed("العقار غير موجود", "PROPERTY_NOT_FOUND");
            }

            // زيادة عدد المشاهدات
            // Increment view count
            property.ViewCount++;
            property.UpdatedAt = DateTime.UtcNow;

            // تحديث العقار
            // Update property
            await propertyRepo.UpdateAsync(property);

            // حفظ التغييرات
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ResultDto<bool>.Ok(true, "تم تحديث عدد المشاهدات بنجاح");
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failed($"حدث خطأ أثناء تحديث عدد المشاهدات: {ex.Message}", "UPDATE_VIEW_COUNT_ERROR");
        }
    }
}