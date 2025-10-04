# نظام حفظ البيانات المحلية - Yemen Booking App

## نظرة عامة

تم تطوير نظام متكامل لحفظ البيانات محلياً في تطبيق Yemen Booking لضمان عمل التطبيق حتى في حالة عدم وجود اتصال بالإنترنت. النظام يدعم حفظ أنواع العقارات وأنواع الوحدات والحقول الديناميكية.

## الميزات الرئيسية

### 1. حفظ البيانات محلياً
- **أنواع العقارات**: حفظ جميع أنواع العقارات المتاحة
- **أنواع الوحدات**: حفظ أنواع الوحدات حسب نوع العقار
- **الحقول الديناميكية**: حفظ الحقول الديناميكية القابلة للفلترة

### 2. إدارة الاتصال
- **فحص الاتصال**: التحقق من حالة الاتصال بالإنترنت
- **جودة الاتصال**: قياس جودة الاتصال
- **مزامنة ذكية**: تحديث البيانات عند توفر الاتصال

### 3. واجهة مستخدم متقدمة
- **فلترة ديناميكية**: عرض أنواع الوحدات حسب نوع العقار المحدد
- **حقول متقدمة**: عرض الحقول الديناميكية حسب نوع الوحدة
- **عرض الفلاتر**: عرض جميع الفلاتر المحددة في واجهة جميلة

## البنية التقنية

### الخدمات الرئيسية

#### 1. LocalDataService
```dart
class LocalDataService {
  // حفظ أنواع العقارات
  Future<bool> savePropertyTypes(List<PropertyTypeModel> propertyTypes)
  
  // جلب أنواع العقارات المحفوظة
  List<PropertyTypeModel> getPropertyTypes()
  
  // حفظ أنواع الوحدات
  Future<bool> saveUnitTypes(List<UnitTypeModel> unitTypes)
  
  // جلب أنواع الوحدات حسب نوع العقار
  List<UnitTypeModel> getUnitTypesByPropertyType(String propertyTypeId)
  
  // حفظ الحقول الديناميكية
  Future<bool> saveDynamicFields(List<UnitTypeFieldModel> fields)
  
  // جلب الحقول الديناميكية القابلة للفلترة
  List<UnitTypeFieldModel> getFilterableFieldsByUnitType(String unitTypeId)
}
```

#### 2. ConnectivityService
```dart
class ConnectivityService {
  // التحقق من حالة الاتصال
  Future<bool> checkConnection()
  
  // جلب نوع الاتصال
  Future<ConnectivityResult> getConnectionType()
  
  // قياس جودة الاتصال
  Future<ConnectionQuality> checkConnectionQuality()
  
  // Stream لحالة الاتصال
  Stream<bool> get connectionStatus
}
```

#### 3. DataSyncService
```dart
class DataSyncService {
  // جلب أنواع العقارات مع دعم الحفظ المحلي
  Future<List<PropertyTypeModel>> getPropertyTypes()
  
  // جلب أنواع الوحدات حسب نوع العقار
  Future<List<UnitTypeModel>> getUnitTypes({required String propertyTypeId})
  
  // جلب الحقول الديناميكية القابلة للفلترة
  Future<List<UnitTypeFieldModel>> getFilterableFieldsByUnitType(String unitTypeId)
  
  // مزامنة جميع البيانات من الباك اند
  Future<bool> syncAllData()
  
  // مزامنة البيانات عند فتح التطبيق
  Future<void> syncOnAppStart()
}
```

### التدفق العملي

#### 1. عند فتح التطبيق
```dart
// في main.dart
await ConnectivityService().initialize();

// في HomePage
WidgetsBinding.instance.addPostFrameCallback((_) {
  final dataSyncService = sl<DataSyncService>();
  dataSyncService.syncOnAppStart();
});
```

#### 2. عند جلب البيانات
```dart
// في HomeBloc
List<dynamic> propertyTypes = [];
try {
  final propertyTypesResult = await getPropertyTypesUseCase(NoParams());
  propertyTypes = propertyTypesResult.fold(
    (l) => <dynamic>[],
    (r) => r,
  );
} catch (e) {
  // Fallback to local data if remote fails
  try {
    final localPropertyTypes = await dataSyncService.getPropertyTypes();
    propertyTypes = localPropertyTypes;
  } catch (localError) {
    print('Error loading property types from both remote and local: $localError');
    propertyTypes = <dynamic>[];
  }
}
```

#### 3. في صفحة الفلترة
```dart
// تحميل البيانات المحفوظة محلياً
void _loadLocalData() async {
  setState(() {
    _isLoadingData = true;
  });
  
  try {
    // جلب أنواع العقارات المحفوظة محلياً
    final propertyTypes = await _dataSyncService.getPropertyTypes();
    setState(() {
      _propertyTypes = propertyTypes;
    });
    
    // إذا كان هناك نوع عقار محدد، جلب أنواع الوحدات التابعة له
    if (_filters['propertyTypeId'] != null) {
      await _loadUnitTypesForPropertyType(_filters['propertyTypeId']);
    }
    
    // إذا كان هناك نوع وحدة محدد، جلب الحقول الديناميكية
    if (_filters['unitTypeId'] != null) {
      await _loadDynamicFieldsForUnitType(_filters['unitTypeId']);
    }
  } catch (e) {
    print('Error loading local data: $e');
  } finally {
    setState(() {
      _isLoadingData = false;
    });
  }
}
```

## الاستخدام في الواجهات

### 1. صفحة الفلترة
- **تحديد نوع العقار**: يعرض أنواع الوحدات التابعة له
- **تحديد نوع الوحدة**: يعرض الحقول الديناميكية القابلة للفلترة
- **الحقول الديناميكية**: واجهة متقدمة لإدخال قيم الفلترة

### 2. عرض الفلاتر
- **FilterChipsWidget**: عرض جميع الفلاتر المحددة
- **دعم الحقول الديناميكية**: عرض عدد الحقول الديناميكية المحددة
- **ألوان مميزة**: كل نوع فلتر له لون مميز

### 3. الصفحة الرئيسية
- **مزامنة تلقائية**: تحديث البيانات عند فتح التطبيق
- **استخدام البيانات المحفوظة**: في حالة عدم وجود اتصال

## إدارة البيانات

### صلاحية البيانات
- **مدة الصلاحية**: 24 ساعة
- **التحديث التلقائي**: عند توفر الاتصال
- **النسخ الاحتياطية**: حفظ البيانات في SharedPreferences

### إحصائيات البيانات
```dart
Map<String, dynamic> getDataStats() {
  return {
    'propertyTypesCount': propertyTypes.length,
    'unitTypesCount': unitTypes.length,
    'dynamicFieldsCount': dynamicFields.length,
    'lastSyncTime': lastSync?.toIso8601String(),
    'dataVersion': dataVersion,
    'isDataValid': isDataValid(),
    'hasCachedData': hasCachedData(),
  };
}
```

## التبعيات المطلوبة

```yaml
dependencies:
  connectivity_plus: ^5.0.2
  shared_preferences: ^2.2.2
  get_it: ^7.6.7
  flutter_bloc: ^8.1.6
```

## التثبيت والإعداد

### 1. إضافة التبعيات
```bash
flutter pub add connectivity_plus
```

### 2. تهيئة الخدمات
```dart
// في main.dart
await ConnectivityService().initialize();

// في injection_container.dart
sl.registerLazySingleton(() => LocalDataService(sl()));
sl.registerLazySingleton(() => ConnectivityService());
sl.registerLazySingleton(() => DataSyncService(
  localDataService: sl(),
  connectivityService: sl(),
  remoteDataSource: sl(),
));
```

### 3. استخدام الخدمات
```dart
// في HomeBloc
final dataSyncService = sl<DataSyncService>();

// في SearchFiltersPage
final _dataSyncService = sl<DataSyncService>();
```

## المزايا

### 1. الأداء
- **سرعة الاستجابة**: البيانات المحفوظة محلياً
- **تقليل استهلاك البيانات**: تحديث ذكي للبيانات
- **تجربة مستخدم سلسة**: عمل التطبيق بدون انقطاع

### 2. الموثوقية
- **عمل بدون اتصال**: استخدام البيانات المحفوظة
- **نسخ احتياطية**: حفظ البيانات في عدة أماكن
- **استرداد البيانات**: إعادة تحميل البيانات عند الحاجة

### 3. المرونة
- **تحديث ديناميكي**: تحديث البيانات حسب الحاجة
- **فلترة متقدمة**: دعم الحقول الديناميكية
- **واجهة متجاوبة**: عرض البيانات بشكل مناسب

## استكشاف الأخطاء

### مشاكل شائعة

#### 1. عدم تحميل البيانات المحفوظة
```dart
// التحقق من وجود بيانات محفوظة
bool hasCachedData = _localDataService.hasCachedData();
if (!hasCachedData) {
  // إعادة مزامنة البيانات
  await _dataSyncService.syncAllData();
}
```

#### 2. مشاكل الاتصال
```dart
// التحقق من حالة الاتصال
bool isConnected = await _connectivityService.checkConnection();
if (!isConnected) {
  // استخدام البيانات المحفوظة
  final localData = _localDataService.getPropertyTypes();
}
```

#### 3. مشاكل في الفلترة
```dart
// التحقق من صحة البيانات
bool isDataValid = _localDataService.isDataValid();
if (!isDataValid) {
  // إعادة تحميل البيانات
  await _dataSyncService.syncAllData();
}
```

## التطوير المستقبلي

### 1. تحسينات مقترحة
- **مزامنة خلفية**: تحديث البيانات في الخلفية
- **ضغط البيانات**: تقليل حجم البيانات المحفوظة
- **تشفير البيانات**: حماية البيانات المحفوظة

### 2. ميزات إضافية
- **إحصائيات مفصلة**: تتبع استخدام البيانات
- **إدارة الذاكرة**: تنظيف البيانات القديمة
- **مزامنة متقدمة**: مزامنة جزئية للبيانات

## الخلاصة

تم تطوير نظام متكامل ومتقدم لحفظ البيانات محلياً في تطبيق Yemen Booking، مما يضمن:

1. **عمل التطبيق بدون انقطاع** حتى في حالة عدم وجود اتصال
2. **تجربة مستخدم سلسة** مع واجهات متقدمة وجميلة
3. **إدارة ذكية للبيانات** مع مزامنة تلقائية
4. **دعم الحقول الديناميكية** للفلترة المتقدمة
5. **أداء عالي** مع استجابة سريعة

النظام جاهز للاستخدام ويمكن تطويره وإضافة ميزات جديدة حسب الحاجة.