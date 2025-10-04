using System.Threading.Tasks;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Application.Interfaces.Services;

/// <summary>
/// واجهة خدمة تحليل المشاعر
/// Interface for sentiment analysis service
/// </summary>
public interface ISentimentAnalysisService
{
    /// <summary>
    /// تحليل مشاعر النص المقدم
    /// Analyze sentiment of the provided text
    /// </summary>
    Task<ReviewSentimentResult> AnalyzeSentimentAsync(string text);
} 