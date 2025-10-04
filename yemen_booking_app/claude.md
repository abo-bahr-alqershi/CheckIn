أريد تطوير نظام ديناميكي جميل جدا ورائع وهادئ لعرض المحتوى في الشاشة الرئيسية لتطبيق حجوزات الفنادق باستخدام Flutter و BLoC.

المتطلبات الأساسية:
- إنشاء 20 نوع widget مختلف بتصاميم احترافية وحديثة
- نظام Factory Pattern لإنشاء العناصر ديناميكيًا
- Placeholders مع shimmer effect لكل نوع
- دعم RTL/LTR والتصميم المتجاوب
- نظام caching ذكي للصور والبيانات

أنواع العناصر المطلوبة:
1. إعلانات ممولة (4 أنواع):
   - إعلان فردي للعقار مع تأثيرات parallax
   - إعلان مميز بتصميم premium
   - إعلان متعدد العقارات بنظام grid/carousel
   - عرض الوحدة مباشرة مع معرض صور

2. عروض خاصة (6 أنواع):
   - عرض عقار واحد مع countdown timer
   - عرض محدود الوقت مع animation
   - عروض موسمية بتصميم thematic
   - شبكة عروض متعددة
   - carousel للعروض مع auto-play
   - عروض flash deals

3. قوائم العقارات (5 أنواع):
   - قائمة أفقية مع lazy loading
   - شبكة عمودية responsive
   - تخطيط مختلط (بطاقات كبيرة وصغيرة)
   - قائمة مضغوطة للمساحات الصغيرة
   - عرض العقارات المميزة

4. عرض المدن (3 أنواع):
   - بطاقات مدن مع صور خلفية
   - carousel تفاعلي للوجهات
   - widget استكشاف المدن

5. Carousels مبتكرة (2 نوع):
   - Premium carousel مع 3D effect
   - Interactive showcase مع gestures

المواصفات التقنية:
- استخدام CustomPainter للتصاميم المعقدة
- Animations مع Flutter's animation framework
- Hero animations للانتقالات
- Responsive design مع LayoutBuilder
- Performance optimization مع const constructors
- Error handling مع fallback widgets

هيكل البيانات:
```json
{
  "sections": [
    {
      "id": "uuid",
      "type": "SINGLE_PROPERTY_AD",
      "order": 1,
      "isActive": true,
      "config": {
        "propertyIds": ["uuid1"],
        "title": "عرض خاص",
        "backgroundColor": "#FF5733",
        "customImage": "url"
      }
    }
  ]
}

أريد كود نظيف وقابل للصيانة مع تعليقات واضحة ودعم كامل للتخصيص من لوحة التحكم.

ولاكن اولا ابدأ بانشاء الملفات التالية حسب مخطط المعمارية
│   │   │   ├── models/
│   │   │   │   ├── property_model.dart
│   │   │   │   ├── featured_property_model.dart
│   │   │   │   ├── home_section_model.dart
│   │   │   │   ├── home_config_model.dart
│   │   │   │   ├── section_data_model.dart
│   │   │   │   ├── sponsored_ad_model.dart
│   │   │   │   ├── special_offer_model.dart
│   │   │   │   ├── city_destination_model.dart
│   │   │   │   └── section_config_model.dart

│   │   │   ├── entities/
│   │   │   │   ├── featured_property.dart
│   │   │   │   ├── home_section.dart
│   │   │   │   ├── home_config.dart
│   │   │   │   ├── section_type.dart
│   │   │   │   ├── sponsored_ad.dart
│   │   │   │   ├── special_offer.dart
│   │   │   │   ├── city_destination.dart
│   │   │   │   └── section_config.dart
واذا احتجت لقراءة اي ملف من الملفات في بافي طبقات المشروع قم بقرائتها حتى تستطيع ضمان التوافق التام مع الطبقات الاخرى والكود النظيف الغير مكرر (في حال كانت الملفات المطلوبة منك تحتاج معرفة جيدة بالملفات الاخرى في المشروع)

وهذا هو كامل المخطط 
lib/
├── main.dart
├── app.dart
├── injection_container.dart  # Dependency Injection
│
├── core/
│   ├── constants/
│   │   ├── api_constants.dart
│   │   ├── app_constants.dart
│   │   ├── storage_constants.dart
│   │   ├── route_constants.dart
│   │   ├── home_constants.dart
│   │   ├── section_constants.dart
│   │   └── animation_constants.dart
│   │
│   ├── theme/
│   │   ├── app_theme.dart
│   │   ├── app_colors.dart
│   │   ├── app_text_styles.dart
│   │   ├── app_dimensions.dart
│   │   │
│   │   └── section_themes/
│   │       ├── sponsored_section_theme.dart
│   │       ├── offer_section_theme.dart
│   │       ├── listing_section_theme.dart
│   │       └── carousel_section_theme.dart
│   │
│   ├── localization/
│   │   ├── app_localizations.dart
│   │   ├── l10n/
│   │   │   ├── app_ar.arb
│   │   │   └── app_en.arb
│   │   └── locale_manager.dart
│   │
│   ├── network/
│   │   ├── api_client.dart
│   │   ├── api_interceptors.dart
│   │   ├── api_exceptions.dart
│   │   └── network_info.dart
│   │
│   ├── error/
│   │   ├── failures.dart
│   │   ├── exceptions.dart
│   │   └── error_handler.dart
│   │
│   ├── utils/
│   │   ├── validators.dart
│   │   ├── formatters.dart
│   │   ├── date_utils.dart
│   │   ├── price_calculator.dart
│   │   └── location_utils.dart
│   │
│   ├── models/
│   │   ├── home_section_base_model.dart
│   │   ├── dynamic_content_model.dart
│   │   ├── paginated_result.dart
│   │   └── result_dto.dart
│   │
│   ├── enums/
│   │   ├── section_type_enum.dart
│   │   ├── section_size_enum.dart
│   │   └── section_animation_enum.dart
│   │   
│   ├── widgets/
│   │   ├── app_bar_widget.dart
│   │   ├── loading_widget.dart
│   │   ├── error_widget.dart
│   │   ├── empty_widget.dart
│   │   ├── cached_image_widget.dart
│   │   ├── rating_widget.dart
│   │   └── price_widget.dart
│   │
│   └── mixins/
│       ├── section_analytics_mixin.dart
│       ├── section_caching_mixin.dart
│       └── section_error_handling_mixin.dart
│
├── features/
│   ├── auth/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── auth_local_datasource.dart
│   │   │   │   └── auth_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── user_model.dart
│   │   │   │   └── auth_response_model.dart
│   │   │   └── repositories/
│   │   │       └── auth_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── user.dart
│   │   │   │   └── auth_response.dart
│   │   │   ├── repositories/
│   │   │   │   └── auth_repository.dart
│   │   │   └── usecases/
│   │   │       ├── login_usecase.dart
│   │   │       ├── register_usecase.dart
│   │   │       ├── logout_usecase.dart
│   │   │       ├── reset_password_usecase.dart
│   │   │       ├── update_profile_usecase.dart
│   │   │       ├── upload_user_image_usecase.dart
│   │   │       ├── check_auth_status_usecase.dart # Added
│   │   │       └── get_current_user_usecase.dart   # Added
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── auth_bloc.dart
│   │       │   ├── auth_event.dart
│   │       │   └── auth_state.dart
│   │       ├── pages/
│   │       │   ├── login_page.dart
│   │       │   ├── register_page.dart
│   │       │   ├── forgot_password_page.dart
│   │       │   └── profile_page.dart
│   │       └── widgets/
│   │           ├── login_form.dart
│   │           ├── register_form.dart
│   │           └── social_login_buttons.dart
│   │
│   ├── home/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── home_local_datasource.dart
│   │   │   │   └── home_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── property_model.dart
│   │   │   │   ├── featured_property_model.dart
│   │   │   │   ├── home_section_model.dart
│   │   │   │   ├── home_config_model.dart
│   │   │   │   ├── section_data_model.dart
│   │   │   │   ├── sponsored_ad_model.dart
│   │   │   │   ├── special_offer_model.dart
│   │   │   │   ├── city_destination_model.dart
│   │   │   │   └── section_config_model.dart
│   │   │   └── repositories/
│   │   │       └── home_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── featured_property.dart
│   │   │   │   ├── home_section.dart
│   │   │   │   ├── home_config.dart
│   │   │   │   ├── section_type.dart
│   │   │   │   ├── sponsored_ad.dart
│   │   │   │   ├── special_offer.dart
│   │   │   │   ├── city_destination.dart
│   │   │   │   └── section_config.dart
│   │   │   ├── repositories/
│   │   │   │   └── home_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_featured_properties_usecase.dart
│   │   │       ├── get_nearby_properties_usecase.dart
│   │   │       ├── get_popular_destinations_usecase.dart
│   │   │       ├── get_home_config_usecase.dart
│   │   │       ├── get_section_data_usecase.dart
│   │   │       ├── refresh_home_sections_usecase.dart
│   │   │       ├── track_section_impression_usecase.dart
│   │   │       └── track_section_interaction_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── home_bloc.dart
│   │       │   ├── home_event.dart
│   │       │   ├── home_state.dart
│   │       │   ├── section_bloc/
│   │       │   │   ├── section_bloc.dart
│   │       │   │   ├── section_event.dart
│   │       │   │   └── section_state.dart
│   │       │   └── analytics_bloc/
│   │       │       ├── home_analytics_bloc.dart
│   │       │       ├── home_analytics_event.dart
│   │       │       └── home_analytics_state.dart
│   │       ├── pages/
│   │       │   ├── home_page.dart
│   │       │   └── section_detail_page.dart
│   │       └── widgets/
│   │           ├── search_bar_widget.dart
│   │           ├── featured_properties_widget.dart
│   │           ├── property_categories_widget.dart
│   │           ├── popular_destinations_widget.dart
│   │           ├── section_factory.dart
│   │           ├── section_builder_widget.dart
│   │           ├── home_sections_list_widget.dart
│   │           ├── sections/
│   │           │   ├── base/
│   │           │   │   ├── base_section_widget.dart
│   │           │   │   ├── section_placeholder.dart
│   │           │   │   ├── section_error_widget.dart
│   │           │   │   ├── section_container.dart
│   │           │   │   ├── section_header.dart
│   │           │   │   └── section_analytics_wrapper.dart
│   │           │   ├── sponsored/
│   │           │   │   ├── single_property_ad_widget.dart
│   │           │   │   ├── featured_property_ad_widget.dart
│   │           │   │   ├── multi_property_ad_widget.dart
│   │           │   │   ├── unit_showcase_ad_widget.dart
│   │           │   │   └── placeholders/
│   │           │   │       ├── single_property_ad_placeholder.dart
│   │           │   │       ├── featured_property_ad_placeholder.dart
│   │           │   │       ├── multi_property_ad_placeholder.dart
│   │           │   │       └── unit_showcase_ad_placeholder.dart
│   │           │   ├── offers/
│   │           │   │   ├── single_property_offer_widget.dart
│   │           │   │   ├── limited_time_offer_widget.dart
│   │           │   │   ├── seasonal_offer_widget.dart
│   │           │   │   ├── multi_property_offers_grid.dart
│   │           │   │   ├── offers_carousel_widget.dart
│   │           │   │   ├── flash_deals_widget.dart
│   │           │   │   └── placeholders/
│   │           │   │       ├── offer_placeholder.dart
│   │           │   │       ├── offers_grid_placeholder.dart
│   │           │   │       └── offers_carousel_placeholder.dart
│   │           │   ├── listings/
│   │           │   │   ├── horizontal_property_list.dart
│   │           │   │   ├── vertical_property_grid.dart
│   │           │   │   ├── mixed_layout_list.dart
│   │           │   │   ├── compact_property_list.dart
│   │           │   │   ├── featured_properties_showcase.dart
│   │           │   │   └── placeholders/
│   │           │   │       ├── horizontal_list_placeholder.dart
│   │           │   │       ├── grid_placeholder.dart
│   │           │   │       └── mixed_layout_placeholder.dart
│   │           │   ├── destinations/
│   │           │   │   ├── city_cards_grid.dart
│   │           │   │   ├── destination_carousel.dart
│   │           │   │   ├── explore_cities_widget.dart
│   │           │   │   └── placeholders/
│   │           │   │       ├── city_card_placeholder.dart
│   │           │   │       └── destination_placeholder.dart
│   │           │   ├── carousels/
│   │           │   │   ├── premium_property_carousel.dart
│   │           │   │   ├── interactive_showcase_carousel.dart
│   │           │   │   └── placeholders/
│   │           │   │       └── carousel_placeholder.dart
│   │           │   └── common/
│   │           │       ├── countdown_timer_widget.dart
│   │           │       ├── parallax_image_widget.dart
│   │           │       ├── shimmer_container.dart
│   │           │       ├── gradient_overlay_widget.dart
│   │           │       ├── animated_badge_widget.dart
│   │           │       ├── price_tag_widget.dart
│   │           │       ├── rating_badge_widget.dart
│   │           │       └── carousel_indicators_widget.dart
│   │           └── utils/
│   │               ├── section_type_mapper.dart
│   │               ├── section_config_parser.dart
│   │               ├── section_animation_controller.dart
│   │               └── section_visibility_detector.dart
│   │
│   ├── search/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── search_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── search_filter_model.dart
│   │   │   │   └── search_result_model.dart
│   │   │   └── repositories/
│   │   │       └── search_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── search_filter.dart
│   │   │   │   └── search_result.dart
│   │   │   ├── repositories/
│   │   │   │   └── search_repository.dart
│   │   │   └── usecases/
│   │   │       ├── search_properties_usecase.dart
│   │   │       ├── get_search_filters_usecase.dart
│   │   │       └── get_search_suggestions_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── search_bloc.dart
│   │       │   ├── search_event.dart
│   │       │   └── search_state.dart
│   │       ├── pages/
│   │       │   ├── search_page.dart
│   │       │   ├── search_filters_page.dart
│   │       │   └── search_results_page.dart
│   │       └── widgets/
│   │           ├── search_input_widget.dart
│   │           ├── filter_chips_widget.dart
│   │           ├── sort_options_widget.dart
│   │           ├── price_range_slider_widget.dart
│   │           └── search_result_card_widget.dart
│   │
│   ├── property/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── property_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── property_detail_model.dart
│   │   │   │   ├── unit_model.dart
│   │   │   │   ├── amenity_model.dart
│   │   │   │   └── review_model.dart
│   │   │   └── repositories/
│   │   │       └── property_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── property_detail.dart
│   │   │   │   ├── unit.dart
│   │   │   │   ├── amenity.dart
│   │   │   │   └── property_policy.dart
│   │   │   ├── repositories/
│   │   │   │   └── property_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_property_details_usecase.dart
│   │   │       ├── get_property_units_usecase.dart
│   │   │       ├── get_property_reviews_usecase.dart
│   │   │       └── add_to_favorites_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── property_bloc.dart
│   │       │   ├── property_event.dart
│   │       │   └── property_state.dart
│   │       ├── pages/
│   │       │   ├── property_details_page.dart
│   │       │   ├── property_gallery_page.dart
│   │       │   ├── property_reviews_page.dart
│   │       │   ├── property_units_page.dart
│   │       │   └── property_map_page.dart
│   │       └── widgets/
│   │           ├── property_header_widget.dart
│   │           ├── property_images_slider_widget.dart
│   │           ├── property_info_widget.dart
│   │           ├── amenities_grid_widget.dart
│   │           ├── units_list_widget.dart
│   │           ├── reviews_summary_widget.dart
│   │           ├── policies_widget.dart
│   │           └── location_map_widget.dart
│   │
│   ├── booking/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── booking_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── booking_model.dart
│   │   │   │   ├── booking_request_model.dart
│   │   │   │   └── payment_model.dart
│   │   │   └── repositories/
│   │   │       └── booking_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── booking.dart
│   │   │   │   ├── booking_request.dart
│   │   │   │   └── payment.dart
│   │   │   ├── repositories/
│   │   │   │   └── booking_repository.dart
│   │   │   └── usecases/
│   │   │       ├── create_booking_usecase.dart
│   │   │       ├── get_booking_details_usecase.dart
│   │   │       ├── cancel_booking_usecase.dart
│   │   │       ├── get_user_bookings_usecase.dart
│   │   │       └── check_availability_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── booking_bloc.dart
│   │       │   ├── booking_event.dart
│   │       │   └── booking_state.dart
│   │       ├── pages/
│   │       │   ├── booking_form_page.dart
│   │       │   ├── booking_summary_page.dart
│   │       │   ├── booking_payment_page.dart
│   │       │   ├── booking_confirmation_page.dart
│   │       │   ├── my_bookings_page.dart
│   │       │   └── booking_details_page.dart
│   │       └── widgets/
│   │           ├── date_picker_widget.dart
│   │           ├── guest_selector_widget.dart
│   │           ├── services_selector_widget.dart
│   │           ├── price_breakdown_widget.dart
│   │           ├── booking_card_widget.dart
│   │           ├── booking_status_widget.dart
│   │           └── payment_methods_widget.dart
│   │
│   ├── payment/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── payment_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── payment_method_model.dart
│   │   │   │   └── transaction_model.dart
│   │   │   └── repositories/
│   │   │       └── payment_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── payment_method.dart
│   │   │   │   └── transaction.dart
│   │   │   ├── repositories/
│   │   │   │   └── payment_repository.dart
│   │   │   └── usecases/
│   │   │       ├── process_payment_usecase.dart
│   │   │       ├── get_payment_methods_usecase.dart
│   │   │       └── get_payment_history_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── payment_bloc.dart
│   │       │   ├── payment_event.dart
│   │       │   └── payment_state.dart
│   │       ├── pages/
│   │       │   ├── payment_methods_page.dart
│   │       │   ├── add_payment_method_page.dart
│   │       │   └── payment_history_page.dart
│   │       └── widgets/
│   │           ├── credit_card_form_widget.dart
│   │           ├── payment_method_card_widget.dart
│   │           └── transaction_item_widget.dart
│   │
│   ├── chat/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── chat_local_datasource.dart
│   │   │   │   └── chat_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── conversation_model.dart
│   │   │   │   ├── message_model.dart
│   │   │   │   └── attachment_model.dart
│   │   │   └── repositories/
│   │   │       └── chat_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── conversation.dart
│   │   │   │   ├── message.dart
│   │   │   │   └── attachment.dart
│   │   │   ├── repositories/
│   │   │   │   └── chat_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_conversations_usecase.dart
│   │   │       ├── get_messages_usecase.dart
│   │   │       ├── send_message_usecase.dart
│   │   │       ├── mark_as_read_usecase.dart
│   │   │       └── upload_attachment_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── chat_bloc.dart
│   │       │   ├── chat_event.dart
│   │       │   └── chat_state.dart
│   │       ├── pages/
│   │       │   ├── conversations_page.dart
│   │       │   ├── chat_page.dart
│   │       │   └── chat_settings_page.dart
│   │       └── widgets/
│   │           ├── conversation_item_widget.dart
│   │           ├── message_bubble_widget.dart
│   │           ├── message_input_widget.dart
│   │           ├── typing_indicator_widget.dart
│   │           ├── attachment_preview_widget.dart
│   │           └── reaction_picker_widget.dart
│   │
│   ├── notifications/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── notification_local_datasource.dart
│   │   │   │   └── notification_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── notification_model.dart
│   │   │   └── repositories/
│   │   │       └── notification_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── notification.dart
│   │   │   ├── repositories/
│   │   │   │   └── notification_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_notifications_usecase.dart
│   │   │       ├── mark_as_read_usecase.dart
│   │   │       ├── dismiss_notification_usecase.dart
│   │   │       └── update_notification_settings_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── notification_bloc.dart
│   │       │   ├── notification_event.dart
│   │       │   └── notification_state.dart
│   │       ├── pages/
│   │       │   ├── notifications_page.dart
│   │       │   └── notification_settings_page.dart
│   │       └── widgets/
│   │           ├── notification_item_widget.dart
│   │           ├── notification_badge_widget.dart
│   │           └── notification_filter_widget.dart
│   │
│   ├── favorites/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── favorites_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── favorite_property_model.dart
│   │   │   └── repositories/
│   │   │       └── favorites_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── favorite_property.dart
│   │   │   ├── repositories/
│   │   │   │   └── favorites_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_favorites_usecase.dart
│   │   │       ├── add_to_favorites_usecase.dart
│   │   │       └── remove_from_favorites_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── favorites_bloc.dart
│   │       │   ├── favorites_event.dart
│   │       │   └── favorites_state.dart
│   │       ├── pages/
│   │       │   └── favorites_page.dart
│   │       └── widgets/
│   │           ├── favorite_property_card_widget.dart
│   │           └── favorite_button_widget.dart
│   │
│   ├── review/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── review_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── review_model.dart
│   │   │   │   └── review_image_model.dart
│   │   │   └── repositories/
│   │   │       └── review_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── review.dart
│   │   │   │   └── review_image.dart
│   │   │   ├── repositories/
│   │   │   │   └── review_repository.dart
│   │   │   └── usecases/
│   │   │       ├── create_review_usecase.dart
│   │   │       ├── get_property_reviews_usecase.dart
│   │   │       └── upload_review_images_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── review_bloc.dart
│   │       │   ├── review_event.dart
│   │       │   └── review_state.dart
│   │       ├── pages/
│   │       │   ├── write_review_page.dart
│   │       │   └── reviews_list_page.dart
│   │       └── widgets/
│   │           ├── review_form_widget.dart
│   │           ├── review_card_widget.dart
│   │           ├── rating_selector_widget.dart
│   │           └── review_images_picker_widget.dart
│   │
│   └── settings/
│       ├── data/
│       │   ├── datasources/
│       │   │   └── settings_local_datasource.dart
│       │   ├── models/
│       │   │   └── app_settings_model.dart
│       │   └── repositories/
│       │       └── settings_repository_impl.dart
│       │
│       ├── domain/
│       │   ├── entities/
│       │   │   └── app_settings.dart
│       │   ├── repositories/
│       │   │   └── settings_repository.dart
│       │   └── usecases/
│       │       ├── get_settings_usecase.dart
│       │       ├── update_language_usecase.dart
│       │       └── update_theme_usecase.dart
│       │
│       └── presentation/
│           ├── bloc/
│           │   ├── settings_bloc.dart
│           │   ├── settings_event.dart
│           │   └── settings_state.dart
│           ├── pages/
│           │   ├── settings_page.dart
│           │   ├── language_settings_page.dart
│           │   ├── privacy_policy_page.dart
│           │   └── about_page.dart
│           └── widgets/
│               ├── settings_item_widget.dart
│               ├── language_selector_widget.dart
│               └── theme_selector_widget.dart
│
├── routes/
│   ├── app_router.dart
│   ├── route_guards.dart
│   └── route_animations.dart
│
├── home_cache_service.dart
│   ├── section_analytics_service.dart
│   └── dynamic_content_service.dart
│
└── services/
    ├── local_storage_service.dart
    ├── location_service.dart
    ├── notification_service.dart
    ├── deep_link_service.dart
    ├── analytics_service.dart
    ├── crash_reporting_service.dart
    └── websocket_service.dart


# ملفات إضافية مهمة

test/
├── unit/
│   ├── auth/
│   ├── booking/
│   ├── property/
│   └── payment/
├── widget/
│   ├── auth_widgets_test.dart
│   ├── booking_widgets_test.dart
│   └── property_widgets_test.dart
└── integration/
    ├── auth_flow_test.dart
    ├── booking_flow_test.dart
    └── search_flow_test.dart

assets/
├── images/
│   ├── logo.png
│   ├── splash_screen.png
│   └── placeholders/
├── icons/
│   ├── amenity_icons/
│   └── category_icons/
├── animations/
│   ├── loading.json
│   └── success.json
└── fonts/
    ├── arabic_font.ttf
    └── english_font.ttf

# ملفات التكوين

pubspec.yaml
analysis_options.yaml
.env
.env.production


وهذا ملف مخطط كيانات ال Entities في الباك اند في حال كانت ستكون مغيدة
erDiagram
    BaseEntity {
        Guid Id PK
        Guid CreatedBy FK
        DateTime CreatedAt
        Guid UpdatedBy FK
        DateTime UpdatedAt
        bool IsActive
        bool IsDeleted
        Guid DeletedBy FK
        DateTime DeletedAt
    }

    User {
        Guid Id PK
        string Name
        string Email
        string Password
        string Phone
        string ProfileImage
        DateTime CreatedAt
        bool IsActive
        DateTime LastLoginDate
        decimal TotalSpent
        string LoyaltyTier
        bool EmailConfirmed
        string EmailConfirmationToken
        DateTime EmailConfirmationTokenExpires
        string PasswordResetToken
        DateTime PasswordResetTokenExpires
        string SettingsJson
        string FavoritesJson
    }

    Role {
        Guid Id PK
        string Name
    }

    UserRole {
        Guid Id PK
        Guid UserId FK
        Guid RoleId FK
    }

    PropertyType {
        Guid Id PK
        string Name
        string Description
        string DefaultAmenities
    }

    Property {
        Guid Id PK
        Guid OwnerId FK
        Guid TypeId FK
        string Name
        string Address
        string City
        decimal Latitude
        decimal Longitude
        int StarRating
        string Description
        bool IsApproved
        DateTime CreatedAt
        int ViewCount
        int BookingCount
        decimal AverageRating
    }

    UnitType {
        Guid Id PK
        Guid PropertyTypeId FK
        string Name
        string Description
        string DefaultPricingRules
        int MaxCapacity
    }

    Unit {
        Guid Id PK
        Guid PropertyId FK
        Guid UnitTypeId FK
        string Name
        decimal BasePrice
        string Currency
        int MaxCapacity
        string CustomFeatures
        bool IsAvailable
        int ViewCount
        int BookingCount
        string PricingMethod
    }

    UnitTypeField {
        Guid Id PK
        Guid UnitTypeId FK
        string FieldKey
        string FieldName
        string FieldType
        bool IsRequired
        bool IsSearchable
        bool IsPublic
        string DefaultValue
        string ValidationRules
        string Options
        int SortOrder
        string HelpText
        string Placeholder
    }

    FieldGroup {
        Guid Id PK
        Guid UnitTypeId FK
        string GroupName
        string DisplayName
        string Description
        int SortOrder
        bool IsCollapsible
        bool IsExpandedByDefault
    }

    FieldGroupField {
        Guid Id PK
        Guid FieldId FK
        Guid GroupId FK
        int SortOrder
    }

    UnitFieldValue {
        Guid Id PK
        Guid UnitId FK
        Guid UnitTypeFieldId FK
        string FieldValue
    }

    SearchFilter {
        Guid Id PK
        Guid UnitTypeId FK
        Guid FieldId FK
        string FilterKey
        string FilterName
        string FilterType
        string Operators
        int SortOrder
        bool IsMainFilter
    }

    Booking {
        Guid Id PK
        Guid UserId FK
        Guid UnitId FK
        DateTime CheckIn
        DateTime CheckOut
        int GuestsCount
        decimal TotalPrice
        string Currency
        string Status
        DateTime BookedAt
        string BookingSource
        string CancellationReason
        bool IsWalkIn
        decimal PlatformCommissionAmount
        DateTime ActualCheckInDate
        DateTime ActualCheckOutDate
        decimal FinalAmount
        decimal CustomerRating
        string CompletionNotes
    }

    Payment {
        Guid Id PK
        Guid BookingId FK
        decimal Amount
        string Currency
        string Method
        string TransactionId
        string Status
        DateTime PaymentDate
        string GatewayTransactionId
        Guid ProcessedBy FK
    }

    PropertyService {
        Guid Id PK
        Guid PropertyId FK
        string Name
        decimal Price
        string Currency
        string PricingModel
    }

    BookingService {
        Guid Id PK
        Guid BookingId FK
        Guid ServiceId FK
        int Quantity
        decimal TotalPrice
        string Currency
    }

    Review {
        Guid Id PK
        Guid BookingId FK
        Guid PropertyId FK
        int Cleanliness
        int Service
        int Location
        int Value
        decimal AverageRating
        string Comment
        DateTime CreatedAt
        string ResponseText
        DateTime ResponseDate
        bool IsPendingApproval
    }

    ReviewImage {
        Guid Id PK
        Guid ReviewId FK
        string Name
        string Url
        long SizeBytes
        string Type
        string Category
        string Caption
        string AltText
        string Tags
        bool IsMain
        int DisplayOrder
        DateTime UploadedAt
        string Status
    }

    PropertyImage {
        Guid Id PK
        Guid PropertyId FK
        Guid UnitId FK
        string Name
        string Url
        long SizeBytes
        string Type
        string Category
        string Caption
        string AltText
        string Tags
        string Sizes
        bool IsMain
        int SortOrder
        int Views
        int Downloads
        DateTime UploadedAt
        int DisplayOrder
        string Status
        bool IsMainImage
    }

    Amenity {
        Guid Id PK
        string Name
        string Icon
        string Category
    }

    PropertyTypeAmenity {
        Guid Id PK
        Guid PropertyTypeId FK
        Guid AmenityId FK
        bool IsDefault
    }

    PropertyAmenity {
        Guid Id PK
        Guid PropertyId FK
        Guid PtaId FK
        bool IsAvailable
        decimal ExtraCost
        string Currency
    }

    PropertyPolicy {
        Guid Id PK
        Guid PropertyId FK
        string Type
        int CancellationWindowDays
        bool RequireFullPaymentBeforeConfirmation
        decimal MinimumDepositPercentage
        int MinHoursBeforeCheckIn
        string Description
        string Rules
    }

    Staff {
        Guid Id PK
        Guid PropertyId FK
        Guid UserId FK
        string Position
        string Permissions
        DateTime StartDate
        DateTime EndDate
        bool IsActive
    }

    PricingRule {
        Guid Id PK
        Guid UnitId FK
        string PriceType
        DateTime StartDate
        DateTime EndDate
        TimeSpan StartTime
        TimeSpan EndTime
        decimal PriceAmount
        string Currency
        string PricingTier
        decimal PercentageChange
        decimal MinPrice
        decimal MaxPrice
        string Description
    }

    UnitAvailability {
        Guid Id PK
        Guid UnitId FK
        Guid BookingId FK
        DateTime StartDate
        DateTime EndDate
        string Status
        string Reason
        string Notes
    }

    Notification {
        Guid Id PK
        string Type
        string Title
        string Message
        string TitleAr
        string MessageAr
        string Priority
        string Status
        Guid RecipientId FK
        Guid SenderId FK
        string Data
        string Channels
        string SentChannels
        bool IsRead
        bool IsDismissed
        bool RequiresAction
        bool CanDismiss
        DateTime ReadAt
        DateTime DismissedAt
        DateTime ScheduledFor
        DateTime ExpiresAt
        DateTime DeliveredAt
        string GroupId
        string BatchId
        string Icon
        string Color
        string ActionUrl
        string ActionText
    }

    Report {
        Guid Id PK
        Guid ReporterUserId FK
        Guid ReportedUserId FK
        Guid ReportedPropertyId FK
        string Reason
        string Description
        string Status
        string ActionNote
        Guid AdminId FK
    }

    ChatConversation {
        Guid Id PK
        string ConversationType
        string Title
        string Description
        string Avatar
        bool IsArchived
        bool IsMuted
        Guid PropertyId FK
    }

    ChatMessage {
        Guid Id PK
        Guid ConversationId FK
        Guid SenderId FK
        string MessageType
        string Content
        string LocationJson
        Guid ReplyToMessageId FK
        string Status
        bool IsEdited
        DateTime EditedAt
        DateTime DeliveredAt
        DateTime ReadAt
    }

    ChatAttachment {
        Guid Id PK
        Guid ConversationId FK
        string FileName
        string ContentType
        long FileSize
        string FilePath
        Guid UploadedBy FK
        DateTime CreatedAt
    }

    MessageReaction {
        Guid Id PK
        Guid MessageId FK
        Guid UserId FK
        string ReactionType
    }

    ChatSettings {
        Guid Id PK
        Guid UserId FK
        bool NotificationsEnabled
        bool SoundEnabled
        bool ShowReadReceipts
        bool ShowTypingIndicator
        string Theme
        string FontSize
        bool AutoDownloadMedia
        bool BackupMessages
    }

    %% العلاقات
    User ||--o{ UserRole : has
    Role ||--o{ UserRole : has
    User ||--o{ Property : owns
    User ||--o{ Booking : makes
    User ||--o{ Staff : works_as
    User ||--o{ Report : makes
    User ||--o{ Report : reported_in
    User ||--o{ Notification : receives
    User ||--o{ Notification : sends
    User ||--o{ ChatSettings : has
    User }o--o{ ChatConversation : participates_in
    User ||--o{ ChatMessage : sends
    User ||--o{ MessageReaction : makes

    PropertyType ||--o{ Property : categorizes
    PropertyType ||--o{ UnitType : has
    PropertyType ||--o{ PropertyTypeAmenity : has

    Property ||--o{ Unit : contains
    Property ||--o{ PropertyService : offers
    Property ||--o{ PropertyPolicy : has
    Property ||--o{ Review : receives
    Property ||--o{ Staff : employs
    Property ||--o{ PropertyImage : has
    Property ||--o{ PropertyAmenity : has
    Property ||--o{ Report : reported_in
    Property ||--o{ ChatConversation : related_to

    UnitType ||--o{ Unit : categorizes
    UnitType ||--o{ UnitTypeField : has
    UnitType ||--o{ FieldGroup : has
    UnitType ||--o{ SearchFilter : has

    Unit ||--o{ Booking : booked_in
    Unit ||--o{ PropertyImage : has
    Unit ||--o{ UnitAvailability : has
    Unit ||--o{ PricingRule : has
    Unit ||--o{ UnitFieldValue : has

    UnitTypeField ||--o{ FieldGroupField : grouped_in
    UnitTypeField ||--o{ UnitFieldValue : stores
    UnitTypeField ||--o{ SearchFilter : used_in

    FieldGroup ||--o{ FieldGroupField : contains

    Booking ||--o{ Payment : has
    Booking ||--o{ BookingService : includes
    Booking ||--o{ Review : generates
    Booking ||--o{ UnitAvailability : blocks

    PropertyService ||--o{ BookingService : included_in

    Review ||--o{ ReviewImage : has

    Amenity ||--o{ PropertyTypeAmenity : used_in
    PropertyTypeAmenity ||--o{ PropertyAmenity : implemented_as

    ChatConversation ||--o{ ChatMessage : contains
    ChatConversation ||--o{ ChatAttachment : has

    ChatMessage ||--o{ MessageReaction : has
    ChatMessage ||--o{ ChatAttachment : has