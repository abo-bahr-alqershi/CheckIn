using System;
using MediatR;
using YemenBooking.Application.DTOs.Analytics;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Analytics;

/// <summary>
/// استعلام للحصول على تحليل مشاعر التقييمات لكيان محدد
/// Query to get review sentiment analysis for a specific property
/// </summary>
public class GetReviewSentimentAnalysisQuery : IRequest<ResultDto<ReviewSentimentDto>>
{
    /// <summary>
    /// معرف الكيان
    /// Property identifier
    /// </summary>
    public Guid PropertyId { get; set; }

    public GetReviewSentimentAnalysisQuery(Guid propertyId)
    {
        PropertyId = propertyId;
    }
} 