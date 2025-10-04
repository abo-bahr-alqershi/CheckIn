import React, { useState, useCallback, useMemo, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  IconButton,
  Card,
  CardContent,
  Chip,
  Tooltip,
  Button,
  Menu,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  Switch,
  FormControlLabel,
  Alert,
  Fade,
  useTheme,
  Skeleton,
  Snackbar,
  Divider,
  useMediaQuery,
  Slide,
  Zoom
} from '@mui/material';
import {
  ChevronLeft as ChevronLeftIcon,
  ChevronRight as ChevronRightIcon,
  Today as TodayIcon,
  CalendarToday as CalendarTodayIcon,
  Event as EventIcon,
  CheckCircle as CheckCircleIcon,
  Cancel as CancelIcon,
  Build as BuildIcon,
  AttachMoney as AttachMoneyIcon,
  MoreVert as MoreVertIcon,
  ViewModule as ViewModuleIcon,
  ViewList as ViewListIcon,
  FilterList as FilterListIcon,
  Clear as ClearIcon,
  Block as BlockIcon
} from '@mui/icons-material';
import { styled, alpha } from '@mui/material/styles';
import { 
  startOfMonth, 
  endOfMonth, 
  startOfWeek, 
  endOfWeek, 
  addDays, 
  addMonths, 
  subMonths, 
  format, 
  isSameDay, 
  isToday, 
  isSameMonth,
  parseISO,
  isWeekend
} from 'date-fns';
import { ar } from 'date-fns/locale';
import type { 
  UnitManagementData, 
  PricingTier 
} from '../../types/availability.types';

type AvailabilityStatus = 'available' | 'unavailable' | 'maintenance' | 'booked' | 'blocked';

// ===== التحسينات والأنماط المخصصة =====
const CalendarContainer = styled(Paper)(({ theme }) => ({
  padding: 0,
  borderRadius: theme.spacing(2),
  background: theme.palette.background.paper,
  boxShadow: '0 8px 32px rgba(0, 0, 0, 0.08)',
  border: `1px solid ${alpha(theme.palette.divider, 0.12)}`,
  overflow: 'hidden',
  transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
  '&:hover': {
    boxShadow: '0 12px 40px rgba(0, 0, 0, 0.12)'
  }
}));

const CalendarHeader = styled(Box)(({ theme }) => ({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  padding: theme.spacing(2.5, 3),
  background: `linear-gradient(135deg, ${theme.palette.primary.main}15, ${theme.palette.primary.main}08)`,
  borderBottom: `1px solid ${alpha(theme.palette.divider, 0.08)}`,
  backdropFilter: 'blur(10px)',
}));

const CalendarContent = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2, 3, 3, 3)
}));

const WeekdayHeader = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: 'repeat(7, 1fr)',
  gap: theme.spacing(0.5),
  marginBottom: theme.spacing(1),
  padding: theme.spacing(1, 0),
  borderBottom: `1px solid ${alpha(theme.palette.divider, 0.08)}`
}));

const WeekdayCell = styled(Typography)(({ theme }) => ({
  textAlign: 'center',
  fontWeight: 600,
  color: theme.palette.text.secondary,
  fontSize: '0.75rem',
  textTransform: 'uppercase',
  letterSpacing: 0.5,
  padding: theme.spacing(0.5)
}));

const CalendarGrid = styled(Box)(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: 'repeat(7, 1fr)',
  gap: theme.spacing(0.5),
  marginTop: theme.spacing(1)
}));

// دالة لحساب لون خلفية خلية اليوم وفق الحالة والوضع مع شرح باللغة العربية
const DayCell = styled(Card, {
  shouldForwardProp: (prop) => 
    !['isSelected', 'isToday', 'isInRange', 'status', 'pricingTier', 'mode', 'isCurrentMonth', 'isWeekend'].includes(prop as string)
})<{
  isSelected?: boolean;
  isToday?: boolean;
  isInRange?: boolean;
  status?: AvailabilityStatus;
  pricingTier?: PricingTier;
  mode?: 'availability' | 'pricing';
  isCurrentMonth?: boolean;
  isWeekend?: boolean;
}>(({ theme, isSelected, isToday, isInRange, status, pricingTier, mode, isCurrentMonth, isWeekend: isWeekendDay }) => {
  const getBackgroundColor = () => {
    if (!isCurrentMonth) {
      return alpha(theme.palette.text.disabled, 0.02);
    }

    if (isSelected) {
      return `linear-gradient(135deg, ${theme.palette.primary.main}20, ${theme.palette.primary.main}10)`;
    }

    if (isInRange) {
      return `linear-gradient(135deg, ${theme.palette.primary.main}12, ${theme.palette.primary.main}06)`;
    }

    if (isToday) {
      return `linear-gradient(135deg, ${theme.palette.secondary.main}15, ${theme.palette.secondary.main}08)`;
    }

    if (mode === 'pricing') {
      switch (pricingTier) {
        case 'normal':
          return `linear-gradient(135deg, ${theme.palette.success.main}08, ${theme.palette.success.main}04)`;
        case 'high':
          return `linear-gradient(135deg, ${theme.palette.warning.main}08, ${theme.palette.warning.main}04)`;
        case 'peak':
          return `linear-gradient(135deg, ${theme.palette.error.main}08, ${theme.palette.error.main}04)`;
        case 'discount':
          return `linear-gradient(135deg, ${theme.palette.info.main}08, ${theme.palette.info.main}04)`;
        default:
          return isWeekendDay ? alpha(theme.palette.action.hover, 0.3) : theme.palette.background.paper;
      }
    } else {
      switch (status) {
        case 'available':
          return `linear-gradient(135deg, ${theme.palette.success.main}08, ${theme.palette.success.main}04)`;
        case 'unavailable':
          return `linear-gradient(135deg, ${theme.palette.error.main}08, ${theme.palette.error.main}04)`;
        case 'maintenance':
          return `linear-gradient(135deg, ${theme.palette.warning.main}08, ${theme.palette.warning.main}04)`;
        case 'booked':
          return `linear-gradient(135deg, ${theme.palette.info.main}08, ${theme.palette.info.main}04)`;
        case 'blocked':
          // حالة محجوب: خلفية بتدرج رمادي خفيف
          return `linear-gradient(135deg, ${theme.palette.grey[500]}08, ${theme.palette.grey[500]}04)`;
        default:
          return isWeekendDay ? alpha(theme.palette.action.hover, 0.3) : theme.palette.background.paper;
      }
    }
  };

  // دالة لحساب لون إطار خلية اليوم وفق الحالة والوضع مع شرح باللغة العربية
  const getBorderColor = () => {
    if (isSelected) return theme.palette.primary.main;
    if (isInRange) return theme.palette.primary.light;
    if (isToday) return theme.palette.secondary.main;
    // Color border based on availability status
    if (mode === 'availability' && status) {
      switch (status) {
        case 'available':
          return theme.palette.success.main;
        case 'unavailable':
          return theme.palette.error.main;
        case 'maintenance':
          return theme.palette.warning.main;
        case 'booked':
          return theme.palette.info.main;
        case 'blocked':
          // حالة محجوب: لون الإطار رمادي
          return theme.palette.grey[500];
        default:
          return alpha(theme.palette.divider, 0.12);
      }
    }
    // Default border for other modes
    return alpha(theme.palette.divider, 0.12);
  };

  return {
    background: getBackgroundColor(),
    border: `2px solid ${getBorderColor()}`,
    cursor: isCurrentMonth ? 'pointer' : 'default',
    transition: 'all 0.2s cubic-bezier(0.4, 0, 0.2, 1)',
    position: 'relative',
    minHeight: 80,
    opacity: isCurrentMonth ? 1 : 0.4,
    '&:hover': isCurrentMonth ? {
      transform: 'translateY(-1px)',
      boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
      borderColor: theme.palette.primary.main,
      '& .day-number': {
        transform: 'scale(1.1)',
        fontWeight: 700
      }
    } : {},
    ...(isSelected && {
      transform: 'scale(1.02)',
      zIndex: 10,
      boxShadow: '0 8px 25px rgba(0, 0, 0, 0.12)'
    })
  };
});

const UnitIndicator = styled(Box, {
  shouldForwardProp: (prop) => prop !== 'isActive'
})<{ color: string; isActive?: boolean }>(({ theme, color, isActive }) => ({
  width: isActive ? 10 : 6,
  height: isActive ? 10 : 6,
  borderRadius: '50%',
  backgroundColor: color,
  display: 'inline-block',
  marginRight: theme.spacing(0.5),
  transition: 'all 0.2s ease',
  boxShadow: isActive ? `0 0 8px ${alpha(color, 0.4)}` : 'none'
}));

const LegendContainer = styled(Box)(({ theme }) => ({
  marginTop: theme.spacing(2),
  padding: theme.spacing(2),
  background: alpha(theme.palette.background.paper, 0.8),
  borderRadius: theme.spacing(1.5),
  border: `1px solid ${alpha(theme.palette.divider, 0.08)}`,
  backdropFilter: 'blur(10px)'
}));

const ControlBar = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'space-between',
  gap: theme.spacing(2),
  marginBottom: theme.spacing(2),
  padding: theme.spacing(1.5, 2),
  background: alpha(theme.palette.background.paper, 0.8),
  borderRadius: theme.spacing(1),
  border: `1px solid ${alpha(theme.palette.divider, 0.08)}`,
  backdropFilter: 'blur(10px)'
}));

// ===== أنواع البيانات المحسنة =====
interface DateRangeCalendarProps {
  units: UnitManagementData[];
  onDateSelect?: (dates: { start: Date | null; end: Date | null }) => void;
  onMonthChange?: (range: { start: Date; end: Date }) => void;
  onUnitClick?: (unit: UnitManagementData, date: Date) => void;
  selectedDateRange?: { start: Date | null; end: Date | null };
  mode?: 'availability' | 'pricing';
  showTimeSelection?: boolean;
  allowRangeSelection?: boolean;
  compactView?: boolean;
  showLegend?: boolean;
  loading?: boolean;
  maxSelectableDays?: number;
  minSelectableDate?: Date;
  maxSelectableDate?: Date;
  onError?: (error: string) => void;
}

// ===== دوال مساعدة محسنة =====
// دالة مساعدة: إرجاع أيقونة تمثل حالة الإتاحة لليوم مع شرح باللغة العربية
const getStatusIcon = (status: AvailabilityStatus, size: 'small' | 'medium' = 'small') => {
  const iconSize = size === 'small' ? 'small' : 'medium';
  switch (status) {
    case 'available':
      // حالة متاحة: عرض أيقونة دائرة صح باللون الأخضر
      return <CheckCircleIcon fontSize={iconSize} sx={{ color: 'success.main' }} />;
    case 'unavailable':
      // حالة غير متاحة: عرض أيقونة إلغاء باللون الأحمر
      return <CancelIcon fontSize={iconSize} sx={{ color: 'error.main' }} />;
    case 'maintenance':
      // حالة صيانة: عرض أيقونة أدوات باللون البرتقالي
      return <BuildIcon fontSize={iconSize} sx={{ color: 'warning.main' }} />;
    case 'booked':
      // حالة محجوز: عرض أيقونة حدث باللون الأزرق
      return <EventIcon fontSize={iconSize} sx={{ color: 'info.main' }} />;
    case 'blocked':
      // حالة محجوب: عرض أيقونة حظر باللون الرمادي
      return <BlockIcon fontSize={iconSize} sx={{ color: 'text.secondary' }} />;
    default:
      // حالة افتراضية: أيقونة التقويم باللون المعطل
      return <CalendarTodayIcon fontSize={iconSize} sx={{ color: 'text.disabled' }} />;
  }
};

// دالة مساعدة: إرجاع أيقونة السعر بناءً على الفئة مع شرح باللغة العربية
const getPricingIcon = (tier: PricingTier, size: 'small' | 'medium' = 'small') => {
  const iconSize = size === 'small' ? 'small' : 'medium';
  const colorMap = {
    peak: 'error.main',
    high: 'warning.main',
    discount: 'info.main',
    normal: 'success.main',
    custom: 'secondary.main'
  };
  
  return <AttachMoneyIcon fontSize={iconSize} sx={{ color: colorMap[tier] || 'text.secondary' }} />;
};

// دالة مساعدة: إرجاع اللون المناسب لحالة الإتاحة لليوم مع شرح باللغة العربية
const getStatusColor = (status: AvailabilityStatus): string => {
  const colorMap = {
    available: '#2e7d32',    // يوم متاح: أخضر
    unavailable: '#d32f2f',  // يوم غير متاح: أحمر
    maintenance: '#ed6c02',  // يوم صيانة: برتقالي
    booked: '#0288d1',        // يوم محجوز: أزرق
    blocked: '#757575'        // يوم محجوب: رمادي
  };
  return colorMap[status] || '#757575'; // اللون الافتراضي للتواريخ غير المعروفة
};

// دالة مساعدة: إرجاع اللون المناسب للفئة السعرية لليوم مع شرح باللغة العربية
const getPricingColor = (tier: PricingTier): string => {
  const colorMap = {
    normal: '#2e7d32',
    high: '#ed6c02',
    peak: '#d32f2f',
    discount: '#0288d1',
    custom: '#7b1fa2'
  };
  return colorMap[tier] || '#757575';
};

// ===== المكون الرئيسي المحسن =====
const DateRangeCalendar: React.FC<DateRangeCalendarProps> = ({
  units = [],
  onDateSelect,
  onMonthChange,
  onUnitClick,
  selectedDateRange,
  mode: modeProp,
  showTimeSelection = false,
  allowRangeSelection = true,
  compactView = false,
  showLegend = true,
  loading = false,
  maxSelectableDays,
  minSelectableDate,
  maxSelectableDate,
  onError
}) => {
  const mode = modeProp ?? 'availability';
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  
  // الحالات المحسنة
  const [currentMonth, setCurrentMonth] = useState(new Date());
  const [selectedDates, setSelectedDates] = useState<{ start: Date | null; end: Date | null }>({
    start: selectedDateRange?.start || null,
    end: selectedDateRange?.end || null
  });
  const [hoveredDate, setHoveredDate] = useState<Date | null>(null);
  const [selectedUnit, setSelectedUnit] = useState<string | null>(null);
  const [showAllUnits, setShowAllUnits] = useState(true);
  const [viewMode, setViewMode] = useState<'grid' | 'compact'>('grid');
  const [showSnackbar, setShowSnackbar] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState('');
  const [animationKey, setAnimationKey] = useState(0);

  // Notify parent when the displayed month changes
  useEffect(() => {
    const start = startOfMonth(currentMonth);
    const end = endOfMonth(currentMonth);
    onMonthChange?.({ start, end });
  }, [currentMonth, onMonthChange]);

  // تأثيرات جانبية
  useEffect(() => {
    setSelectedDates({
      start: selectedDateRange?.start || null,
      end: selectedDateRange?.end || null
    });
  }, [selectedDateRange]);

  useEffect(() => {
    setAnimationKey(prev => prev + 1);
  }, [currentMonth]);

  // دوال التنقل المحسنة
  const goToPreviousMonth = useCallback(() => {
    setCurrentMonth(prev => subMonths(prev, 1));
  }, []);

  const goToNextMonth = useCallback(() => {
    setCurrentMonth(prev => addMonths(prev, 1));
  }, []);

  const goToToday = useCallback(() => {
    setCurrentMonth(new Date());
  }, []);

  // معالجة الأخطاء
  const showError = useCallback((message: string) => {
    setSnackbarMessage(message);
    setShowSnackbar(true);
    onError?.(message);
  }, [onError]);

  // التحقق من صحة التاريخ
  const isDateSelectable = useCallback((date: Date) => {
    if (minSelectableDate && date < minSelectableDate) return false;
    if (maxSelectableDate && date > maxSelectableDate) return false;
    return true;
  }, [minSelectableDate, maxSelectableDate]);

  // معالجة اختيار التاريخ المحسنة
  const handleDateClick = useCallback((date: Date) => {
    if (!isDateSelectable(date)) {
      showError('لا يمكن اختيار هذا التاريخ');
      return;
    }

    if (!allowRangeSelection) {
      setSelectedDates({ start: date, end: date });
      onDateSelect?.({ start: date, end: date });
      return;
    }

    if (!selectedDates.start || (selectedDates.start && selectedDates.end)) {
      setSelectedDates({ start: date, end: null });
    } else if (selectedDates.start && !selectedDates.end) {
      const start = selectedDates.start;
      const end = date;
      
      // التحقق من الحد الأقصى للأيام
      if (maxSelectableDays) {
        const daysDiff = Math.abs((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
        if (daysDiff >= maxSelectableDays) {
          showError(`لا يمكن اختيار أكثر من ${maxSelectableDays} يوم`);
          return;
        }
      }
      
      if (start <= end) {
        setSelectedDates({ start, end });
        onDateSelect?.({ start, end });
      } else {
        setSelectedDates({ start: end, end: start });
        onDateSelect?.({ start: end, end: start });
      }
    }
  }, [selectedDates, allowRangeSelection, onDateSelect, isDateSelectable, maxSelectableDays, showError]);

  // معالجة التمرير المحسنة
  const handleDateHover = useCallback((date: Date) => {
    if (allowRangeSelection && selectedDates.start && !selectedDates.end && isDateSelectable(date)) {
      setHoveredDate(date);
    }
  }, [allowRangeSelection, selectedDates, isDateSelectable]);

  // معالجة مغادرة المؤشر
  const handleDateLeave = useCallback(() => {
    setHoveredDate(null);
  }, []);

  // معالجة النقر على الوحدة المحسنة
  const handleUnitClick = useCallback((unit: UnitManagementData, date: Date, event: React.MouseEvent) => {
    event.stopPropagation();
    onUnitClick?.(unit, date);
  }, [onUnitClick]);

  // مسح التحديد
  const clearSelection = useCallback(() => {
    setSelectedDates({ start: null, end: null });
    onDateSelect?.({ start: null, end: null });
  }, [onDateSelect]);

  // إنشاء أيام الشهر
  const monthDays = useMemo(() => {
    const monthStart = startOfMonth(currentMonth);
    const monthEnd = endOfMonth(currentMonth);
    const startDate = startOfWeek(monthStart, { weekStartsOn: 6 });
    const endDate = endOfWeek(monthEnd, { weekStartsOn: 6 });

    const days: Date[] = [];
    let day = startDate;

    while (day <= endDate) {
      days.push(new Date(day));
      day = addDays(day, 1);
    }

    return days;
  }, [currentMonth]);

  // التحقق من كون التاريخ في النطاق المحدد
  const isDateInRange = useCallback((date: Date) => {
    if (!selectedDates.start) return false;
    
    if (selectedDates.end) {
      return date >= selectedDates.start && date <= selectedDates.end;
    }
    
    if (hoveredDate && allowRangeSelection) {
      const start = selectedDates.start;
      const end = hoveredDate;
      return start <= end ? 
        (date >= start && date <= end) : 
        (date >= end && date <= start);
    }
    
    return isSameDay(date, selectedDates.start);
  }, [selectedDates, hoveredDate, allowRangeSelection]);

  // الحصول على بيانات اليوم المحسنة
  const getDayData = useCallback((date: Date) => {
    const dayData = {
      units: units.map(unit => {
        const availabilityCalendar = unit.availabilityCalendar?.find(cal => {
          const entry: any = cal;
          // support range entries with startDate/endDate or single date entries
          const start = entry.startDate ? parseISO(entry.startDate) : parseISO(cal.date);
          const end   = entry.endDate   ? parseISO(entry.endDate)   : parseISO(cal.date);
          return (date >= start && date <= end) || isSameDay(parseISO(cal.date), date);
        });
        
        return {
          ...unit,
          dayStatus: availabilityCalendar?.status || 'available',
          dayPricingTier: availabilityCalendar?.pricingTier || 'normal',
          dayPrice: availabilityCalendar?.currentPrice || unit.unit.basePrice,
          // Add dynamic currency from calendar entry
          dayCurrency: availabilityCalendar?.currency,
        };
      }),
      hasBookings: units.some(unit => 
        unit.upcomingBookings?.some(booking => 
          isSameDay(new Date(booking.startDate), date)
        )
      ),
      bookingsCount: units.reduce((count, unit) => 
        count + (unit.upcomingBookings?.filter(booking => 
          isSameDay(new Date(booking.startDate), date)
        ).length || 0), 0
      )
    };

    return dayData;
  }, [units]);

  // فلترة الوحدات المعروضة
  const filteredUnits = useMemo(() => {
    if (showAllUnits || !selectedUnit) {
      return units;
    }
    return units.filter(unit => unit.unit.unitId === selectedUnit);
  }, [units, showAllUnits, selectedUnit]);

  // إنشاء وسيلة الإيضاح المحسنة
  const legendItems = useMemo(() => {
    if (mode === 'pricing') {
      return [
        { label: 'سعر عادي', color: getPricingColor('normal'), tier: 'normal' },
        { label: 'سعر مرتفع', color: getPricingColor('high'), tier: 'high' },
        { label: 'سعر ذروة', color: getPricingColor('peak'), tier: 'peak' },
        { label: 'خصم', color: getPricingColor('discount'), tier: 'discount' },
        { label: 'مخصص', color: getPricingColor('custom'), tier: 'custom' }
      ];
    } else {
      return [
        { label: 'متاح', color: getStatusColor('available'), status: 'available' },
        { label: 'غير متاح', color: getStatusColor('unavailable'), status: 'unavailable' },
        { label: 'محجوب', color: getStatusColor('blocked'), status: 'blocked' },
        { label: 'صيانة', color: getStatusColor('maintenance'), status: 'maintenance' },
        { label: 'محجوز', color: getStatusColor('booked'), status: 'booked' }
      ];
    }
  }, [mode]);

  // أسماء أيام الأسبوع
  const weekdays = ['السبت', 'الأحد', 'الاثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة'];

  if (loading) {
    return (
      <CalendarContainer>
        <Skeleton variant="rectangular" height={60} sx={{ mb: 2 }} />
        <Skeleton variant="rectangular" height={400} />
      </CalendarContainer>
    );
  }

  return (
    <CalendarContainer>
      {/* رأس التقويم المحسن */}
      <CalendarHeader>
        <Box display="flex" alignItems="center" gap={2}>
          <Tooltip title="الشهر السابق">
            <IconButton 
              onClick={goToPreviousMonth}
              sx={{ 
                color: 'text.primary',
                '&:hover': { 
                  backgroundColor: alpha(theme.palette.primary.main, 0.1),
                  transform: 'scale(1.1)'
                }
              }}
              size="small"
            >
              <ChevronRightIcon />
            </IconButton>
          </Tooltip>
          
          <Typography 
            variant={isMobile ? "h6" : "h5"} 
            sx={{ 
              fontWeight: 600, 
              minWidth: isMobile ? 150 : 200, 
              textAlign: 'center',
              color: 'text.primary'
            }}
          >
            {format(currentMonth, 'MMMM yyyy', { locale: ar })}
          </Typography>
          
          <Tooltip title="الشهر التالي">
            <IconButton 
              onClick={goToNextMonth}
              sx={{ 
                color: 'text.primary',
                '&:hover': { 
                  backgroundColor: alpha(theme.palette.primary.main, 0.1),
                  transform: 'scale(1.1)'
                }
              }}
              size="small"
            >
              <ChevronLeftIcon />
            </IconButton>
          </Tooltip>
        </Box>
        
        <Box display="flex" alignItems="center" gap={1.5}>
          <Tooltip title="اليوم">
            <Button
              startIcon={<TodayIcon />}
              onClick={goToToday}
              variant="outlined"
              sx={{ 
                borderColor: alpha(theme.palette.primary.main, 0.3),
                color: 'text.primary',
                '&:hover': {
                  borderColor: theme.palette.primary.main,
                  backgroundColor: alpha(theme.palette.primary.main, 0.05)
                }
              }}
              size="small"
            >
              {!isMobile && 'اليوم'}
            </Button>
          </Tooltip>
          
          {units.length > 1 && (
            <FormControl size="small" sx={{ minWidth: isMobile ? 100 : 120 }}>
              <InputLabel>الوحدة</InputLabel>
              <Select
                value={selectedUnit || ''}
                onChange={(e) => setSelectedUnit(e.target.value || null)}
                sx={{ fontSize: '0.875rem' }}
              >
                <MenuItem value="">جميع الوحدات</MenuItem>
                {units.map(unit => (
                  <MenuItem key={unit.unit.unitId} value={unit.unit.unitId}>
                    {unit.unit.unitName}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}
        </Box>
      </CalendarHeader>

      <CalendarContent>
        {/* شريط التحكم */}
        <ControlBar>
          <Box display="flex" alignItems="center" gap={2}>
            <Tooltip title="تبديل العرض">
              <IconButton
                onClick={() => setViewMode(viewMode === 'grid' ? 'compact' : 'grid')}
                size="small"
                sx={{ color: 'text.secondary' }}
              >
                {viewMode === 'grid' ? <ViewListIcon /> : <ViewModuleIcon />}
              </IconButton>
            </Tooltip>
            
            <FormControlLabel
              control={
                <Switch
                  checked={showAllUnits}
                  onChange={(e) => setShowAllUnits(e.target.checked)}
                  size="small"
                />
              }
              label={
                <Typography variant="caption" color="text.secondary">
                  عرض جميع الوحدات
                </Typography>
              }
            />
          </Box>

          {(selectedDates.start || selectedDates.end) && (
            <Tooltip title="مسح التحديد">
              <IconButton onClick={clearSelection} size="small">
                <ClearIcon />
              </IconButton>
            </Tooltip>
          )}
        </ControlBar>

        {/* رؤوس أيام الأسبوع */}
        <WeekdayHeader>
          {weekdays.map(day => (
            <WeekdayCell key={day} variant="caption">
              {isMobile ? day.slice(0, 3) : day}
            </WeekdayCell>
          ))}
        </WeekdayHeader>

        {/* شبكة التقويم */}
        <Slide direction="up" in={true} timeout={300} key={animationKey}>
          <CalendarGrid>
            {monthDays.map((date, index) => {
              const dayData = getDayData(date);
              const isSelected = !!(selectedDates.start && isSameDay(date, selectedDates.start));
              const inRange = isDateInRange(date);
              const isCurrentMonth = isSameMonth(date, currentMonth);
              const dayIsToday = isToday(date);
              const isWeekendDay = isWeekend(date);
              const isSelectable = isDateSelectable(date);

              return (
                <Zoom in={true} timeout={200 + index * 20} key={index}>
                  <DayCell
                    className="calendar-day"
                    isSelected={isSelected}
                    isToday={dayIsToday}
                    isInRange={inRange}
                    mode={mode}
                    status={dayData.units[0]?.dayStatus}
                    pricingTier={dayData.units[0]?.dayPricingTier}
                    isCurrentMonth={isCurrentMonth}
                    isWeekend={isWeekendDay}
                    onClick={() => isCurrentMonth && isSelectable && handleDateClick(date)}
                    onMouseEnter={() => handleDateHover(date)}
                    onMouseLeave={handleDateLeave}
                    elevation={inRange ? 4 : 1}
                    sx={{
                      cursor: isCurrentMonth && isSelectable ? 'pointer' : 'not-allowed'
                    }}
                  >
                    <CardContent sx={{ p: 1.5, '&:last-child': { pb: 1.5 } }}>
                      {/* رقم اليوم */}
                      <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                        <Typography 
                          variant="body2" 
                          className="day-number"
                          sx={{ 
                            fontWeight: dayIsToday ? 700 : 500,
                            color: dayIsToday ? 'secondary.main' : 
                                   isCurrentMonth ? 'text.primary' : 'text.disabled',
                            transition: 'all 0.2s ease',
                            fontSize: dayIsToday ? '1rem' : '0.875rem'
                          }}
                        >
                          {format(date, 'd')}
                        </Typography>
                        
                        {!compactView && dayData.hasBookings && (
                          <Chip 
                            size="small" 
                            label={dayData.bookingsCount > 1 ? `${dayData.bookingsCount}` : "حجز"}
                            color="primary" 
                            sx={{ 
                              fontSize: '0.6rem', 
                              height: 18,
                              '& .MuiChip-label': { px: 0.75 },
                              animation: 'pulse 2s infinite',
                              '@keyframes pulse': {
                                '0%': { transform: 'scale(1)' },
                                '50%': { transform: 'scale(1.05)' },
                                '100%': { transform: 'scale(1)' }
                              }
                            }}
                          />
                        )}
                      </Box>

                      {/* مؤشرات الوحدات */}
                      {!compactView && viewMode === 'grid' && (
                        mode === 'availability' ? (
                          <Box sx={{ minHeight: 40 }}>
                            {filteredUnits.slice(0, isMobile ? 2 : 3).map(unit => {
                              const unitData = dayData.units.find(u => u.unit.unitId === unit.unit.unitId);
                              if (!unitData) return null;
                              const color = getStatusColor(unitData.dayStatus);
                              const isActiveUnit = selectedUnit === unit.unit.unitId;

                              return (
                                <Tooltip 
                                  key={unit.unit.unitId}
                                  title={`${unitData.dayPrice} ${unitData.dayCurrency}`}
                                  placement="top"
                                >
                                  <Box 
                                    display="flex" 
                                    alignItems="center" 
                                    gap={0.5}
                                    onClick={(e) => handleUnitClick(unit, date, e)}
                                    sx={{ 
                                      cursor: 'pointer',
                                      '&:hover': { 
                                        bgcolor: alpha(color, 0.1),
                                        transform: 'translateX(2px)'
                                      },
                                      borderRadius: 1,
                                      p: 0.5,
                                      mb: 0.5,
                                      transition: 'all 0.2s ease',
                                      border: isActiveUnit ? `1px solid ${color}` : 'none'
                                    }}
                                  >
                                    <UnitIndicator color={color} isActive={isActiveUnit} />
                                    <Typography 
                                      variant="caption" 
                                      sx={{ fontSize: '0.65rem', fontWeight: isActiveUnit ? 600 : 400, color: isActiveUnit ? color : 'text.secondary' }}
                                    >
                                      {unitData.dayPrice} {unitData.dayCurrency}
                                    </Typography>
                                  </Box>
                                </Tooltip>
                              );
                            })}
                            {filteredUnits.length > (isMobile ? 2 : 3) && (
                              <Typography variant="caption" color="text.secondary" sx={{ display: 'block', textAlign: 'center', fontSize: '0.6rem', fontStyle: 'italic' }}>
                                +{filteredUnits.length - (isMobile ? 2 : 3)} أخرى
                              </Typography>
                            )}
                          </Box>
                        ) : (
                          <Box sx={{ minHeight: 40, display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                            <Typography 
                              variant="body2" 
                              sx={{ fontWeight: 500, color: 'text.primary' }}
                            >
                              {dayData.units[0]?.dayPrice} {dayData.units[0]?.dayCurrency}
                            </Typography>
                          </Box>
                        )
                      )}

                      {/* العرض المضغوط */}
                      {(compactView || viewMode === 'compact') && (
                        <Box display="flex" justifyContent="center" alignItems="center" mt={0.5}>
                          {mode === 'pricing' ? 
                            getPricingIcon('normal', 'small') : 
                            getStatusIcon('available', 'small')
                          }
                          {dayData.hasBookings && (
                            <Box 
                              sx={{ 
                                width: 6, 
                                height: 6, 
                                borderRadius: '50%', 
                                bgcolor: 'primary.main',
                                ml: 0.5,
                                animation: 'blink 1s infinite',
                                '@keyframes blink': {
                                  '0%, 50%': { opacity: 1 },
                                  '51%, 100%': { opacity: 0.3 }
                                }
                              }} 
                            />
                          )}
                        </Box>
                      )}

                      {/* مؤشر نهاية الأسبوع */}
                      {isWeekendDay && isCurrentMonth && (
                        <Box
                          sx={{
                            position: 'absolute',
                            top: 2,
                            right: 2,
                            width: 4,
                            height: 4,
                            borderRadius: '50%',
                            bgcolor: 'warning.main',
                            opacity: 0.6
                          }}
                        />
                      )}
                    </CardContent>
                  </DayCell>
                </Zoom>
              );
            })}
          </CalendarGrid>
        </Slide>

        {/* وسيلة الإيضاح المحسنة */}
        {showLegend && (
          <Fade in={true} timeout={600}>
            <LegendContainer>
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={1.5}>
                <Typography variant="subtitle2" sx={{ fontWeight: 600, color: 'text.primary' }}>
                  وسيلة الإيضاح:
                </Typography>
                <Chip 
                  label={mode === 'pricing' ? 'أسعار' : 'حالة التوفر'} 
                  size="small" 
                  color="primary" 
                  variant="outlined"
                />
              </Box>
              
              <Box display="flex" flexWrap="wrap" gap={2}>
                {legendItems.map((item, index) => (
                  <Fade in={true} timeout={300 + index * 100} key={index}>
                    <Box 
                      display="flex" 
                      alignItems="center" 
                      gap={1}
                      sx={{
                        p: 1,
                        borderRadius: 1,
                        '&:hover': {
                          bgcolor: alpha(item.color, 0.05),
                          transform: 'translateY(-1px)'
                        },
                        transition: 'all 0.2s ease'
                      }}
                    >
                      <UnitIndicator color={item.color} />
                      <Typography 
                        variant="caption"
                        sx={{ 
                          fontWeight: 500,
                          color: 'text.secondary'
                        }}
                      >
                        {item.label}
                      </Typography>
                    </Box>
                  </Fade>
                ))}
              </Box>

              {/* معلومات إضافية */}
              <Divider sx={{ my: 1.5 }} />
              <Box display="flex" flexWrap="wrap" gap={3} sx={{ fontSize: '0.75rem' }}>
                <Box display="flex" alignItems="center" gap={0.5}>
                  <Box sx={{ width: 4, height: 4, borderRadius: '50%', bgcolor: 'warning.main' }} />
                  <Typography variant="caption" color="text.secondary">نهاية الأسبوع</Typography>
                </Box>
                <Box display="flex" alignItems="center" gap={0.5}>
                  <Box sx={{ width: 4, height: 4, borderRadius: '50%', bgcolor: 'primary.main' }} />
                  <Typography variant="caption" color="text.secondary">يحتوي على حجوزات</Typography>
                </Box>
                <Box display="flex" alignItems="center" gap={0.5}>
                  <Box sx={{ width: 4, height: 4, borderRadius: '50%', bgcolor: 'secondary.main' }} />
                  <Typography variant="caption" color="text.secondary">اليوم الحالي</Typography>
                </Box>
              </Box>
            </LegendContainer>
          </Fade>
        )}

        {/* معلومات الفترة المحددة */}
        {selectedDates.start && selectedDates.end && (
          <Slide direction="up" in={true} timeout={400}>
            <Alert 
              severity="info" 
              sx={{ 
                mt: 2,
                borderRadius: 2,
                bgcolor: alpha(theme.palette.info.main, 0.05),
                border: `1px solid ${alpha(theme.palette.info.main, 0.2)}`,
                '& .MuiAlert-icon': { color: 'info.main' }
              }}
              action={
                <IconButton size="small" onClick={clearSelection}>
                  <ClearIcon fontSize="small" />
                </IconButton>
              }
            >
              <Typography variant="body2" sx={{ fontWeight: 500 }}>
                الفترة المحددة: {format(selectedDates.start, 'dd/MM/yyyy', { locale: ar })} - {format(selectedDates.end, 'dd/MM/yyyy', { locale: ar })}
                <Box component="span" sx={{ display: 'block', fontSize: '0.875rem', color: 'text.secondary', mt: 0.5 }}>
                  ({Math.ceil((selectedDates.end.getTime() - selectedDates.start.getTime()) / (1000 * 60 * 60 * 24)) + 1} يوم)
                  {maxSelectableDays && ` من أصل ${maxSelectableDays} يوم مسموح`}
                </Box>
              </Typography>
            </Alert>
          </Slide>
        )}

        {/* رسالة عدم وجود بيانات */}
        {units.length === 0 && (
          <Alert 
            severity="warning" 
            sx={{ 
              mt: 2,
              textAlign: 'center',
              borderRadius: 2
            }}
          >
            <Typography variant="body2">
              لا توجد وحدات متاحة للعرض
            </Typography>
          </Alert>
        )}
      </CalendarContent>

      {/* إشعار الأخطاء */}
      <Snackbar
        open={showSnackbar}
        autoHideDuration={4000}
        onClose={() => setShowSnackbar(false)}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert 
          onClose={() => setShowSnackbar(false)} 
          severity="error" 
          sx={{ 
            width: '100%',
            borderRadius: 2
          }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </CalendarContainer>
  );
};

export default DateRangeCalendar;