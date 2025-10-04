using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Api.Controllers.Admin
{
    [Route("api/admin/system-settings")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingsService _settingsService;
        private readonly ICurrencySettingsService _currencySettingsService;
        private readonly ICitySettingsService _citySettingsService;

        public SystemSettingsController(ISystemSettingsService settingsService, ICurrencySettingsService currencySettingsService, ICitySettingsService citySettingsService)
        {
            _settingsService = settingsService;
            _currencySettingsService = currencySettingsService;
            _citySettingsService = citySettingsService;
        }

        /// <summary>
        /// جلب إعدادات النظام
        /// Get system settings
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResultDto<Dictionary<string, string>>>> GetSettingsAsync(CancellationToken cancellationToken)
        {
            var settings = await _settingsService.GetSettingsAsync(cancellationToken);
            return Ok(ResultDto<Dictionary<string, string>>.Succeeded(settings));
        }

        /// <summary>
        /// حفظ أو تحديث إعدادات النظام
        /// Save or update system settings
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<ResultDto<bool>>> SaveSettingsAsync([FromBody] Dictionary<string, string> settings, CancellationToken cancellationToken)
        {
            await _settingsService.SaveSettingsAsync(settings, cancellationToken);
            return Ok(ResultDto<bool>.Succeeded(true));
        }

        /// <summary>
        /// جلب قائمة العملات
        /// Get currency settings
        /// </summary>
        [AllowAnonymous]
        [HttpGet("currencies")]
        public async Task<ActionResult<ResultDto<List<CurrencyDto>>>> GetCurrenciesAsync(CancellationToken cancellationToken)
        {
            var currencies = await _currencySettingsService.GetCurrenciesAsync(cancellationToken);
            return Ok(ResultDto<List<CurrencyDto>>.Succeeded(currencies));
        }

        /// <summary>
        /// جلب قائمة المدن
        /// Get city settings
        /// </summary>
        [AllowAnonymous]
        [HttpGet("cities")]
        public async Task<ActionResult<ResultDto<List<CityDto>>>> GetCitiesAsync(CancellationToken cancellationToken)
        {
            var cities = await _citySettingsService.GetCitiesAsync(cancellationToken);
            // Ensure image URLs are absolute for frontend cards, leverage same behavior as ImagesController
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            foreach (var c in cities)
            {
                for (int i = 0; i < c.Images.Count; i++)
                {
                    var url = c.Images[i] ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(url) && !url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        c.Images[i] = baseUrl + (url.StartsWith("/") ? url : "/" + url);
                    }
                }
            }
            return Ok(ResultDto<List<CityDto>>.Succeeded(cities));
        }

        /// <summary>
        /// حفظ أو تحديث قائمة العملات
        /// Save or update currency settings
        /// </summary>
        [HttpPut("currencies")]
        public async Task<ActionResult<ResultDto<bool>>> SaveCurrenciesAsync([FromBody] List<CurrencyDto> currencies, CancellationToken cancellationToken)
        {
            await _currencySettingsService.SaveCurrenciesAsync(currencies, cancellationToken);
            return Ok(ResultDto<bool>.Succeeded(true));
        }

        /// <summary>
        /// حذف عملة نهائياً بعد التحقق من الارتباطات
        /// Permanently delete a currency after verifying references
        /// </summary>
        [HttpDelete("currencies/{code}")]
        public async Task<ActionResult<ResultDto>> DeleteCurrencyAsync([FromRoute] string code, CancellationToken cancellationToken)
        {
            try
            {
                await _currencySettingsService.DeleteCurrencyAsync(code, cancellationToken);
                return Ok(ResultDto.Ok("تم حذف العملة بنجاح"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResultDto.Failure(ex.Message, errorCode: "CURRENCY_DELETE_CONFLICT"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResultDto.Failure(ex.Message, errorCode: "INVALID_CURRENCY_CODE"));
            }
        }

        /// <summary>
        /// حفظ أو تحديث قائمة المدن
        /// Save or update city settings
        /// </summary>
        [HttpPut("cities")]
        public async Task<ActionResult<ResultDto<bool>>> SaveCitiesAsync([FromBody] List<CityDto> cities, CancellationToken cancellationToken)
        {
            // Normalize URLs to server-relative paths to keep DB clean
            var origin = $"{Request.Scheme}://{Request.Host}";
            foreach (var c in cities)
            {
                for (int i = 0; i < c.Images.Count; i++)
                {
                    var url = c.Images[i] ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(url) && url.StartsWith(origin, StringComparison.OrdinalIgnoreCase))
                    {
                        c.Images[i] = url.Substring(origin.Length);
                    }
                }
            }
            await _citySettingsService.SaveCitiesAsync(cities, cancellationToken);
            return Ok(ResultDto<bool>.Succeeded(true));
        }

        /// <summary>
        /// حذف مدينة مع التحقق من الارتباطات
        /// Delete city after validating dependent references
        /// </summary>
        [HttpDelete("cities/{name}")]
        public async Task<ActionResult<ResultDto>> DeleteCityAsync([FromRoute] string name, CancellationToken cancellationToken)
        {
            try
            {
                await _citySettingsService.DeleteCityAsync(name, cancellationToken);
                return Ok(ResultDto.Ok("تم حذف المدينة بنجاح"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResultDto.Failure(ex.Message, errorCode: "CITY_DELETE_CONFLICT"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResultDto.Failure(ex.Message, errorCode: "INVALID_CITY_NAME"));
            }
        }
    }
} 