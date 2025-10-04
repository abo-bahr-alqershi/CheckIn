using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;

namespace YemenBooking.Application.Queries.MobileApp.PropertyTypes;

/// <summary>
/// استعلام الحصول على جميع أنواع الكيانات المتاحة
/// Query to get all available property types
/// </summary>
public class GetPropertyTypesQuery : IRequest<ResultDto<List<PropertyTypeDto>>>
{
    // لا توجد معلمات خاصة بهذا الاستعلام
}