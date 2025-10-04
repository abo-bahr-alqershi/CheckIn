# برومبت احترافي لبناء الفرونت اند - React TypeScript + Vite

أنت مطور frontend محترف متخصص في React TypeScript + Vite. مهمتك بناء نظام إدارة شامل (Admin & Property Owner Dashboard) وفقاً للمواصفات التالية:

## 🎯 المهمة الأساسية
بناء frontend متكامل لنظام حجوزات عام يدعم الفنادق، السيارات، وأنواع الكيانات المختلفة مع لوحتي تحكم منفصلتين:
1. **Admin Dashboard** - للإدارة العامة للنظام
2. **Property Owner Dashboard** - لملاك الكيانات والموظفين

## 📋 المتطلبات الأساسية

### 1. التقنيات المطلوبة
- **React 18** مع TypeScript
- **Vite** كأداة البناء
- **React Router DOM** للتنقل
- **Zustand** لإدارة الحالة
- **React Query** للبيانات
- **React Hook Form** للنماذج
- **Tailwind CSS** للتصميم
- **Recharts** للرسوم البيانية
- **Headless UI** للمكونات
- **React Table** للجداول

### 2. الهيكلة المطلوبة
```
src/
├── components/ui/          # مكونات UI الأساسية
├── components/common/      # مكونات مشتركة
├── components/charts/      # مكونات الرسوم البيانية
├── pages/admin/           # صفحات الإدارة
├── pages/property/        # صفحات إدارة الكيانات
├── pages/shared/          # صفحات مشتركة
├── hooks/                 # Custom React Hooks
├── services/              # خدمات API
├── types/                 # تعريفات TypeScript
├── utils/                 # دوال مساعدة
├── stores/                # إدارة الحالة
├── constants/             # الثوابت
├── layouts/               # تخطيطات الصفحات
└── assets/                # الصور والأيقونات
```

## 🔍 متطلبات التحليل والتطابق

### 1. تحليل Backend Commands & Queries
يجب عليك تحليل قائمة الـ Commands والـ Queries المتوفرة:

**Commands (110 command):**
- ActivateUser, AddServiceToBooking, AddStaff, ApproveProperty, ApproveReview... إلخ

**Queries (142 query):**
- GetAdminActivityLogs, GetAdminDashboard, GetAllAmenities, GetAllFieldTypes... إلخ

### 2. ضمان التطابق التام
- **أنواع البيانات (Types)**: يجب أن تتطابق مع نماذج Backend
- **خدمات API**: يجب أن تغطي جميع الـ Commands والـ Queries
- **واجهات المستخدم**: يجب أن تدعم كل وظيفة متاحة
- **التحقق من الصحة**: يجب أن يتطابق مع قواعد Backend

### 3. تحليل توزيع الصلاحيات
تحليل التوزيع المحدد لكل endpoint:
- **[Admin]**: وظائف الإدارة العامة
- **[Property]**: وظائف ملاك الكيانات
- **[Client]**: وظائف العملاء
- **[Common]**: وظائف مشتركة

## 🏗️ خطة التنفيذ المطلوبة

### المرحلة 1: إعداد البيئة والهيكلة
1. **إنشاء مشروع Vite + React + TypeScript**
2. **تثبيت جميع المكتبات المطلوبة**
3. **إعداد Tailwind CSS والتكوينات**
4. **إنشاء هيكل المجلدات الكامل**
5. **إعداد ESLint, Prettier, والتكوينات**

### المرحلة 2: الطبقات الأساسية (الأولوية القصوى)
1. **طبقة الأنواع (Types Layer)**
   - تحليل جميع الـ Commands والـ Queries
   - إنشاء interfaces لجميع نماذج البيانات
   - تعريف Request/Response types
   - إنشاء Enums للحالات والأنواع

2. **طبقة الخدمات (Services Layer)**
   - إعداد Axios configuration
   - إنشاء base API service
   - تطوير services لكل مجموعة وظائف
   - تطبيق error handling شامل

### المرحلة 3: الطبقات الداعمة
1. **طبقة إدارة الحالة (State Management)**
   - إعداد Zustand stores
   - تطبيق authentication state
   - إدارة cache للبيانات
   - إعداد React Query

2. **طبقة الأدوات والمساعدات (Utils & Helpers)**
   - دوال validation
   - formatters للبيانات
   - date/time utilities
   - file upload helpers

### المرحلة 4: مكونات UI الأساسية
1. **مكونات UI قابلة للإعادة الاستخدام**
   - Button, Input, Modal, DataTable
   - Charts components
   - Form components
   - Status badges

2. **مكونات مشتركة**
   - Navigation components
   - Layout components
   - Error boundaries
   - Loading states

### المرحلة 5: صفحات Admin Dashboard
1. **Dashboard الرئيسي**
   - KPIs والإحصائيات
   - Charts والرسوم البيانية
   - Recent activities
   - System alerts

2. **إدارة المستخدمين**
   - Users list & management
   - Roles & permissions
   - User activity logs
   - User creation/editing

3. **إدارة الكيانات**
   - Properties approval
   - Property types management
   - Fields management
   - Bulk operations

4. **إدارة الحجوزات**
   - Bookings overview
   - Booking analytics
   - Payment management
   - Refunds handling

5. **إدارة المراجعات**
   - Reviews moderation
   - Sentiment analysis
   - Bulk approval/rejection

6. **التقارير والتحليلات**
   - Custom reports
   - Export functionality
   - Scheduled reports
   - Data visualization

### المرحلة 6: صفحات Property Owner Dashboard
1. **Dashboard الرئيسي**
   - Property performance
   - Revenue analytics
   - Occupancy rates
   - Recent bookings

2. **إدارة الكيانات**
   - My properties list
   - Property creation/editing
   - Images management
   - Policies & services

3. **إدارة الوحدات**
   - Units management
   - Availability calendar
   - Pricing management
   - Bulk updates

4. **إدارة الحجوزات**
   - Bookings calendar
   - Check-in/out process
   - Booking services
   - Customer communication

5. **إدارة الموظفين**
   - Staff management
   - Schedules
   - Performance tracking

6. **التقارير المتخصصة**
   - Property analytics
   - Revenue reports
   - Customer reports
   - Performance comparison

### المرحلة 7: التحسين والتطوير
1. **تحسين الأداء**
   - Code splitting
   - Lazy loading
   - Image optimization
   - Bundle analysis

2. **تحسين UX/UI**
   - Responsive design
   - Loading states
   - Error handling
   - Accessibility

3. **اختبار ومراجعة**
   - Unit testing
   - Integration testing
   - Performance testing
   - Security review

## 📝 المتطلبات الخاصة

### 1. معايير الكود
- **TypeScript strict mode**
- **ESLint + Prettier**
- **Conventional commits**
- **Component documentation**
- **Error boundaries**

### 2. الأمان والصلاحيات
- **Role-based access control**
- **Route protection**
- **API authentication**
- **Data validation**
- **XSS protection**

### 3. الأداء والتحسين
- **Lazy loading للصفحات**
- **Image optimization**
- **Bundle splitting**
- **Caching strategies**
- **Memory management**

### 4. تجربة المستخدم
- **Loading states**
- **Error messages**
- **Success notifications**
- **Responsive design**
- **Accessibility compliance**

## 🎨 التصميم والواجهة

### 1. نظام التصميم
- **Modern & Professional**
- **Dark/Light themes**
- **Consistent spacing**
- **Typography hierarchy**
- **Color scheme**

### 2. التفاعل والحركة
- **Smooth animations**
- **Hover effects**
- **Loading animations**
- **Transition effects**
- **Micro-interactions**

### 3. التخطيط والتنظيم
- **Sidebar navigation**
- **Breadcrumb navigation**
- **Tabs organization**
- **Modal dialogs**
- **Dropdown menus**

## 🚀 معايير التسليم

### 1. الكود
- **Clean & maintainable**
- **Well-documented**
- **Type-safe**
- **Reusable components**
- **Consistent patterns**

### 2. الوظائف
- **All endpoints covered**
- **Complete CRUD operations**
- **Advanced filtering**
- **Bulk operations**
- **Export capabilities**

### 3. التوافق
- **Cross-browser compatibility**
- **Mobile responsive**
- **Performance optimized**
- **SEO friendly**
- **Accessibility compliant**

## 📚 التوثيق المطلوب

### 1. التوثيق التقني
- **API integration guide**
- **Component library**
- **State management guide**
- **Build & deployment**
- **Troubleshooting guide**

### 2. دليل المستخدم
- **Admin user guide**
- **Property owner guide**
- **Feature documentation**
- **FAQ section**
- **Video tutorials**

## ⚠️ نقاط مهمة يجب مراعاتها

1. **ابدأ بالتحليل العميق** لجميع الـ Commands والـ Queries
2. **تأكد من التطابق التام** بين Frontend وBackend
3. **اتبع الأولويات المحددة** في خطة التنفيذ
4. **طبق best practices** في كل مرحلة
5. **اختبر كل وظيفة** قبل الانتقال للتالية
6. **وثق كل شيء** بشكل مفصل
7. **تأكد من الأمان** في كل خطوة

## 🎯 الهدف النهائي

بناء نظام إدارة متكامل وعالي الجودة يوفر:
- **تجربة مستخدم متميزة**
- **أداء عالي وسرعة**
- **أمان وموثوقية**
- **قابلية للتوسع**
- **سهولة الصيانة**

ابدأ بالتحليل والتخطيط، ثم انتقل إلى التنفيذ وفقاً للمراحل المحددة. تأكد من التطابق التام مع Backend في كل خطوة.