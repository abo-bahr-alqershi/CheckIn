import { apiClient } from './api.service';
import type { PaymentDto, RefundPaymentCommand, VoidPaymentCommand, UpdatePaymentStatusCommand, GetPaymentsByBookingQuery, GetPaymentsByMethodQuery, GetPaymentsByStatusQuery, GetPaymentsByUserQuery, GetAllPaymentsQuery } from '../types/payment.types';
import type { ResultDto, PaginatedResult } from '../types/common.types';

// خدمات المدفوعات (Payments Service) للإدارة
export const AdminPaymentsService = {
  /** استرجاع دفعة */
  refund: (data: RefundPaymentCommand) =>
    apiClient.post<ResultDto<boolean>>('/api/admin/Payments/refund', data).then(res => res.data),

  /** إبطال دفعة */
  void: (data: VoidPaymentCommand) =>
    apiClient.post<ResultDto<boolean>>('/api/admin/Payments/void', data).then(res => res.data),

  /** تحديث حالة الدفعة */
  updateStatus: (paymentId: string, data: UpdatePaymentStatusCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/admin/Payments/${paymentId}/status`, data).then(res => res.data),

  /** جلب جميع المدفوعات مع دعم الفلاتر */
  getAll: (query: GetAllPaymentsQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>('/api/admin/Payments', { params: query }).then(res => res.data),

};
