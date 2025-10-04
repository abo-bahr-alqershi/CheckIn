# نظام الفهرسة المتقدم - مخططات التدفق التفصيلية

## 1. مخطط إنشاء الفهارس الأساسية

```mermaid
graph TD
    A[IndexGenerationService.GenerateAllIndexesAsync] --> B[إنشاء مهام متوازية]
    
    %% مهمة فهارس المدن
    B --> C1[GenerateCityIndexesAsync]
    C1 --> D1[جلب جميع المدن الفريدة من قاعدة البيانات]
    D1 --> E1[لكل مدينة: جلب جميع الكيانات النشطة]
    E1 --> F1[حساب الإحصائيات الأساسية]
    F1 --> G1[تجميع البيانات حسب نطاقات الأسعار]
    G1 --> H1[تجميع البيانات حسب المرافق]
    H1 --> I1[تجميع البيانات حسب أنواع الكيانات]
    I1 --> J1[إنشاء فهرس التوفر للأشهر القادمة 12]
    J1 --> K1[إنشاء CityIndex object]
    K1 --> L1[ضغط البيانات مع CompressionService]
    L1 --> M1[حفظ في ملف cities/{cityName}.json]
    
    %% مهمة فهارس نطاقات الأسعار
    B --> C2[GeneratePriceRangeIndexesAsync]
    C2 --> D2[تحديد نطاقات الأسعار المطلوبة]
    D2 --> E2[لكل نطاق: جلب الكيانات المطابقة]
    E2 --> F2[حساب إحصائيات النطاق]
    F2 --> G2[إنشاء PriceRangeIndex object]
    G2 --> H2[ضغط وحفظ في price-ranges/{range}.json]
    
    %% مهمة فهارس المرافق
    B --> C3[GenerateAmenityIndexesAsync]
    C3 --> D3[جلب جميع المرافق النشطة]
    D3 --> E3[لكل مرفق: جلب الكيانات التي تحتويه]
    E3 --> F3[حساب إحصائيات المرفق]
    F3 --> G3[إنشاء AmenityIndex object]
    G3 --> H3[ضغط وحفظ في amenities/{amenityId}.json]
    
    %% مهمة فهارس أنواع الكيانات
    B --> C4[GeneratePropertyTypeIndexesAsync]
    C4 --> D4[جلب جميع أنواع الكيانات]
    D4 --> E4[لكل نوع: جلب الكيانات المرتبطة]
    E4 --> F4[حساب إحصائيات النوع]
    F4 --> G4[إنشاء PropertyTypeIndex object]
    G4 --> H4[ضغط وحفظ في property-types/{typeId}.json]
    
    %% مهمة فهارس التوفر
    B --> C5[GenerateAvailabilityIndexesAsync]
    C5 --> D5[تجميع بيانات التوفر حسب التاريخ]
    D5 --> E5[حساب إحصائيات الإشغال]
    E5 --> F5[إنشاء AvailabilityIndex object]
    F5 --> G5[ضغط وحفظ في availability/{date}.json]
    
    %% مهمة فهارس البحث النصي
    B --> C6[GenerateTextSearchIndexAsync]
    C6 --> D6[بناء هيكل Trie للبحث النصي]
    D6 --> E6[إضافة أسماء الكيانات والأوصاف]
    E6 --> F6[إضافة أسماء المدن والمناطق]
    F6 --> G6[تحسين هيكل Trie]
    G6 --> H6[ضغط وحفظ في text-search/trie.json]
    
    %% مهمة فهارس الحقول الديناميكية
    B --> C7[DynamicFieldIndexingService.GenerateDynamicFieldIndexesAsync]
    C7 --> D7[جلب جميع UnitTypeFields النشطة]
    D7 --> E7[لكل حقل: معالجة القيم الديناميكية]
    
    %% انتظار اكتمال جميع المهام
    M1 --> Z[انتظار اكتمال جميع المهام]
    H2 --> Z
    H3 --> Z
    H4 --> Z
    G5 --> Z
    H6 --> Z
    E7 --> Z
    
    Z --> AA[تسجيل نجاح العملية]
    AA --> BB[إرسال إشعار انتهاء الفهرسة]
    
    %% الألوان والتصنيف
    classDef process fill:#e3f2fd
    classDef database fill:#e8f5e8
    classDef file fill:#fff3e0
    classDef success fill:#e8f5e8
    
    class C1,C2,C3,C4,C5,C6,C7 process
    class D1,E1,D2,E2,D3,E3,D4,E4,D5,D7 database
    class M1,H2,H3,H4,G5,H6 file
    class Z,AA,BB success
```

## 2. مخطط تفصيلي لفهرسة الحقول الديناميكية

```mermaid
graph TD
    A[DynamicFieldIndexingService] --> B[GenerateDynamicFieldIndexesAsync]
    B --> C{هل فهرسة الحقول الديناميكية مفعلة؟}
    C -->|لا| D[تسجيل: الفهرسة معطلة]
    C -->|نعم| E[جلب جميع UnitTypeFields النشطة]
    
    E --> F[لكل حقل ديناميكي]
    F --> G[ProcessDynamicFieldAsync]
    
    G --> H[جلب جميع UnitFieldValues للحقل]
    H --> I[فلترة القيم: نشطة وغير محذوفة]
    I --> J{هل توجد قيم؟}
    J -->|لا| K[تسجيل: لا توجد قيم للحقل]
    J -->|نعم| L[إنشاء DynamicFieldIndex object]
    
    L --> M[تجميع القيم حسب FieldValue]
    M --> N[لكل قيمة: جمع معرفات الكيانات]
    N --> O{هل الحقل رقمي؟}
    O -->|نعم| P[إنشاء نطاقات رقمية]
    O -->|لا| Q[تخطي النطاقات الرقمية]
    
    P --> R[حساب البيانات الوصفية]
    Q --> R
    R --> S[حساب القيم الشائعة أكثر 10]
    S --> T[حساب Min/Max للقيم الرقمية]
    T --> U[ضغط البيانات مع GZIP]
    U --> V[حفظ في dynamic-fields/{fieldId}.json]
    
    V --> W{هل الفهارس الفرعية مفعلة؟}
    W -->|نعم| X[GenerateSecondaryIndexesAsync]
    W -->|لا| Y[تخطي الفهارس الفرعية]
    
    X --> Z[جلب جميع PropertyTypes]
    Z --> AA[لكل نوع كيان]
    AA --> BB[جلب الحقول المتوفرة للنوع]
    BB --> CC[إنشاء DynamicFieldSecondaryIndex]
    CC --> DD[تحميل فهارس الحقول ذات الصلة]
    DD --> EE[ضغط وحفظ في secondary-dynamic/{typeId}.json]
    
    EE --> FF[انتهاء معالجة الحقول الديناميكية]
    Y --> FF
    K --> FF
    
    classDef process fill:#e3f2fd
    classDef decision fill:#fff3e0
    classDef database fill:#e8f5e8
    classDef file fill:#fce4ec
    
    class B,G,M,N,R,S,T,X,AA,BB,CC,DD process
    class C,J,O,W decision
    class E,H,I,Z database
    class V,EE file
```

## 3. مخطط عملية البحث والفلترة

```mermaid
graph TD
    A[عميل يرسل طلب بحث] --> B[FastSearchService.SearchAsync]
    B --> C[التحقق من صحة معايير البحث]
    C --> D{معايير صحيحة؟}
    D -->|لا| E[إرجاع خطأ التحقق]
    D -->|نعم| F[إنشاء مفتاح كاش للبحث]
    
    F --> G{هل النتائج في الكاش؟}
    G -->|نعم| H[استرجاع من Memory Cache]
    G -->|لا| I[بدء عملية البحث الجديدة]
    
    I --> J[تحليل أنواع الفلاتر المطلوبة]
    J --> K{نوع البحث}
    
    %% البحث النصي
    K -->|بحث نصي| L1[قراءة فهرس Trie من القرص]
    L1 --> M1[فك ضغط GZIP]
    M1 --> N1[البحث في هيكل Trie]
    N1 --> O1[استخراج معرفات الكيانات المطابقة]
    
    %% البحث الجغرافي
    K -->|بحث جغرافي| L2[قراءة الفهرس الجغرافي]
    L2 --> M2[حساب المسافات باستخدام Haversine]
    M2 --> N2[فلترة النتائج حسب نصف القطر]
    N2 --> O2[ترتيب حسب المسافة]
    
    %% الفلاتر التقليدية
    K -->|فلاتر تقليدية| L3[تحديد الفهارس المطلوبة]
    L3 --> M3[قراءة متوازية للفهارس]
    
    M3 --> N3[قراءة فهرس المدينة]
    M3 --> N4[قراءة فهرس نطاق السعر]
    M3 --> N5[قراءة فهرس المرافق]
    
    N3 --> P3[فك ضغط وتحليل JSON]
    N4 --> P4[فك ضغط وتحليل JSON]
    N5 --> P5[فك ضغط وتحليل JSON]
    
    P3 --> Q[حساب تقاطع معرفات الكيانات]
    P4 --> Q
    P5 --> Q
    
    %% الحقول الديناميكية
    K -->|حقول ديناميكية| L4[تحديد الحقول المطلوبة]
    L4 --> M4[قراءة فهارس الحقول الديناميكية]
    M4 --> N6[DynamicFieldIndexingService.FilterByDynamicFieldAsync]
    N6 --> O6[استخراج معرفات الكيانات للقيم المطلوبة]
    
    O1 --> R[دمج نتائج جميع أنواع البحث]
    O2 --> R
    Q --> R
    O6 --> R
    
    R --> S[تطبيق ترتيب النتائج]
    S --> T[تطبيق Pagination]
    T --> U{هل تفاصيل الكيانات في Hot Cache؟}
    
    U -->|نعم| V[استرجاع من JsonIndexFileService Hot Cache]
    U -->|لا| W[LoadSinglePropertySummaryAsync]
    
    W --> X[جلب تفاصيل الكيانات من قاعدة البيانات]
    X --> Y[تحويل إلى PropertySummary]
    Y --> Z[حفظ في Hot Cache]
    
    V --> AA[تجميع النتائج النهائية]
    Z --> AA
    AA --> BB[إنشاء SearchResult object]
    BB --> CC[حفظ النتائج في Memory Cache]
    CC --> DD[إرجاع النتائج للعميل]
    
    H --> DD
    
    classDef client fill:#e1f5fe
    classDef service fill:#e3f2fd
    classDef cache fill:#fff3e0
    classDef file fill:#fce4ec
    classDef database fill:#e8f5e8
    
    class A,DD client
    class B,C,I,J,N1,N2,Q,R,S,T,Y service
    class G,H,U,V,CC cache
    class L1,L2,M3,N3,N4,N5,M4 file
    class W,X database
```

## 4. مخطط إدارة الكاش ودورة الحياة

```mermaid
graph TD
    A[نظام إدارة الكاش] --> B[Hot Cache في JsonIndexFileService]
    A --> C[Memory Cache في FastSearchService]
    
    %% Hot Cache Management
    B --> D[قراءة فهرس من القرص]
    D --> E{هل الحجم مناسب للكاش؟}
    E -->|نعم| F[إضافة إلى Hot Cache]
    E -->|لا| G[عدم إضافة للكاش]
    
    F --> H[تحديث وقت آخر وصول]
    H --> I{هل الكاش ممتلئ؟}
    I -->|نعم| J[تشغيل CleanupHotCacheAsync]
    I -->|لا| K[الاحتفاظ بالعنصر]
    
    J --> L[فرز العناصر حسب آخر وصول]
    L --> M[حذف 25% من الأقل استخداماً]
    M --> N[تحديث إحصائيات الكاش]
    N --> K
    
    %% Memory Cache Management
    C --> O[حفظ نتائج البحث في Memory Cache]
    O --> P[تعيين مدة انتهاء الصلاحية]
    P --> Q[مراقبة استهلاك الذاكرة]
    Q --> R{تجاوز الحد الأقصى؟}
    R -->|نعم| S[تنظيف تلقائي بواسطة .NET]
    R -->|لا| T[الاحتفاظ بالبيانات]
    
    %% Cache Invalidation
    K --> U[تحديث الفهارس]
    T --> U
    U --> V{نوع التحديث}
    V -->|فهرس محدد| W[إزالة العناصر المرتبطة من الكاش]
    V -->|إعادة بناء كاملة| X[مسح جميع الكاش]
    
    W --> Y[تحديث الفهرس الجديد]
    X --> Z[إعادة بناء الفهارس]
    Y --> AA[الكاش محدث وجاهز]
    Z --> AA
    
    %% Monitoring
    AA --> BB[مراقبة أداء الكاش]
    BB --> CC[تسجيل إحصائيات Hit/Miss]
    CC --> DD[تحليل أنماط الاستخدام]
    DD --> EE[تحسين إعدادات الكاش]
    EE --> B
    
    classDef cache fill:#fff3e0
    classDef process fill:#e3f2fd
    classDef monitoring fill:#e8f5e8
    classDef decision fill:#ffebee
    
    class B,C,F,H,O,P cache
    class D,J,L,M,N,Y,Z process
    class BB,CC,DD,EE monitoring
    class E,I,R,V decision
```

## 5. مخطط التحديث التلقائي للفهارس

```mermaid
graph TD
    A[تعديل كيان في قاعدة البيانات] --> B[Domain Event Handler]
    B --> C{نوع الحدث}
    
    C -->|PropertyCreated| D1[PropertyCreatedEventHandler]
    C -->|PropertyUpdated| D2[PropertyUpdatedEventHandler]
    C -->|PropertyDeleted| D3[PropertyDeletedEventHandler]
    C -->|UnitFieldValueChanged| D4[UnitFieldValueChangedEventHandler]
    
    %% معالجة إنشاء كيان جديد
    D1 --> E1[IndexAutoUpdateService.HandlePropertyCreatedAsync]
    E1 --> F1[تحديد الفهارس المتأثرة]
    F1 --> G1[تحديث فهرس المدينة]
    G1 --> H1[تحديث فهرس نطاق السعر]
    H1 --> I1[تحديث فهارس المرافق]
    I1 --> J1[تحديث الفهرس النصي]
    
    %% معالجة تعديل كيان
    D2 --> E2[HandlePropertyUpdatedAsync]
    E2 --> F2[مقارنة القيم القديمة والجديدة]
    F2 --> G2{هل المدينة تغيرت؟}
    G2 -->|نعم| H2[تحديث فهارس المدينة القديمة والجديدة]
    G2 -->|لا| I2[تحديث فهرس المدينة الحالية فقط]
    
    H2 --> J2[تحديث باقي الفهارس المتأثرة]
    I2 --> J2
    
    %% معالجة حذف كيان
    D3 --> E3[HandlePropertyDeletedAsync]
    E3 --> F3[إزالة من جميع الفهارس]
    F3 --> G3[تحديث الإحصائيات]
    G3 --> H3[إعادة حساب البيانات الوصفية]
    
    %% معالجة تغيير الحقول الديناميكية
    D4 --> E4[HandleUnitFieldValueChangedAsync]
    E4 --> F4[DynamicFieldIndexingService.UpdateDynamicFieldIndexAsync]
    F4 --> G4[قراءة الفهرس الحالي]
    G4 --> H4[إزالة القيمة القديمة]
    H4 --> I4[إضافة القيمة الجديدة]
    I4 --> J4[تحديث البيانات الوصفية]
    J4 --> K4[ضغط وحفظ الفهرس المحدث]
    
    %% تجميع التحديثات
    J1 --> L[RetryIndexUpdateAsync]
    J2 --> L
    H3 --> L
    K4 --> L
    
    L --> M{هل التحديث نجح؟}
    M -->|نعم| N[تسجيل نجاح التحديث]
    M -->|لا| O[إعادة المحاولة مع تأخير]
    
    O --> P{عدد المحاولات < الحد الأقصى؟}
    P -->|نعم| Q[انتظار وإعادة المحاولة]
    P -->|لا| R[تسجيل فشل التحديث]
    
    Q --> L
    R --> S[إضافة للقائمة المؤجلة]
    N --> T[إشعار اكتمال التحديث]
    S --> U[معالجة لاحقة في الخلفية]
    
    T --> V[تحديث الكاش]
    U --> V
    V --> W[النظام محدث وجاهز]
    
    classDef event fill:#e1f5fe
    classDef handler fill:#e3f2fd
    classDef update fill:#e8f5e8
    classDef retry fill:#fff3e0
    classDef success fill:#c8e6c9
    classDef error fill:#ffcdd2
    
    class A,B,C event
    class D1,D2,D3,D4,E1,E2,E3,E4 handler
    class F1,G1,H1,I1,J1,F2,G2,H2,I2,J2,F3,G3,H3,F4,G4,H4,I4,J4,K4 update
    class L,O,P,Q retry
    class N,T,V,W success
    class R,S,U error
```

هذه المخططات تعكس بدقة نظام الفهرسة المتقدم الذي قمنا بتطويره، وتوضح:

1. **التدفق الكامل** من إنشاء البيانات إلى إرجاع نتائج البحث
2. **معالجة الفهارس الديناميكية** بالتفصيل
3. **إدارة الكاش** متعدد المستويات
4. **التحديث التلقائي** للفهارس عند تغيير البيانات
5. **معالجة الأخطاء** وإعادة المحاولة

كل مخطط يتضمن التفاصيل الفنية الدقيقة لنظامنا، بما في ذلك ضغط GZIP، والكاش الساخن، والفهارس الفرعية، والحقول الديناميكية.