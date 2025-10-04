import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';

import type {
  BookingDto,
  // Commands
  CancelBookingCommand,
  UpdateBookingCommand,
  ConfirmBookingCommand,
  // Queries
  GetBookingByIdQuery,
  GetBookingsByDateRangeQuery
} from '../types/booking.types';

// المسار الأساسي لخدمة حجوزات الإدارة
const API_BASE = '/api/admin/bookings';

/**
 * خدمات إدارة الحجوزات للإدمن
 */
export const AdminBookingsService = {
  /** إلغاء حجز */
  cancel: (command: CancelBookingCommand) =>
    apiClient.post<ResultDto<boolean>>(
      `${API_BASE}/${command.bookingId}/cancel`,
      command
    ).then(res => res.data),

  /** تحديث بيانات الحجز */
  update: (command: UpdateBookingCommand) =>
    apiClient.put<ResultDto<boolean>>(
      `${API_BASE}/${command.bookingId}/update`,
      command
    ).then(res => res.data),

  /** جلب بيانات حجز بواسطة المعرف */
  getById: (query: GetBookingByIdQuery) =>
    apiClient.get<ResultDto<BookingDto>>(
      `${API_BASE}/${query.bookingId}`
    ).then(res => res.data),



  /** استعلام الحجوزات في نطاق زمني */
  getByDateRange: (query: GetBookingsByDateRangeQuery) =>
    apiClient.get<PaginatedResult<BookingDto>>(
      `${API_BASE}/by-date-range`,
      { params: query }
    ).then(res => res.data),

      /** تأكيد الحجز */
  confirm: (data: ConfirmBookingCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/confirm`, data).then(res => res.data),

}; 