# Property Module API Alignment

This document summarizes the updates performed to ensure 100% compatibility between the Flutter mobile property module and the backend APIs.

## Models

- **AmenityModel**
  - Supports both `amenityId` and `id` for identification.
  - Recognizes `isAvailable` and falls back to `isActive`.
  - Handles missing `createdAt` gracefully.

- **PropertyDetailModel**
  - Falls back to nested `propertyType` object for `typeId` and `typeName` when top-level keys are absent.

- **UnitModel**
  - Parses `basePrice`, `customFeatures`, `pricingMethod`, and `distanceKm` correctly.
  - Maps nested `fieldValues` and `dynamicFields` arrays.

- **ReviewModel**
  - Handles optional `responseText`, `responseDate`, and `images` fields.

## Remote Data Source & Repository

- **PropertyRemoteDataSourceImpl**
  - Extracts `data.units` from the API payload for `getPropertyUnits`.
  - Reads boolean results (`addToFavorites`, `removeFromFavorites`, `updateViewCount`) from the standard `data` key.

- **PropertyRepositoryImpl**
  - (If present) Should wrap the remote data source and convert `ResultDto` to domain models.

## Testing

- Added **property_model_test.dart** under the `test/` directory to verify:
  - Correct JSON parsing and `toJson` round-trip for all models.

---
*Generated on 2025-08-06*