import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AdminReviewsService } from '../services/admin-reviews.service';
import type { ReviewDto } from '../types/review.types';
import type { ResultDto } from '../types/common.types';
import type { GetReviewsByPropertyQuery, GetReviewsByUserQuery, GetAllReviewsQuery } from '../types/review.types';

/**
 * هوك لإدارة استعلامات وعمليات التقييمات للوحة الإدارة
 * يعزل التعامل مع react-query وخدمات التقييمات في مكان واحد
 * @param params معايير الاستعلام (حالة، فلاتر، صفحات)
 * @returns قائمة التقييمات، حالات التحميل والأخطاء، ودوال الموافقة والحذف
 */
export const useAdminReviews = (params: Record<string, any>) => {
  const queryClient = useQueryClient();

  // جلب جميع التقييمات مع تطبيق الفلاتر من السيرفر
  const { data: reviewsData, isLoading, error } = useQuery<ReviewDto[], Error>({
    queryKey: ['admin-reviews', params],
    queryFn: () => AdminReviewsService.getAll(params as GetAllReviewsQuery),
  });

  // الموافقة على تقييم
  const approveReview = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (reviewId) => AdminReviewsService.approve(reviewId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-reviews', params] });
    },
  });

  // حذف تقييم
  const deleteReview = useMutation<ResultDto<boolean>, Error, string>({
    mutationFn: (reviewId) => AdminReviewsService.deleteReview(reviewId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-reviews', params] });
    },
  });

  return { reviewsData, isLoading, error, approveReview, deleteReview };
}; 