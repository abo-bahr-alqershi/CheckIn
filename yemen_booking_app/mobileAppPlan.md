lib/
├── app.dart
├── main.dart
├── injection_container.dart                         # Dependency Injection
├── presentation/
│   └── screens/
│       ├── main_screen.dart                     # الشاشة الرئيسية مع Bottom Navigation
│       └── splash_screen.dart                 # شاشة البداية
│
├── core/
│   ├── bloc/
│   │   └── app_bloc.dart
│   ├── constants/
│   │   ├── animation_constants.dart
│   │   ├── api_constants.dart
│   │   ├── app_constants.dart
│   │   ├── route_constants.dart
│   │   ├── home_constants.dart
│   │   ├── section_constants.dart
│   │   └── storage_constants.dart
│   ├── enums/
│   │   ├── booking_status.dart
│   │   ├── payment_method_enum.dart
│   │   ├── section_target_enum.dart
│   │   └── section_type_enum.dart
│   ├── error/
│   │   ├── error_handler.dart
│   │   ├── exceptions.dart
│   │   └── failures.dart
│   ├── localization/
│   │   ├── app_localizations.dart
│   │   ├── locale_manager.dart
│   │   └── l10n/
│   │       ├── app_ar.arb
│   │       └── app_en.arb
│   ├── network/
│   │   ├── api_client.dart
│   │   ├── api_interceptors.dart
│   │   ├── api_exceptions.dart
│   │   └── network_info.dart
│   ├── theme/
│   │   ├── app_colors.dart
│   │   ├── app_dimensions.dart
│   │   ├── app_text_styles.dart
│   │   ├── app_theme.dart
│   │   └── app_theme.dart
│   ├── utils/
│   │   ├── color_extensions.dart
│   │   ├── date_utils.dart
│   │   ├── formatters.dart
│   │   ├── image_utils.dart
│   │   ├── location_utils.dart
│   │   ├── price_calculator.dart
│   │   ├── request_logger.dart
│   │   └── validators.dart
│   ├── models/
│   │   ├── paginated_result.dart
│   │   └── result_dto.dart
│   └── widgets/
│       ├── app_bar_widget.dart
│       ├── cached_image_widget.dart
│       ├── empty_widget.dart
│       ├── error_widget.dart
│       ├── loading_widget.dart
│       ├── price_widget.dart
│       └── rating_widget.dart
│
├── features/
│   ├── auth/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── auth_local_datasource.dart
│   │   │   │   └── auth_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── auth_response_model.dart
│   │   │   │   └── user_model.dart
│   │   │   └── repositories/
│   │   │       └── auth_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── auth_response.dart
│   │   │   │   └── user.dart
│   │   │   ├── repositories/
│   │   │   │   └── auth_repository.dart
│   │   │   └── usecases/
│   │   │       ├── check_auth_status_usecase.dart
│   │   │       ├── get_current_user_usecase.dart
│   │   │       ├── login_usecase.dart
│   │   │       ├── logout_usecase.dart
│   │   │       ├── register_usecase.dart
│   │   │       ├── reset_password_usecase.dart
│   │   │       ├── update_profile_usecase.dart
│   │   │       └── upload_user_image_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── auth_bloc.dart
│   │       │   ├── auth_event.dart
│   │       │   └── auth_state.dart
│   │       ├── pages/
│   │       │   ├── forgot_password_page.dart
│   │       │   ├── login_page.dart
│   │       │   ├── profile_page.dart
│   │       │   └── register_page.dart
│   │       └── widgets/
│   │           ├── animated_auth_button.dart
│   │           ├── auth_header_widget.dart
│   │           ├── login_form.dart
│   │           ├── otp_input_widget.dart
│   │           ├── password_strength_indicator.dart
│   │           ├── register_form.dart
│   │           ├── social_login_buttons.dart
│   │           └── upload_user_image.dart
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
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── booking.dart
│   │   │   │   ├── booking_request.dart
│   │   │   │   └── payment.dart
│   │   │   ├── repositories/
│   │   │   │   └── booking_repository.dart
│   │   │   └── usecases/
│   │   │       ├── add_services_to_booking_usecase.dart
│   │   │       ├── cancel_booking_usecase.dart
│   │   │       ├── check_availability_usecase.dart
│   │   │       ├── create_booking_usecase.dart
│   │   │       ├── get_booking_details_usecase.dart
│   │   │       ├── get_user_bookings_summary_usecase.dart
│   │   │       ├── get_user_bookings_usecase.dart
│   │   │       └── get_user_bookings_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── booking_bloc.dart
│   │       │   ├── booking_event.dart
│   │       │   └── booking_state.dart
│   │       ├── pages/
│   │       │   ├── booking_confirmation_page.dart
│   │       │   ├── booking_details_page.dart
│   │       │   ├── booking_form_page.dart
│   │       │   ├── booking_payment_page.dart
│   │       │   ├── booking_summary_page.dart
│   │       │   └── my_bookings_page.dart
│   │       └── widgets/
│   │           ├── booking_card_widget.dart
│   │           ├── booking_status_widget.dart
│   │           ├── cancellation_deadline_has_expired_widget.dart
│   │           ├── date_picker_widget.dart
│   │           ├── guest_selector_widget.dart
│   │           ├── payment_methods_widget.dart
│   │           ├── price_breakdown_widget.dart
│   │           └── services_selector_widget.dart
│   │
│   ├── chat/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── chat_local_datasource.dart
│   │   │   │   └── chat_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── attachment_model.dart
│   │   │   │   ├── chat_settings_model.dart
│   │   │   │   ├── chat_user_model.dart
│   │   │   │   ├── conversation_model.dart
│   │   │   │   ├── delivery_receipt_model.dart
│   │   │   │   ├── message_model.dart
│   │   │   │   ├── message_reaction_model.dart
│   │   │   │   └── search_result_model.dart
│   │   │   └── repositories/
│   │   │       └── chat_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── attachment.dart
│   │   │   │   ├── conversation.dart
│   │   │   │   └── message.dart
│   │   │   ├── repositories/
│   │   │   │   └── chat_repository.dart
│   │   │   └── usecases/
│   │   │       ├── add_reaction_usecase.dart
│   │   │       ├── archive_conversation_usecase.dart
│   │   │       ├── create_conversation_usecase.dart
│   │   │       ├── delete_conversation_usecase.dart
│   │   │       ├── delete_message_usecase.dart
│   │   │       ├── edit_message_usecase.dart
│   │   │       ├── get_available_users_usecase.dart
│   │   │       ├── get_chat_settings_usecase.dart
│   │   │       ├── get_conversations_usecase.dart
│   │   │       ├── get_messages_usecase.dart
│   │   │       ├── mark_as_read_usecase.dart
│   │   │       ├── remove_reaction_usecase.dart
│   │   │       ├── search_chats_usecase.dart
│   │   │       ├── send_message_usecase.dart
│   │   │       ├── update_chat_settings_usecase.dart
│   │   │       └── upload_attachment_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── chat_bloc.dart
│   │       │   ├── chat_event.dart
│   │       │   └── chat_state.dart
│   │       ├── providers/
│   │       │   └── typing_indicator_provider.dart
│   │       ├── pages/
│   │       │   ├── chat_page.dart
│   │       │   ├── chat_settings_page.dart
│   │       │   └── conversations_page.dart
│   │       └── widgets/
│   │           ├── attachment_preview_widget.dart
│   │           ├── chat_app_bar.dart
│   │           ├── chat_fab.dart
│   │           ├── chat_search_bar.dart
│   │           ├── conversation_item_widget.dart
│   │           ├── message_bubble_widget.dart
│   │           ├── message_input_widget.dart
│   │           ├── typing_indicator_widget.dart
│   │           ├── reaction_picker_widget.dart
│   │           ├── online_status_indicator.dart
│   │           └── message_status_indicator.dart
│   │
│   ├── favorites/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── favorites_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── favorite_property_model.dart
│   │   │   └── repositories/
│   │   │       └── favorites_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── favorite_property.dart
│   │   │   ├── repositories/
│   │   │   │   └── favorites_repository.dart
│   │   │   └── usecases/
│   │   │       ├── add_to_favorites_usecase.dart
│   │   │       ├── get_favorites_usecase.dart
│   │   │       └── remove_from_favorites_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── favorites_bloc.dart
│   │       │   ├── favorites_event.dart
│   │       │   └── favorites_state.dart
│   │       ├── pages/
│   │       │   └── favorites_page.dart
│   │       └── widgets/
│   │           ├── favorite_button_widget.dart
│   │           └── favorite_property_card_widget.dart
│   │
│   ├── home/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── home_local_datasource.dart
│   │   │   │   └── home_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── property_model.dart
│   │   │   │   ├── property_type_model.dart
│   │   │   │   ├── section_model.dart
│   │   │   │   └── unit_type_model.dart
│   │   │   └── repositories/
│   │   │       └── home_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── property_type.dart
│   │   │   │   ├── section.dart
│   │   │   │   └── unit_type.dart
│   │   │   ├── repositories/
│   │   │   │   └── home_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_property_types_usecase.dart
│   │   │       ├── get_section_data_usecase.dart
│   │   │       ├── get_sections_usecase.dart
│   │   │       └── get_unit_types_with_fields_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── home_bloc.dart
│   │       │   ├── home_event.dart
│   │       │   └── home_state.dart
│   │       ├── pages/
│   │       │   ├── home_page.dart
│   │       │   ├── all_sections_page.dart              # صفحة عرض جميع الأقسام
│   │       │   └── section_details_page.dart          # صفحة تفاصيل القسم
│   │       └── widgets/
│   │           ├── sections/
│   │           │   ├── base_section_widget.dart       # القاعدة لجميع أنواع الأقسام
│   │           │   ├── section_header_widget.dart     # رأس القسم مع العنوان وزر "عرض الكل"
│   │           │   ├── section_loading_widget.dart    # حالة التحميل للقسم
│   │           │   ├── section_error_widget.dart      # حالة الخطأ للقسم
│   │           │   ├── section_empty_widget.dart      # حالة القسم الفارغ
│   │           │   │
│   │           │   ├── ads/                           # أقسام الإعلانات
│   │           │   │   ├── single_property_ad_widget.dart
│   │           │   │   ├── multi_property_ad_widget.dart
│   │           │   │   └── unit_showcase_ad_widget.dart
│   │           │   │
│   │           │   ├── offers/                        # أقسام العروض
│   │           │   │   ├── single_property_offer_widget.dart
│   │           │   │   ├── limited_time_offer_widget.dart
│   │           │   │   ├── seasonal_offer_widget.dart
│   │           │   │   ├── multi_property_offers_grid_widget.dart
│   │           │   │   ├── offers_carousel_widget.dart
│   │           │   │   ├── flash_deals_widget.dart
│   │           │   │   └── offer_countdown_timer_widget.dart
│   │           │   │
│   │           │   ├── properties/                    # أقسام العقارات
│   │           │   │   ├── horizontal_property_list_widget.dart
│   │           │   │   ├── vertical_property_grid_widget.dart
│   │           │   │   ├── mixed_layout_list_widget.dart
│   │           │   │   ├── compact_property_list_widget.dart
│   │           │   │   └── property_card_variants/
│   │           │   │       ├── horizontal_property_card.dart
│   │           │   │       ├── vertical_property_card.dart
│   │           │   │       ├── compact_property_card.dart
│   │           │   │       └── featured_property_card.dart
│   │           │   │
│   │           │   ├── destinations/                  # أقسام الوجهات
│   │           │   │   ├── city_cards_grid_widget.dart
│   │           │   │   ├── destination_carousel_widget.dart
│   │           │   │   ├── explore_cities_widget.dart
│   │           │   │   └── city_card_widget.dart
│   │           │   │
│   │           │   └── premium/                       # أقسام متميزة
│   │           │       ├── premium_carousel_widget.dart
│   │           │       ├── interactive_showcase_widget.dart
│   │           │       └── premium_property_card.dart
│   │           │
│   │           ├── search/
│   │           │   ├── home_search_bar_widget.dart    # شريط البحث الرئيسي
│   │           │   ├── quick_search_widget.dart       # بحث سريع
│   │           │   ├── search_filters_chips.dart     # شرائح الفلاتر السريعة
│   │           │   └── recent_searches_widget.dart    # عمليات البحث الأخيرة
│   │           │
│   │           ├── categories/
│   │           │   ├── property_types_grid_widget.dart    # شبكة أنواع العقارات
│   │           │   ├── property_type_card_widget.dart     # بطاقة نوع العقار
│   │           │   ├── unit_types_list_widget.dart        # قائمة أنواع الوحدات
│   │           │   └── unit_type_card_widget.dart         # بطاقة نوع الوحدة
│   │           │
│   │           ├── banners/
│   │           │   ├── hero_banner_widget.dart        # البانر الرئيسي
│   │           │   ├── promotional_banner_widget.dart # بانر ترويجي
│   │           │   ├── image_slider_widget.dart       # عارض الصور المتحرك
│   │           │   └── banner_indicator_widget.dart   # مؤشرات البانر
│   │           │
│   │           ├── common/
│   │           │   ├── section_shimmer_widget.dart    # تأثير الشيمر للتحميل
│   │           │   ├── view_all_button_widget.dart    # زر عرض الكل
│   │           │   ├── refresh_indicator_widget.dart  # مؤشر التحديث
│   │           │   ├── home_app_bar_widget.dart       # شريط التطبيق للصفحة الرئيسية
│   │           │   ├── location_selector_widget.dart  # محدد الموقع
│   │           │   ├── notification_bell_widget.dart  # جرس الإشعارات
│   │           │   └── user_avatar_widget.dart        # صورة المستخدم
│   │           │
│   │           ├── analytics/
│   │           │   ├── section_visibility_detector.dart   # كاشف رؤية القسم
│   │           │   └── interaction_tracker_widget.dart    # تتبع التفاعلات
│   │           │
│   │           └── animations/
│   │               ├── fade_in_animation_widget.dart      # حركة الظهور التدريجي
│   │               ├── slide_animation_widget.dart        # حركة الانزلاق
│   │               ├── scale_animation_widget.dart        # حركة التكبير/التصغير
│   │               └── stagger_animation_widget.dart      # حركة متتابعة
│   │
│   ├── property/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── property_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── amenity_model.dart
│   │   │   │   ├── property_detail_model.dart
│   │   │   │   ├── review_model.dart
│   │   │   │   └── unit_model.dart
│   │   │   └── repositories/
│   │   │       ├── property_repository_impl.dart
│   │   │       └─ property_repository_impl_v2.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── amenity.dart
│   │   │   │   ├── property_detail.dart
│   │   │   │   ├── property_policy.dart
│   │   │   │   └─ unit.dart
│   │   │   ├── repositories/
│   │   │   │   └─ property_repository.dart
│   │   │   └── usecases/
│   │   │       ├─ add_to_favorites_usecase.dart
│   │   │       ├─ get_property_details_usecase.dart
│   │   │       ├─ get_property_units_usecase.dart
│   │   │       └ get_property_reviews_usecase.dart
│   │   └── presentation/
│   │       ├─ bloc/
│   │       │   ├─ property_bloc.dart
│   │       │   ├─ property_event.dart
│   │       │   └─ property_state.dart
│   │       └─ pages/
│   │           ├─ property_details_page.dart
│   │           ├─ property_gallery_page.dart
│   │           ├─ property_map_page.dart
│   │           ├─ property_reviews_page.dart
│   │           └─ property_units_page.dart
│   │       └─ widgets/
│   │           ├─ amenities_grid_widget.dart
│   │           ├─ location_map_widget.dart
│   │           ├─ policies_widget.dart
│   │           ├─ property_header_widget.dart
│   │           ├─ property_images_grid_widget.dart
│   │           ├─ property_info_widget.dart
│   │           ├─ reviews_summary_widget.dart
│   │           └─ units_list_widget.dart
│   │
│   ├── payment/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── payment_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── transaction_model.dart
│   │   │   └── repositories/
│   │   │       └─ payment_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └─ transaction.dart
│   │   │   ├── repositories/
│   │   │   │   └─ payment_repository.dart
│   │   │   └── usecases/
│   │   │       ├─ get_payment_history_usecase.dart
│   │   │       └ process_payment_usecase.dart
│   │   └── presentation/
│   │       ├─ bloc/
│   │       │   ├─ payment_bloc.dart
│   │       │   ├─ payment_event.dart
│   │       │   └- payment_state.dart
│   │       ├─ pages/
│   │       │   ├─ add_payment_method_page.dart
│   │       │   ├─ payment_history_page.dart
│   │       │   └- payment_methods_page.dart
│   │       └─ widgets/
│   │           ├─ credit_card_form_widget.dart
│   │           ├─ payment_method_card_widget.dart
│   │           └─ transaction_item_widget.dart
│   │
│   ├── review/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └─ review_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├─ review_model.dart
│   │   │   │   └─ review_image_model.dart
│   │   │   └── repositories/
│   │   │       └─ review_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├─ review.dart
│   │   │   │   └─ review_image.dart
│   │   │   ├── repositories/
│   │   │   │   └─ review_repository.dart
│   │   │   └── usecases/
│   │   │       ├─ create_review_usecase.dart
│   │   │       ├─ get_property_reviews_Summary_usecase.dart
│   │   │       ├─ get_property_reviews_usecase.dart
│   │   │       └─ upload_review_images_usecase.dart
│   │   └── presentation/
│   │       ├─ bloc/
│   │       │   ├─ review_bloc.dart
│   │       │   ├─ review_event.dart
│   │       │   └- review_state.dart
│   │       ├─ pages/
│   │       │   ├─ write_review_page.dart
│   │       │   └─ reviews_list_page.dart
│   │       └─ widgets/
│   │           ├─ rating_selector_widget.dart
│   │           ├─ review_card_widget.dart
│   │           ├─ review_form_widget.dart
│   │           ├─ review_images_picker_widget.dart
│   │           └─ upload_review_image_widget.dart
│   │
│   ├── search/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └─ search_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├─ search_filter_model.dart
│   │   │   │   ├─ search_properties_response_model.dart
│   │   │   │   ├─ search_result_model.dart
│   │   │   │   └─ search_statistics_model.dart
│   │   │   └─ repositories/
│   │   │       └─ search_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├─ search_filter.dart
│   │   │   │   └─ search_result.dart
│   │   │   ├── repositories/
│   │   │   │   └─ search_repository.dart
│   │   │   └── usecases/
│   │   │       ├─ get_search_filters_usecase.dart
│   │   │       ├─ get_search_suggestions_usecase.dart
│   │   │       └─ search_properties_usecase.dart
│   │   └── presentation/
│   │       ├─ bloc/
│   │       │   ├─ search_bloc.dart
│   │       │   ├─ search_event.dart
│   │       │   └─ search_state.dart
│   │       ├─ pages/
│   │       │   ├─ search_page.dart
│   │       │   ├─ search_filters_page.dart
│   │       │   ├─ search_results_page.dart
│   │       │   └─ search_results_map_page.dart
│   │       └─ widgets/
│   │           ├─ dynamic_fields_widget.dart
│   │           ├─ filter_chips_widget.dart
│   │           ├─ price_range_slider_widget.dart
│   │           ├─ search_input_widget.dart
│   │           ├─ search_result_card_widget.dart
│   │           ├─ search_result_grid_widget.dart
│   │           ├─ search_result_list_widget.dart
│   │           └─ sort_options_widget.dart
│   │
│   ├── notifications/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├─ notification_local_datasource.dart
│   │   │   │   └─ notification_remote_datasource.dart
│   │   │   ├─ models/
│   │   │   │   └─ notification_model.dart
│   │   │   └─ repositories/
│   │   │       └─ notification_repository_impl.dart
│   │   ├── domain/
│   │   │   ├─ entities/
│   │   │   │   └─ notification.dart
│   │   │   ├─ repositories/
│   │   │   │   └─ notification_repository.dart
│   │   │   └─ usecases/
│   │   │       ├─ dismiss_notification_usecase.dart
│   │   │       ├─ get_notifications_usecase.dart
│   │   │       ├─ mark_as_read_usecase.dart
│   │   │       └─ update_notification_settings_usecase.dart
│   │   └─ presentation/
│   │       ├─ bloc/
│   │       │   ├─ notification_bloc.dart
│   │       │   ├─ notification_event.dart
│   │       │   └─ notification_state.dart
│   │       ├─ pages/
│   │       │   ├─ notification_settings_page.dart
│   │       │   └─ notifications_page.dart
│   │       └─ widgets/
│   │           ├─ notification_badge_widget.dart
│   │           ├─ notification_filter_widget.dart
│   │           └─ notification_item_widget.dart
│   │
│   └── settings/
│       ├── data/
│       │   ├── datasources/
│       │   │   └─ settings_local_datasource.dart
│       │   ├─ models/
│       │   │   └─ app_settings_model.dart
│       │   └─ repositories/
│       │       └─ settings_repository_impl.dart
│       ├─ domain/
│       │   ├─ entities/
│       │   │   └─ app_settings.dart
│       │   ├─ repositories/
│       │   │   └─ settings_repository.dart
│       │   └─ usecases/
│       │       ├─ get_settings_usecase.dart
│       │       ├─ update_language_usecase.dart
│       │       ├─ update_notification_settings_usecase.dart
│       │       └ update_theme_usecase.dart
│       └─ presentation/
│           ├─ bloc/
│           │   ├─ settings_bloc.dart
│           │   ├─ settings_event.dart
│           │   └─ settings_state.dart
│           ├─ pages/
│           │   ├─ about_page.dart
│           │   ├─ language_settings_page.dart
│           │   ├─ privacy_policy_page.dart
│           │   └─ settings_page.dart
│           └─ widgets/
│               ├─ language_selector_widget.dart
│               ├─ settings_item_widget.dart
│               └─ theme_selector_widget.dart
│
├── routes/
│   ├── app_router.dart
│   ├── route_animations.dart
│   └── route_guards.dart
│
├── services/
│   ├── analytics_service.dart
│   ├── crash_reporting_service.dart
│   ├── deep_link_service.dart
│   ├── local_storage_service.dart
│   ├── location_service.dart
│   ├── notification_service.dart
│   └─ websocket_service.dart
└── home_cache_service.dart
    ├─ section_analytics_service.dart
    └─ dynamic_content_service.dart

# ملفات إضافية مهمة


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
