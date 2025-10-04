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
│   │   ├── api_constants.dart
│   │   ├── app_constants.dart
│   │   ├── route_constants.dart
│   │   └── storage_constants.dart
│   ├── enums/
│   │   ├── booking_status.dart
│   │   ├── payment_method_enum.dart
│   │   ├── section_content_type.dart    # [Properties, Units, Mixed]
│   │   ├── section_type.dart            # [Featured, Popular, NewArrivals, etc.]
│   │   └── section_display_style.dart   # [Grid, List, Carousel, Map]
│   │
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
│   │   ├── section_item_dto.dart
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
|   ├── admin_sections/
|   |    ├── data/
|   |    │   ├── datasources/
|   |    │   │   ├── sections_local_datasource.dart
|   |    │   │   └── sections_remote_datasource.dart
|   |    │   ├── models/
|   |    │   │   ├── section_model.dart
|   |    │   │   ├── section_item_model.dart
|   |    │   │   ├── property_in_section_model.dart
|   |    │   │   ├── unit_in_section_model.dart
|   |    │   │   ├── section_metadata_model.dart
|   |    │   │   └── dynamic_field_value_model.dart
|   |    │   └── repositories/
|   |    │       └── sections_repository_impl.dart
|   |    │
|   |    ├── domain/
|   |    │   ├── entities/
|   |    │   │   ├── section.dart
|   |    │   │   ├── section_item.dart
|   |    │   │   ├── property_in_section.dart
|   |    │   │   ├── unit_in_section.dart
|   |    │   │   ├── section_metadata.dart
|   |    │   │   └── section_filter_criteria.dart
|   |    │   ├── repositories/
|   |    │   │   └── sections_repository.dart
|   |    │   └── usecases/
|   |    │       ├── sections/
|   |    │       │   ├── create_section_usecase.dart
|   |    │       │   ├── update_section_usecase.dart
|   |    │       │   ├── delete_section_usecase.dart
|   |    │       │   ├── toggle_section_status_usecase.dart
|   |    │       │   ├── get_all_sections_usecase.dart
|   |    │       │   ├── get_section_by_id_usecase.dart
|   |    │       │   └── get_active_sections_for_home_usecase.dart
|   |    │       └── section_items/
|   |    │           ├── add_items_to_section_usecase.dart
|   |    │           ├── remove_items_from_section_usecase.dart
|   |    │           ├── update_item_order_usecase.dart
|   |    │           └── get_section_items_usecase.dart
|   |    │
|   |    └── presentation/
|   |        ├── bloc/
|   |        │   ├── sections_list/
|   |        │   │   ├── sections_list_bloc.dart
|   |        │   │   ├── sections_list_event.dart
|   |        │   │   └── sections_list_state.dart
|   |        │   ├── section_form/
|   |        │   │   ├── section_form_bloc.dart
|   |        │   │   ├── section_form_event.dart
|   |        │   │   └── section_form_state.dart
|   |        │   └── section_items/
|   |        │       ├── section_items_bloc.dart
|   |        │       ├── section_items_event.dart
|   |        │       └── section_items_state.dart
|   |        ├── pages/
|   |        │   ├── sections_list_page.dart
|   |        │   ├── create_section_page.dart
|   |        │   ├── edit_section_page.dart
|   |        │   └── section_items_management_page.dart
|   |        └── widgets/
|   |            ├── futuristic_section_card.dart
|   |            ├── futuristic_sections_table.dart
|   |            ├── section_form_widget.dart
|   |            ├── section_type_selector.dart
|   |            ├── section_content_type_toggle.dart
|   |            ├── section_display_style_picker.dart
|   |            ├── section_filter_criteria_editor.dart
|   |            ├── section_sort_criteria_editor.dart
|   |            ├── section_metadata_editor.dart
|   |            ├── section_schedule_picker.dart
|   |            ├── section_items_list.dart
|   |            ├── add_items_dialog.dart
|   |            ├── unit_item_card.dart
|   |            ├── property_item_card.dart
|   |            ├── image_gallery.dart
|   |            ├── item_order_drag_handle.dart
|   |            ├── section_preview_widget.dart
|   |            ├── section_stats_card.dart
|   |            └── section_status_badge.dart
|   ├── admin_properties/
|   |   ├── data/
|   |   │   ├── datasources/
|   |   │   │   ├── properties_local_datasource.dart
|   |   │   │   ├── properties_remote_datasource.dart
|   |   │   │   ├── property_types_remote_datasource.dart
|   |   │   │   ├── amenities_remote_datasource.dart
|   |   │   │   ├── policies_remote_datasource.dart
|   |   │   │   └── property_images_remote_datasource.dart
|   |   │   ├── models/
|   |   │   │   ├── property_model.dart
|   |   │   │   ├── property_type_model.dart
|   |   │   │   ├── amenity_model.dart
|   |   │   │   ├── policy_model.dart
|   |   │   │   ├── property_image_model.dart
|   |   │   │   ├── property_search_model.dart
|   |   │   │   └── map_marker_model.dart
|   |   │   └── repositories/
|   |   │       ├── properties_repository_impl.dart
|   |   │       ├── property_types_repository_impl.dart
|   |   │       ├── amenities_repository_impl.dart
|   |   │       ├── property_images_repository_impl.dart
|   |   │       └── policies_repository_impl.dart
|   |   ├── domain/
|   |   │   ├── entities/
|   |   │   │   ├── property.dart
|   |   │   │   ├── property_type.dart
|   |   │   │   ├── amenity.dart
|   |   │   │   ├── policy.dart
|   |   │   │   ├── property_image.dart
|   |   │   │   ├── property_search_result.dart
|   |   │   │   └── map_location.dart
|   |   │   ├── repositories/
|   |   │   │   ├── properties_repository.dart
|   |   │   │   ├── property_types_repository.dart
|   |   │   │   ├── amenities_repository.dart
|   |   │       ├── property_images_repository.dart
|   |   │   │   └── policies_repository.dart
|   |   │   └── usecases/
|   |   │       ├── properties/
|   |   │       │   ├── create_property_usecase.dart
|   |   │       │   ├── update_property_usecase.dart
|   |   │       │   ├── delete_property_usecase.dart
|   |   │       │   ├── get_all_properties_usecase.dart
|   |   │       │   ├── get_property_details_usecase.dart
|   |   │       │   ├── approve_property_usecase.dart
|   |   │       │   ├── reject_property_usecase.dart
|   |   │       │   ├── reject_property_usecase.dart
|   |   │       │   ├── reject_property_usecase.dart
|   |   │       │   ├── reject_property_usecase.dart
|   |   │       │   └── search_properties_usecase.dart
|   |   │       ├── property_types/
|   |   │       │   ├── create_property_type_usecase.dart
|   |   │       │   ├── update_property_type_usecase.dart
|   |   │       │   ├── delete_property_type_usecase.dart
|   |   │       │   └── get_property_types_usecase.dart
|   |   │       ├── amenities/
|   |   │       │   ├── create_amenity_usecase.dart
|   |   │       │   ├── update_amenity_usecase.dart
|   |   │       │   ├── delete_amenity_usecase.dart
|   |   │       │   ├── get_amenities_usecase.dart
|   |   │       │   └── assign_amenity_to_property_usecase.dart
|   |   │       └── policies/
|   |   │           ├── create_policy_usecase.dart
|   |   │           ├── update_policy_usecase.dart
|   |   │           ├── delete_policy_usecase.dart
|   |   │           └── get_policies_usecase.dart
|   |   └── presentation/
|   |       ├── bloc/
|   |       │   ├── properties/
|   |       │   │   ├── properties_bloc.dart
|   |       │   │   ├── properties_event.dart
|   |       │   │   └── properties_state.dart
|   |       │   ├── property_types/
|   |       │   │   ├── property_types_bloc.dart
|   |       │   │   ├── property_types_event.dart
|   |       │   │   └── property_types_state.dart
|   |       │   ├── amenities/
|   |       │   │   ├── amenities_bloc.dart
|   |       │   │   ├── amenities_event.dart
|   |       │   │   └── amenities_state.dart
|   |       │   └── policies/
|   |       │       ├── policies_bloc.dart
|   |       │       ├── policies_event.dart
|   |       │       └── policies_state.dart
|   |       ├── pages/
|   |       │   ├── properties_list_page.dart
|   |       │   ├── property_details_page.dart
|   |       │   ├── create_property_page.dart
|   |       │   ├── edit_property_page.dart
|   |       │   ├── property_types_page.dart
|   |       │   ├── amenities_management_page.dart
|   |       │   └── policies_management_page.dart
|   |       └── widgets/
|   |           ├── property_card_widget.dart
|   |           ├── futuristic_property_table.dart
|   |           ├── property_filters_widget.dart
|   |           ├── property_map_view.dart
|   |           ├── property_image_gallery.dart
|   |           ├── amenity_selector_widget.dart
|   |           ├── policy_editor_widget.dart
|   |           └── property_stats_card.dart
|   |   
|   |   
│   ├── admin_property_types/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── property_types_remote_datasource.dart
│   │   │   │   ├── unit_types_remote_datasource.dart
│   │   │   │   └── unit_type_fields_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── property_type_model.dart
│   │   │   │   ├── unit_type_model.dart
│   │   │   │   └── unit_type_field_model.dart
│   │   │   └── repositories/
│   │   │       ├── property_types_repository_impl.dart
│   │   │       ├── unit_types_repository_impl.dart
│   │   │       └── unit_type_fields_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── property_type.dart
│   │   │   │   ├── unit_type.dart
│   │   │   │   └── unit_type_field.dart
│   │   │   ├── repositories/
│   │   │   │   ├── property_types_repository.dart
│   │   │   │   ├── unit_types_repository.dart
│   │   │   │   └── unit_type_fields_repository.dart
│   │   │   └── usecases/
│   │   │       ├── property_types/
│   │   │       │   ├── get_all_property_types_usecase.dart
│   │   │       │   ├── create_property_type_usecase.dart
│   │   │       │   ├── update_property_type_usecase.dart
│   │   │       │   └── delete_property_type_usecase.dart
│   │   │       ├── unit_types/
│   │   │       │   ├── get_unit_types_by_property_usecase.dart
│   │   │       │   ├── create_unit_type_usecase.dart
│   │   │       │   ├── update_unit_type_usecase.dart
│   │   │       │   └── delete_unit_type_usecase.dart
│   │   │       └── fields/
│   │   │           ├── get_fields_by_unit_type_usecase.dart
│   │   │           ├── create_field_usecase.dart
│   │   │           ├── update_field_usecase.dart
│   │   │           └── delete_field_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── property_types/
│   │       │   │   ├── property_types_bloc.dart
│   │       │   │   ├── property_types_event.dart
│   │       │   │   └── property_types_state.dart
│   │       │   ├── unit_types/
│   │       │   │   ├── unit_types_bloc.dart
│   │       │   │   ├── unit_types_event.dart
│   │       │   │   └── unit_types_state.dart
│   │       │   └── unit_type_fields/
│   │       │       ├── unit_type_fields_bloc.dart
│   │       │       ├── unit_type_fields_event.dart
│   │       │       └── unit_type_fields_state.dart
│   │       ├── pages/
│   │       │   └── admin_property_types_page.dart
│   │       └── widgets/
│   │           ├── property_type_card.dart
│   │           ├── unit_type_card.dart
│   │           ├── unit_type_field_card.dart
│   │           ├── property_type_modal.dart
│   │           ├── unit_type_modal.dart 
│   │           ├── unit_type_field_modal.dart
│   │           ├── icon_picker_modal.dart
│   │           └── futuristic_stats_card.dart
│   │   
│   │ 
│   │ 
│   ├── admin_units/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── units_local_datasource.dart
│   │   │   │   └── units_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── unit_model.dart
│   │   │   │   ├── unit_type_model.dart
│   │   │   │   ├── unit_field_value_model.dart
│   │   │   │   ├── money_model.dart
│   │   │   │   └── pricing_method_model.dart
│   │   │   └── repositories/
│   │   │       └── units_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── unit.dart
│   │   │   │   ├── unit_type.dart
│   │   │   │   ├── unit_field_value.dart
│   │   │   │   ├── money.dart
│   │   │   │   └── pricing_method.dart
│   │   │   ├── repositories/
│   │   │   │   └── units_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_units_usecase.dart
│   │   │       ├── get_unit_details_usecase.dart
│   │   │       ├── create_unit_usecase.dart
│   │   │       ├── update_unit_usecase.dart
│   │   │       ├── delete_unit_usecase.dart
│   │   │       ├── get_unit_types_by_property_usecase.dart
│   │   │       ├── get_unit_fields_usecase.dart
│   │   │       └── assign_unit_to_sections_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── units_list/
│   │       │   │   ├── units_list_bloc.dart
│   │       │   │   ├── units_list_event.dart
│   │       │   │   └── units_list_state.dart
│   │       │   ├── unit_form/
│   │       │   │   ├── unit_form_bloc.dart
│   │       │   │   ├── unit_form_event.dart
│   │       │   │   └── unit_form_state.dart
│   │       │   └── unit_details/
│   │       │       ├── unit_details_bloc.dart
│   │       │       ├── unit_details_event.dart
│   │       │       └── unit_details_state.dart
│   │       ├── pages/
│   │       │   ├── units_list_page.dart
│   │       │   ├── unit_details_page.dart
│   │       │   ├── create_unit_page.dart
│   │       │   ├── edit_unit_page.dart
│   │       │   └── unit_gallery_page.dart
│   │       └── widgets/
│   │           ├── futuristic_unit_card.dart
│   │           ├── futuristic_units_table.dart
│   │           ├── futuristic_unit_map_view.dart
│   │           ├── unit_form_widget.dart
│   │           ├── dynamic_fields_widget.dart
│   │           ├── capacity_selector_widget.dart
│   │           ├── pricing_form_widget.dart
│   │           ├── features_tags_widget.dart
│   │           ├── unit_filters_widget.dart
│   │           ├── unit_stats_card.dart
│   │           └── assign_sections_modal.dart
│   │ 
│   │ 
│   │   
│   ├── admin_amenities/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── amenities_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── amenity_model.dart
│   │   │   └── repositories/
│   │   │       └── amenities_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── amenity.dart
│   │   │   ├── repositories/
│   │   │   │   └── amenities_repository.dart
│   │   │   └── usecases/
│   │   │       ├── create_amenity_usecase.dart
│   │   │       ├── update_amenity_usecase.dart
│   │   │       ├── delete_amenity_usecase.dart
│   │   │       ├── get_all_amenities_usecase.dart
│   │   │       └── assign_amenity_to_property_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── amenities_bloc.dart
│   │       │   ├── amenities_event.dart
│   │       │   └── amenities_state.dart
│   │       ├── pages/
│   │       │   └── amenities_management_page.dart
│   │       ├── utils/
│   │       │   └── amenity_icons.dart
│   │       └── widgets/
│   │           ├── futuristic_amenity_card.dart
│   │           ├── futuristic_amenities_table.dart
│   │           ├── amenity_form_dialog.dart
│   │           ├── amenity_filters_widget.dart
│   │           └── amenity_stats_card.dart
│   │ 
│   │ 
│   │ 
│   ├── admin_services/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── services_local_datasource.dart
│   │   │   │   └── services_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── service_model.dart
│   │   │   │   ├── service_details_model.dart
│   │   │   │   ├── money_model.dart
│   │   │   │   └── pricing_model.dart
│   │   │   └── repositories/
│   │   │       └── services_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── service.dart
│   │   │   │   ├── service_details.dart
│   │   │   │   ├── money.dart
│   │   │   │   └── pricing_model.dart
│   │   │   ├── repositories/
│   │   │   │   └── services_repository.dart
│   │   │   └── usecases/
│   │   │       ├── create_service_usecase.dart
│   │   │       ├── update_service_usecase.dart
│   │   │       ├── delete_service_usecase.dart
│   │   │       ├── get_services_by_property_usecase.dart
│   │   │       ├── get_service_details_usecase.dart
│   │   │       └── get_services_by_type_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── services_bloc.dart
│   │       │   ├── services_event.dart
│   │       │   └── services_state.dart
│   │       ├── pages/
│   │       │   └── admin_services_page.dart
│   │       ├── widgets/
│   │       │   ├── futuristic_service_card.dart
│   │       │   ├── futuristic_services_table.dart
│   │       │   ├── service_form_dialog.dart
│   │       │   ├── service_icon_picker.dart
│   │       │   ├── service_details_dialog.dart
│   │       │   ├── service_stats_card.dart
│   │       │   └── service_filters_widget.dart
│   │       └── utils/
│   │           └── service_icons.dart
│   │
│   ├── admin_reviews/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── reviews_local_datasource.dart
│   │   │   │   └── reviews_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── review_model.dart
│   │   │   │   ├── review_image_model.dart
│   │   │   │   └── review_response_model.dart
│   │   │   └── repositories/
│   │   │       └── reviews_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── review.dart
│   │   │   │   ├── review_image.dart
│   │   │   │   └── review_response.dart
│   │   │   ├── repositories/
│   │   │   │   └── reviews_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_all_reviews_usecase.dart
│   │   │       ├── get_review_details_usecase.dart
│   │   │       ├── approve_review_usecase.dart
│   │   │       ├── reject_review_usecase.dart
│   │   │       ├── delete_review_usecase.dart
│   │   │       ├── respond_to_review_usecase.dart
│   │   │       ├── get_review_responses_usecase.dart
│   │   │       └── delete_review_response_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── reviews_list/
│   │       │   │   ├── reviews_list_bloc.dart
│   │       │   │   ├── reviews_list_event.dart
│   │       │   │   └── reviews_list_state.dart
│   │       │   ├── review_details/
│   │       │   │   ├── review_details_bloc.dart
│   │       │   │   ├── review_details_event.dart
│   │       │   │   └── review_details_state.dart
│   │       │   └── review_response/
│   │       │       ├── review_response_bloc.dart
│   │       │       ├── review_response_event.dart
│   │       │       └── review_response_state.dart
│   │       ├── pages/
│   │       │   ├── reviews_list_page.dart
│   │       │   └── review_details_page.dart
│   │       └── widgets/
│   │           ├── futuristic_review_card.dart
│   │           ├── futuristic_reviews_table.dart
│   │           ├── review_filters_widget.dart
│   │           ├── review_stats_card.dart
│   │           ├── review_images_gallery.dart
│   │           ├── review_response_card.dart
│   │           ├── add_response_dialog.dart
│   │           └── rating_breakdown_widget.dart
│   │
│   │
│   │ 
│   ├── admin_audit_logs/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── audit_logs_local_datasource.dart
│   │   │   │   └── audit_logs_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── audit_log_model.dart
│   │   │   └── repositories/
│   │   │       └── audit_logs_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── audit_log.dart
│   │   │   ├── repositories/
│   │   │   │   └── audit_logs_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_audit_logs_usecase.dart
│   │   │       ├── get_customer_activity_logs_usecase.dart
│   │   │       ├── get_property_activity_logs_usecase.dart
│   │   │       ├── get_admin_activity_logs_usecase.dart
│   │   │       └── export_audit_logs_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── audit_logs_bloc.dart
│   │       │   ├── audit_logs_event.dart
│   │       │   └── audit_logs_state.dart
│   │       ├── pages/
│   │       │   └── audit_logs_page.dart
│   │       └── widgets/
│   │           ├── futuristic_audit_log_card.dart
│   │           ├── futuristic_audit_logs_table.dart
│   │           ├── audit_log_details_dialog.dart
│   │           ├── audit_log_filters_widget.dart
│   │           ├── audit_log_timeline_widget.dart
│   │           ├── activity_chart_widget.dart
│   │           └── audit_log_stats_card.dart
│   │ 
│   │ 
│   │ 
│   ├── admin_users/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── users_local_datasource.dart
│   │   │   │   └── users_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── user_model.dart
│   │   │   │   ├── user_details_model.dart
│   │   │   │   └── user_lifetime_stats_model.dart
│   │   │   └── repositories/
│   │   │       └── users_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── user.dart
│   │   │   │   ├── user_details.dart
│   │   │   │   └── user_lifetime_stats.dart
│   │   │   ├── repositories/
│   │   │   │   └── users_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_all_users_usecase.dart
│   │   │       ├── get_user_details_usecase.dart
│   │   │       ├── create_user_usecase.dart
│   │   │       ├── update_user_usecase.dart
│   │   │       ├── activate_user_usecase.dart
│   │   │       ├── deactivate_user_usecase.dart
│   │   │       ├── assign_role_usecase.dart
│   │   │       └── get_user_lifetime_stats_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── users_list/
│   │       │   │   ├── users_list_bloc.dart
│   │       │   │   ├── users_list_event.dart
│   │       │   │   └── users_list_state.dart
│   │       │   └── user_details/
│   │       │       ├── user_details_bloc.dart
│   │       │       ├── user_details_event.dart
│   │       │       └── user_details_state.dart
│   │       ├── pages/
│   │       │   ├── users_list_page.dart
│   │       │   ├── user_details_page.dart
│   │       │   └── create_user_page.dart
│   │       └── widgets/
│   │           ├── futuristic_user_card.dart
│   │           ├── futuristic_users_table.dart
│   │           ├── user_filters_widget.dart
│   │           ├── user_stats_card.dart
│   │           ├── user_form_dialog.dart
│   │           └── user_role_selector.dart
│   │
│   │
│   │
│   ├── admin_cities/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── cities_local_datasource.dart
│   │   │   │   └── cities_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── city_model.dart
│   │   │   └── repositories/
│   │   │       └── cities_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── city.dart
│   │   │   ├── repositories/
│   │   │   │   └── cities_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_cities_usecase.dart
│   │   │       ├── create_city_usecase.dart
│   │   │       ├── delete_city_image_usecase.dart
│   │   │       ├── upload_city_image_usecase.dart
│   │   │       ├── save_cities_usecase.dart
│   │   │       ├── get_cities_statistics_usecase.dart
│   │   │       ├── update_city_usecase.dart
│   │   │       ├── delete_city_usecase.dart
│   │   │       └── search_cities_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── cities_bloc.dart
│   │       │   ├── cities_event.dart
│   │       │   └── cities_state.dart
│   │       ├── pages/
│   │       │   └── admin_cities_page.dart
│   │       └── widgets/
│   │           ├── futuristic_city_card.dart
│   │           ├── futuristic_cities_grid.dart
│   │           ├── city_form_modal.dart
│   │           ├── city_stats_card.dart
│   │           ├── city_search_bar.dart
│   │           └── city_image_gallery.dart
│   │
│   │
│   │
│   ├── admin_currencies/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── currencies_local_datasource.dart
│   │   │   │   └── currencies_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── currency_model.dart
│   │   │   └── repositories/
│   │   │       └── currencies_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── currency.dart
│   │   │   ├── repositories/
│   │   │   │   └── currencies_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_currencies_usecase.dart
│   │   │       ├── save_currencies_usecase.dart
│   │   │       ├── delete_currency_usecase.dart
│   │   │       └── set_default_currency_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── currencies_bloc.dart
│   │       │   ├── currencies_event.dart
│   │       │   └── currencies_state.dart
│   │       ├── pages/
│   │       │   └── currencies_management_page.dart
│   │       └── widgets/
│   │           ├── futuristic_currency_card.dart
│   │           ├── futuristic_currency_form_modal.dart
│   │           ├── currency_stats_card.dart
│   │           └── exchange_rate_indicator.dart
│   │
│   │
│   │
│   ├── admin_availability_pricing/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── availability_remote_datasource.dart
│   │   │   │   ├── pricing_remote_datasource.dart
│   │   │   │   └── availability_local_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── availability_model.dart
│   │   │   │   ├── pricing_model.dart
│   │   │   │   ├── unit_availability_model.dart
│   │   │   │   ├── pricing_rule_model.dart
│   │   │   │   ├── booking_conflict_model.dart
│   │   │   │   └── seasonal_pricing_model.dart
│   │   │   └── repositories/
│   │   │       ├── availability_repository_impl.dart
│   │   │       └── pricing_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── availability.dart
│   │   │   │   ├── pricing.dart
│   │   │   │   ├── unit_availability.dart
│   │   │   │   ├── pricing_rule.dart
│   │   │   │   ├── booking_conflict.dart
│   │   │   │   └── seasonal_pricing.dart
│   │   │   ├── repositories/
│   │   │   │   ├── availability_repository.dart
│   │   │   │   └── pricing_repository.dart
│   │   │   └── usecases/
│   │   │       ├── availability/
│   │   │       │   ├── get_monthly_availability_usecase.dart
│   │   │       │   ├── update_availability_usecase.dart
│   │   │       │   ├── bulk_update_availability_usecase.dart
│   │   │       │   ├── clone_availability_usecase.dart
│   │   │       │   ├── check_availability_usecase.dart
│   │   │       │   └── delete_availability_usecase.dart
│   │   │       └── pricing/
│   │   │           ├── get_monthly_pricing_usecase.dart
│   │   │           ├── update_pricing_usecase.dart
│   │   │           ├── bulk_update_pricing_usecase.dart
│   │   │           ├── copy_pricing_usecase.dart
│   │   │           ├── apply_seasonal_pricing_usecase.dart
│   │   │           └── delete_pricing_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── availability/
│   │       │   │   ├── availability_bloc.dart
│   │       │   │   ├── availability_event.dart
│   │       │   │   └── availability_state.dart
│   │       │   └── pricing/
│   │       │       ├── pricing_bloc.dart
│   │       │       ├── pricing_event.dart
│   │       │       └── pricing_state.dart
│   │       ├── pages/
│   │       │   └── availability_pricing_page.dart
│   │       └── widgets/
│   │           ├── futuristic_calendar_view.dart
│   │           ├── availability_calendar_grid.dart
│   │           ├── pricing_calendar_grid.dart
│   │           ├── unit_selector_card.dart
│   │           ├── availability_status_legend.dart
│   │           ├── pricing_tier_legend.dart
│   │           ├── bulk_update_dialog.dart
│   │           ├── seasonal_pricing_dialog.dart
│   │           ├── conflict_resolution_dialog.dart
│   │           ├── stats_dashboard_card.dart
│   │           └── quick_actions_panel.dart
│   │
│   │
│   │
│   ├── admin_bookings/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── bookings_local_datasource.dart
│   │   │   │   └── bookings_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── booking_model.dart
│   │   │   │   ├── booking_details_model.dart
│   │   │   │   ├── booking_status_model.dart
│   │   │   │   ├── booking_report_model.dart
│   │   │   │   ├── booking_trends_model.dart
│   │   │   │   └── booking_window_analysis_model.dart
│   │   │   └── repositories/
│   │   │       └── bookings_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── booking.dart
│   │   │   │   ├── booking_details.dart
│   │   │   │   ├── booking_report.dart
│   │   │   │   ├── booking_trends.dart
│   │   │   │   └── booking_window_analysis.dart
│   │   │   ├── repositories/
│   │   │   │   └── bookings_repository.dart
│   │   │   └── usecases/
│   │   │       ├── bookings/
│   │   │       │   ├── cancel_booking_usecase.dart
│   │   │       │   ├── update_booking_usecase.dart
│   │   │       │   ├── confirm_booking_usecase.dart
│   │   │       │   ├── get_booking_by_id_usecase.dart
│   │   │       │   ├── get_bookings_by_date_range_usecase.dart
│   │   │       │   ├── get_bookings_by_property_usecase.dart
│   │   │       │   ├── get_bookings_by_status_usecase.dart
│   │   │       │   ├── get_bookings_by_unit_usecase.dart
│   │   │       │   ├── get_bookings_by_user_usecase.dart
│   │   │       │   ├── check_in_usecase.dart
│   │   │       │   ├── check_out_usecase.dart
│   │   │       │   └── complete_booking_usecase.dart
│   │   │       ├── services/
│   │   │       │   ├── add_service_to_booking_usecase.dart
│   │   │       │   ├── remove_service_from_booking_usecase.dart
│   │   │       │   └── get_booking_services_usecase.dart
│   │   │       └── reports/
│   │   │           ├── get_booking_report_usecase.dart
│   │   │           ├── get_booking_trends_usecase.dart
│   │   │           └── get_booking_window_analysis_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── bookings_list/
│   │       │   │   ├── bookings_list_bloc.dart
│   │       │   │   ├── bookings_list_event.dart
│   │       │   │   └── bookings_list_state.dart
│   │       │   ├── booking_details/
│   │       │   │   ├── booking_details_bloc.dart
│   │       │   │   ├── booking_details_event.dart
│   │       │   │   └── booking_details_state.dart
│   │       │   ├── booking_calendar/
│   │       │   │   ├── booking_calendar_bloc.dart
│   │       │   │   ├── booking_calendar_event.dart
│   │       │   │   └── booking_calendar_state.dart
│   │       │   └── booking_analytics/
│   │       │       ├── booking_analytics_bloc.dart
│   │       │       ├── booking_analytics_event.dart
│   │       │       └── booking_analytics_state.dart
│   │       ├── pages/
│   │       │   ├── bookings_list_page.dart
│   │       │   ├── booking_details_page.dart
│   │       │   ├── booking_calendar_page.dart
│   │       │   ├── booking_analytics_page.dart
│   │       │   └── booking_timeline_page.dart
│   │       └── widgets/
│   │           ├── futuristic_booking_card.dart
│   │           ├── futuristic_bookings_table.dart
│   │           ├── booking_status_badge.dart
│   │           ├── booking_filters_widget.dart
│   │           ├── booking_calendar_widget.dart
│   │           ├── booking_timeline_widget.dart
│   │           ├── booking_stats_cards.dart
│   │           ├── booking_actions_dialog.dart
│   │           ├── check_in_out_dialog.dart
│   │           ├── booking_services_widget.dart
│   │           ├── booking_payment_summary.dart
│   │           └── booking_analytics_charts.dart
│   │
│   │
│   ├── admin_payments/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── payments_local_datasource.dart
│   │   │   │   └── payments_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── payment_model.dart
│   │   │   │   ├── payment_details_model.dart
│   │   │   │   ├── payment_method_model.dart
│   │   │   │   ├── payment_status_model.dart
│   │   │   │   ├── money_model.dart
│   │   │   │   ├── refund_model.dart
│   │   │   │   └── payment_analytics_model.dart
│   │   │   └── repositories/
│   │   │       └── payments_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── payment.dart
│   │   │   │   ├── payment_details.dart
│   │   │   │   ├── money.dart
│   │   │   │   ├── refund.dart
│   │   │   │   └── payment_analytics.dart
│   │   │   ├── repositories/
│   │   │   │   └── payments_repository.dart
│   │   │   └── usecases/
│   │   │       ├── payments/
│   │   │       │   ├── refund_payment_usecase.dart
│   │   │       │   ├── void_payment_usecase.dart
│   │   │       │   ├── update_payment_status_usecase.dart
│   │   │       │   ├── process_payment_usecase.dart
│   │   │       │   ├── get_payment_by_id_usecase.dart
│   │   │       │   └── get_all_payments_usecase.dart
│   │   │       ├── queries/
│   │   │       │   ├── get_payments_by_booking_usecase.dart
│   │   │       │   ├── get_payments_by_status_usecase.dart
│   │   │       │   ├── get_payments_by_user_usecase.dart
│   │   │       │   ├── get_payments_by_method_usecase.dart
│   │   │       │   └── get_payments_by_property_usecase.dart
│   │   │       └── analytics/
│   │   │           ├── get_payment_analytics_usecase.dart
│   │   │           ├── get_revenue_report_usecase.dart
│   │   │           ├── get_payment_trends_usecase.dart
│   │   │           └── get_refund_statistics_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── payments_list/
│   │       │   │   ├── payments_list_bloc.dart
│   │       │   │   ├── payments_list_event.dart
│   │       │   │   └── payments_list_state.dart
│   │       │   ├── payment_details/
│   │       │   │   ├── payment_details_bloc.dart
│   │       │   │   ├── payment_details_event.dart
│   │       │   │   └── payment_details_state.dart
│   │       │   ├── payment_refund/
│   │       │   │   ├── payment_refund_bloc.dart
│   │       │   │   ├── payment_refund_event.dart
│   │       │   │   └── payment_refund_state.dart
│   │       │   └── payment_analytics/
│   │       │       ├── payment_analytics_bloc.dart
│   │       │       ├── payment_analytics_event.dart
│   │       │       └── payment_analytics_state.dart
│   │       ├── pages/
│   │       │   ├── payments_list_page.dart
│   │       │   ├── payment_details_page.dart
│   │       │   ├── payment_analytics_page.dart
│   │       │   ├── refunds_management_page.dart
│   │       │   └── revenue_dashboard_page.dart
│   │       └── widgets/
│   │           ├── futuristic_payment_card.dart
│   │           ├── futuristic_payments_table.dart
│   │           ├── payment_status_indicator.dart
│   │           ├── payment_method_icon.dart
│   │           ├── payment_filters_widget.dart
│   │           ├── refund_dialog.dart
│   │           ├── void_payment_dialog.dart
│   │           ├── payment_timeline_widget.dart
│   │           ├── payment_stats_cards.dart
│   │           ├── revenue_chart_widget.dart
│   │           ├── payment_trends_graph.dart
│   │           ├── payment_breakdown_pie_chart.dart
│   │           └── transaction_details_card.dart
│   │
│   │
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
│   │
│   │
│   └── home/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── home_remote_datasource.dart
│   │   │   │   └── home_local_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── dashboard_stats_model.dart
│   │   │   │   ├── recent_activity_model.dart
│   │   │   │   ├── quick_action_model.dart
│   │   │   │   └── analytics_data_model.dart
│   │   │   └── repositories/
│   │   │       └── home_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── dashboard_stats.dart
│   │   │   │   ├── recent_activity.dart
│   │   │   │   ├── quick_action.dart
│   │   │   │   └── analytics_data.dart
│   │   │   ├── repositories/
│   │   │   │   └── home_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_dashboard_stats_usecase.dart
│   │   │       ├── get_recent_activities_usecase.dart
│   │   │       ├── get_quick_actions_usecase.dart
│   │   │       └── get_analytics_data_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── home_bloc.dart
│   │       │   ├── home_event.dart
│   │       │   └── home_state.dart
│   │       ├── pages/
│   │       │   └── admin_home_page.dart
│   │       └── widgets/
│   │           ├── futuristic_stats_grid.dart
│   │           ├── recent_activities_timeline.dart
│   │           ├── quick_actions_panel.dart
│   │           ├── performance_chart_card.dart
│   │           ├── activity_heatmap_widget.dart
│   │           └── animated_counter_widget.dart
│   │
│   │
│   │
│   └── helpers/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── helpers_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── search_result_model.dart
│   │   │   └── repositories/
│   │   │       └── helpers_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── search_result.dart
│   │   │   ├── repositories/
│   │   │   │   └── helpers_repository.dart
│   │   │   └── usecases/
│   │   │       ├── search_users_usecase.dart
│   │   │       ├── search_properties_usecase.dart
│   │   │       ├── search_units_usecase.dart
│   │   │       ├── search_cities_usecase.dart
│   │   │       └── search_bookings_usecase.dart
│   │   └── presentation/
│   │       ├── pages/
│   │       │   ├── user_search_page.dart
│   │       │   ├── property_search_page.dart
│   │       │   ├── unit_search_page.dart
│   │       │   ├── city_search_page.dart
│   │       │   └── booking_search_page.dart
│   │       ├── utils/
│   │       │   └── search_navigation_helper.dart
│   │       └── widgets/
│   │           ├── search_header.dart
│   │           ├── search_item_card.dart
│   │           └── simple_filter_bar.dart
│   │
│   │
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
│
│   ├── admin_notifications/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── admin_notifications_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── admin_notification_model.dart
│   │   │   └── repositories/
│   │   │       └── admin_notifications_repository_impl.dart
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── admin_notification.dart
│   │   │   ├── repositories/
│   │   │   │   └── admin_notifications_repository.dart
│   │   │   └── usecases/
│   │   │       ├── create_notification_usecase.dart
│   │   │       ├── broadcast_notification_usecase.dart
│   │   │       ├── delete_notification_usecase.dart
│   │   │       ├── resend_notification_usecase.dart
│   │   │       ├── get_system_notifications_usecase.dart
│   │   │       ├── get_user_notifications_usecase.dart
│   │   │       └── get_notifications_stats_usecase.dart
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── admin_notifications_bloc.dart
│   │       │   ├── admin_notifications_event.dart
│   │       │   └── admin_notifications_state.dart
│   │       ├── pages/
│   │       │   ├── admin_notifications_page.dart
│   │       │   ├── create_admin_notification_page.dart
│   │       │   └── user_notifications_page.dart
│   │       └── widgets/
│   │           ├── notification_filters_bar.dart
│   │           ├── notifications_stats_card.dart
│   │           ├── admin_notification_form.dart
│   │           └── admin_notifications_table.dart
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
│   ├─ websocket_service.dart
│   ├── section_service.dart              # للعمليات الأساسية
│   └── section_content_service.dart      # لإدارة المحتوى

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
