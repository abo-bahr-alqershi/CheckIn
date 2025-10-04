# Splash Screen & Main Screen Implementation

## 📱 الملفات المضافة

### 1. `lib/presentation/screens/main_screen.dart`
**الشاشة الرئيسية مع Bottom Navigation Bar**

#### المميزات:
- **5 تبويبات رئيسية**:
  - 🏠 الرئيسية (Home)
  - 🔍 البحث (Search) 
  - 📚 حجوزاتي (Bookings)
  - 💬 المحادثة (Chat) - مع Badge للإشعارات
  - 👤 الملف الشخصي (Profile)

- **تقنيات مستخدمة**:
  - `PageView` للتنقل السلس
  - `PageController` للتحكم في التنقل
  - `BottomNavigationBar` مع تصميم مخصص
  - `BlocBuilder` لعرض عدد الإشعارات
  - `SafeArea` للحماية من الشاشات المقطوعة

### 2. `lib/presentation/screens/splash_screen.dart`
**شاشة البداية مع تأثيرات بصرية متقدمة**

#### المميزات:
- **مدة العرض**: 3 ثوانٍ
- **التحقق الذكي من المصادقة**:
  - مسجل دخول → Main Screen
  - غير مسجل → Login Screen
- **التأثيرات البصرية**:
  - حركة تكبير الشعار (Elastic Animation)
  - حركة انزلاق النص (Slide Animation)
  - تدرج لوني في الخلفية (Gradient)
  - مؤشر تحميل متحرك

## 🔄 التحديثات المطلوبة

### 1. `lib/routes/app_router.dart`
- تم تحديث المسار الرئيسي `/` ليعرض `SplashScreen`
- تم إضافة مسار `/main` ليعرض `MainScreen`

### 2. `lib/core/constants/route_constants.dart`
- تم إضافة `static const String main = '/main';`

## 🎨 التصميم

### Splash Screen:
```dart
// خلفية متدرجة
LinearGradient(
  colors: [
    AppTheme.primary,
    AppTheme.primary.withOpacity(0.8),
    AppTheme.primary.withOpacity(0.6),
  ],
)

// شعار مع تأثير الظل
Container(
  decoration: BoxDecoration(
    color: Colors.white,
    borderRadius: BorderRadius.circular(30),
    boxShadow: [/* ظل مخصص */],
  ),
)
```

### Main Screen:
```dart
// Bottom Navigation مع Badge
BottomNavigationBar(
  type: BottomNavigationBarType.fixed,
  selectedItemColor: AppTheme.primary,
  unselectedItemColor: AppTheme.textSecondary,
  // ... المزيد من التخصيص
)
```

## 🚀 كيفية الاستخدام

### 1. تشغيل التطبيق:
```dart
// يبدأ من Splash Screen
context.go('/'); // أو RouteConstants.splash
```

### 2. الانتقال للشاشة الرئيسية:
```dart
// بعد التحقق من المصادقة
context.go('/main'); // أو RouteConstants.main
```

### 3. التنقل بين التبويبات:
```dart
// يتم تلقائياً عبر Bottom Navigation
// أو برمجياً:
_pageController.animateToPage(index, duration: Duration(milliseconds: 300));
```

## 📋 المتطلبات

### Dependencies المطلوبة:
```yaml
dependencies:
  flutter_bloc: ^8.1.3
  go_router: ^12.1.3
```

### Imports المطلوبة:
```dart
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
```

## 🔧 التخصيص

### تغيير مدة Splash Screen:
```dart
// في splash_screen.dart
Future.delayed(const Duration(milliseconds: 3000), () {
  // تغيير من 3000 إلى القيمة المطلوبة
});
```

### إضافة تبويب جديد:
```dart
// في main_screen.dart
final List<Widget> _pages = [
  const HomePage(),
  const SearchPage(),
  const MyBookingsPage(),
  const ConversationsPage(),
  const ProfilePage(),
  const NewTabPage(), // إضافة تبويب جديد
];
```

## 🎯 الخطوات التالية

1. **إضافة Onboarding Screen** (اختياري)
2. **تحسين الانتقالات** بين الشاشات
3. **إضافة Deep Linking** للتبويبات
4. **تحسين الأداء** مع Lazy Loading
5. **إضافة اختبارات** للشاشات الجديدة

---

**تم الإنشاء بواسطة**: AI Assistant  
**التاريخ**: ${new Date().toLocaleDateString('ar-SA')}  
**الإصدار**: 1.0.0