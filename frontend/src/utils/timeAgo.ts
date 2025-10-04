export function timeAgo(date: string | Date): string {
  const now = new Date();
  const then = typeof date === 'string' ? new Date(date) : date;
  const diff = now.getTime() - then.getTime();
  const seconds = Math.floor(diff / 1000);
  if (seconds < 60) {
    if (seconds <= 1) return 'منذ ثانية واحدة';
    if (seconds === 2) return 'منذ ثانيتين';
    return `منذ ${seconds} ثوانٍ`;
  }
  const minutes = Math.floor(seconds / 60);
  if (minutes < 60) {
    if (minutes === 1) return 'منذ دقيقة واحدة';
    if (minutes === 2) return 'منذ دقيقتين';
    return `منذ ${minutes} ${minutes < 5 ? 'دقائق' : 'دقيقة'}`.replace('دقيقة', 'دقائق');
  }
  const hours = Math.floor(minutes / 60);
  if (hours < 24) {
    if (hours === 1) return 'منذ ساعة واحدة';
    if (hours === 2) return 'منذ ساعتين';
    return `منذ ${hours} ساعات`;
  }
  const days = Math.floor(hours / 24);
  if (days < 7) {
    if (days === 1) return 'منذ يوم واحد';
    if (days === 2) return 'منذ يومين';
    return `منذ ${days} أيام`;
  }
  const weeks = Math.floor(days / 7);
  if (weeks < 5) {
    if (weeks === 1) return 'منذ أسبوع واحد';
    if (weeks === 2) return 'منذ أسبوعين';
    return `منذ ${weeks} أسابيع`;
  }
  const months = Math.floor(days / 30);
  if (months < 12) {
    if (months === 1) return 'منذ شهر واحد';
    if (months === 2) return 'منذ شهرين';
    return `منذ ${months} أشهر`;
  }
  const years = Math.floor(months / 12);
  if (years === 1) return 'منذ عام واحد';
  if (years === 2) return 'منذ عامين';
  return `منذ ${years} سنوات`;
} 