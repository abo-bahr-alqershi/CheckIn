import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminBookingsService } from '../services/admin-bookings.service';
import type {
  BookingDto,
  GetBookingsByDateRangeQuery,
  UpdateBookingCommand,
  CancelBookingCommand,
  ConfirmBookingCommand
} from '../types/booking.types';
import type { PaginatedResult, ResultDto } from '../types/common.types';

/**
 * هوك لإدارة استعلامات وحالات حجوزات الإدارة
 * يعزل التعامل مع react-query وخدمات الحجوزات في مكان واحد
 * @param params معايير استعلام الحجوزات (نطاق التاريخ، ترقيم الصفحات، الفلاتر)
 * @returns بيانات الحجوزات وحالات التحميل والأخطاء ودوال التحديث والإلغاء والتأكيد
 */
export const useAdminBookings = (params: GetBookingsByDateRangeQuery) => {
  const queryClient = useQueryClient();
  const queryKey = ['admin-bookings', params] as const;

  // جلب الحجوزات في نطاق زمني مع الفلاتر والصفحات
  const { data: bookingsData, isLoading, error } = useQuery<PaginatedResult<BookingDto>, Error>({
    queryKey,
    queryFn: () => AdminBookingsService.getByDateRange(params)
  });

  // تحديث بيانات الحجز
  const updateBooking = useMutation<ResultDto<boolean>, Error, UpdateBookingCommand>({
    mutationFn: (data) => AdminBookingsService.update(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-bookings'] });
    },
  });

  // إلغاء الحجز
  const cancelBooking = useMutation<ResultDto<boolean>, Error, CancelBookingCommand>({
    mutationFn: (data) => AdminBookingsService.cancel(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-bookings'] });
    },
  });

  // تأكيد الحجز
  const confirmBooking = useMutation<ResultDto<boolean>, Error, ConfirmBookingCommand>({
    mutationFn: (data) => AdminBookingsService.confirm(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-bookings'] });
    },
  });

  return {
    bookingsData,
    isLoading,
    error,
    updateBooking,
    cancelBooking,
    confirmBooking
  };
}; 