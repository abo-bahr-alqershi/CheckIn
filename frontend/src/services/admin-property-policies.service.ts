import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  PolicyDto,
  PolicyDetailsDto,
  CreatePropertyPolicyCommand,
  UpdatePropertyPolicyCommand,
  DeletePropertyPolicyCommand,
  GetPropertyPoliciesQuery,
  GetPolicyByIdQuery,
  GetPoliciesByTypeQuery,
} from '../types/policy.types';

// خدمات إدارة سياسات الكيانات للمدراء (Policies Service)
export const AdminPropertyPoliciesService = {
  /** جلب جميع سياسات كيان معين */
  getByProperty: (query: GetPropertyPoliciesQuery) =>
    apiClient.get<ResultDto<PolicyDto[]>>('/api/admin/PropertyPolicies', { params: query }).then(res => res.data),

  /** جلب تفاصيل سياسة بواسطة المعرف */
  getById: (query: GetPolicyByIdQuery) =>
    apiClient.get<ResultDto<PolicyDetailsDto>>(`/api/admin/PropertyPolicies/${query.policyId}`).then(res => res.data),

  /** إنشاء سياسة جديدة للكيان */
  create: (data: CreatePropertyPolicyCommand) =>
    apiClient.post<ResultDto<string>>('/api/admin/PropertyPolicies', data).then(res => res.data),

  /** تحديث سياسة */
  update: (policyId: string, data: UpdatePropertyPolicyCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/admin/PropertyPolicies/${policyId}`, data).then(res => res.data),

  /** حذف سياسة */
  delete: (policyId: string) =>
    apiClient.delete<ResultDto<boolean>>(`/api/admin/PropertyPolicies/${policyId}`).then(res => res.data),

  /** جلب السياسات حسب النوع مع صفحات */
  getByType: (query: GetPoliciesByTypeQuery) =>
    apiClient.get<PaginatedResult<PolicyDto>>('/api/admin/PropertyPolicies/by-type', { params: query }).then(res => res.data),
};
