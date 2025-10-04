# 📊 أمثلة ملفات JSON للفهرسة المسبقة

## 🏙️ 1. فهرس المدن (City Index)

### `IndexFiles/Properties/cities/sanaa.json`
```json
{
  "metadata": {
    "city": "sanaa",
    "cityId": "3b2c1a0d-4e5f-6789-abcd-123456789abc",
    "totalProperties": 1847,
    "lastUpdated": "2024-01-15T14:30:00Z",
    "dataVersion": "1.2.5",
    "compressionLevel": "optimal"
  },
  "statistics": {
    "averagePrice": 285.50,
    "minPrice": 45.00,
    "maxPrice": 2500.00,
    "popularAmenities": ["wifi", "parking", "ac"],
    "averageRating": 4.3,
    "totalReviews": 12453
  },
  "priceRanges": {
    "0-100": {
      "count": 234,
      "propertyIds": [3, 15, 28, 45, 67, 89, 156, 178, 234, 267]
    },
    "100-300": {
      "count": 892,
      "propertyIds": [1, 7, 12, 33, 47, 89, 134, 167, 245, 289]
    },
    "300-500": {
      "count": 456,
      "propertyIds": [2, 9, 23, 56, 78, 123, 178, 234, 345, 456]
    },
    "500-1000": {
      "count": 189,
      "propertyIds": [5, 18, 34, 67, 89, 145, 189, 267, 345, 423]
    },
    "1000+": {
      "count": 76,
      "propertyIds": [11, 29, 47, 83, 129, 167, 234, 289, 367, 445]
    }
  },
  "amenityIntersections": {
    "wifi": {
      "count": 1456,
      "propertyIds": [1, 3, 5, 7, 9, 11, 13, 15, 17, 19]
    },
    "pool": {
      "count": 423,
      "propertyIds": [2, 8, 14, 26, 38, 52, 68, 84, 102, 118]
    },
    "parking": {
      "count": 1234,
      "propertyIds": [1, 4, 7, 10, 13, 16, 19, 22, 25, 28]
    },
    "gym": {
      "count": 289,
      "propertyIds": [5, 15, 25, 35, 45, 55, 65, 75, 85, 95]
    }
  },
  "propertyTypeDistribution": {
    "hotel": {
      "count": 567,
      "averagePrice": 345.00,
      "propertyIds": [12, 34, 56, 78, 90, 112, 134, 156, 178, 200]
    },
    "apartment": {
      "count": 823,
      "averagePrice": 185.50,
      "propertyIds": [23, 45, 67, 89, 111, 133, 155, 177, 199, 221]
    },
    "villa": {
      "count": 234,
      "averagePrice": 650.00,
      "propertyIds": [34, 56, 78, 100, 122, 144, 166, 188, 210, 232]
    },
    "resort": {
      "count": 123,
      "averagePrice": 890.00,
      "propertyIds": [45, 67, 89, 111, 133, 155, 177, 199, 221, 243]
    }
  },
  "availabilityCalendar": {
    "2024-01": {
      "availableCount": 1234,
      "bookedCount": 613,
      "availableIds": [1, 3, 5, 7, 9, 11, 13, 15, 17, 19]
    },
    "2024-02": {
      "availableCount": 1345,
      "bookedCount": 502,
      "availableIds": [2, 4, 6, 8, 10, 12, 14, 16, 18, 20]
    }
  }
}
```

## 💰 2. فهرس الأسعار (Price Range Index)

### `IndexFiles/Properties/price-ranges/range_100_500.json`
```json
{
  "metadata": {
    "priceRange": "100-500",
    "currency": "SAR",
    "totalProperties": 2341,
    "lastUpdated": "2024-01-15T14:30:00Z",
    "indexVersion": "2.1.0"
  },
  "cityBreakdown": {
    "sanaa": {
      "count": 892,
      "averagePrice": 285.50,
      "propertyIds": [1, 7, 12, 33, 47, 89, 134, 167, 245, 289]
    },
    "jeddah": {
      "count": 734,
      "averagePrice": 315.75,
      "propertyIds": [5, 13, 21, 37, 53, 69, 85, 101, 117, 133]
    },
    "dammam": {
      "count": 456,
      "averagePrice": 245.25,
      "propertyIds": [3, 9, 15, 27, 39, 51, 63, 75, 87, 99]
    },
    "makkah": {
      "count": 259,
      "averagePrice": 375.00,
      "propertyIds": [7, 17, 29, 41, 53, 65, 77, 89, 101, 113]
    }
  },
  "propertyTypeIntersection": {
    "hotel": {
      "count": 678,
      "propertyIds": [12, 24, 36, 48, 60, 72, 84, 96, 108, 120]
    },
    "apartment": {
      "count": 1234,
      "propertyIds": [11, 23, 35, 47, 59, 71, 83, 95, 107, 119]
    },
    "villa": {
      "count": 345,
      "propertyIds": [13, 25, 37, 49, 61, 73, 85, 97, 109, 121]
    },
    "resort": {
      "count": 84,
      "propertyIds": [15, 27, 39, 51, 63, 75, 87, 99, 111, 123]
    }
  },
  "amenityIntersection": {
    "wifi": {
      "count": 2198,
      "propertyIds": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    },
    "pool": {
      "count": 567,
      "propertyIds": [2, 6, 10, 14, 18, 22, 26, 30, 34, 38]
    },
    "parking": {
      "count": 1876,
      "propertyIds": [1, 3, 5, 7, 9, 11, 13, 15, 17, 19]
    }
  },
  "seasonalPricing": {
    "peak": {
      "months": ["12", "01", "02"],
      "averageIncrease": 25.5,
      "affectedProperties": 1876
    },
    "off": {
      "months": ["06", "07", "08"],
      "averageDecrease": 15.2,
      "affectedProperties": 2103
    }
  }
}
```

## 🏊‍♂️ 3. فهرس المرافق (Amenities Index)

### `IndexFiles/Properties/amenities/pool.json`
```json
{
  "metadata": {
    "amenityId": "pool-001",
    "amenityName": "Swimming Pool",
    "amenityNameAr": "مسبح",
    "category": "recreation",
    "totalProperties": 1234,
    "lastUpdated": "2024-01-15T14:30:00Z"
  },
  "cityDistribution": {
    "sanaa": {
      "count": 423,
      "averagePrice": 425.00,
      "popularWith": ["families", "tourists"],
      "propertyIds": [2, 8, 14, 26, 38, 52, 68, 84, 102, 118]
    },
    "jeddah": {
      "count": 345,
      "averagePrice": 485.50,
      "popularWith": ["tourists", "business"],
      "propertyIds": [4, 12, 20, 28, 36, 44, 52, 60, 68, 76]
    },
    "dammam": {
      "count": 234,
      "averagePrice": 365.75,
      "popularWith": ["families"],
      "propertyIds": [6, 14, 22, 30, 38, 46, 54, 62, 70, 78]
    }
  },
  "priceRangeIntersection": {
    "100-300": {
      "count": 156,
      "propertyIds": [12, 28, 44, 60, 76, 92, 108, 124, 140, 156]
    },
    "300-500": {
      "count": 487,
      "propertyIds": [15, 31, 47, 63, 79, 95, 111, 127, 143, 159]
    },
    "500-1000": {
      "count": 423,
      "propertyIds": [18, 34, 50, 66, 82, 98, 114, 130, 146, 162]
    },
    "1000+": {
      "count": 168,
      "propertyIds": [21, 37, 53, 69, 85, 101, 117, 133, 149, 165]
    }
  },
  "propertyTypeIntersection": {
    "hotel": {
      "count": 567,
      "averageRating": 4.5,
      "propertyIds": [25, 50, 75, 100, 125, 150, 175, 200, 225, 250]
    },
    "resort": {
      "count": 289,
      "averageRating": 4.8,
      "propertyIds": [30, 60, 90, 120, 150, 180, 210, 240, 270, 300]
    },
    "villa": {
      "count": 234,
      "averageRating": 4.3,
      "propertyIds": [35, 70, 105, 140, 175, 210, 245, 280, 315, 350]
    },
    "apartment": {
      "count": 144,
      "averageRating": 4.1,
      "propertyIds": [40, 80, 120, 160, 200, 240, 280, 320, 360, 400]
    }
  },
  "relatedAmenities": {
    "poolBar": {
      "count": 234,
      "correlation": 0.85,
      "propertyIds": [25, 45, 65, 85, 105, 125, 145, 165, 185, 205]
    },
    "poolside": {
      "count": 567,
      "correlation": 0.92,
      "propertyIds": [30, 50, 70, 90, 110, 130, 150, 170, 190, 210]
    },
    "kidPool": {
      "count": 345,
      "correlation": 0.78,
      "propertyIds": [35, 55, 75, 95, 115, 135, 155, 175, 195, 215]
    }
  },
  "seasonalAvailability": {
    "summer": {
      "availabilityRate": 0.95,
      "popularityBoost": 1.8
    },
    "winter": {
      "availabilityRate": 0.75,
      "popularityBoost": 1.2
    }
  }
}
```

## 🏨 4. فهرس أنواع الكيانات (Property Types Index)

### `IndexFiles/Properties/property-types/hotel.json`
```json
{
  "metadata": {
    "propertyTypeId": "hotel-type-001",
    "typeName": "Hotel",
    "typeNameAr": "فندق",
    "totalProperties": 1567,
    "lastUpdated": "2024-01-15T14:30:00Z"
  },
  "starRatingDistribution": {
    "5-star": {
      "count": 234,
      "averagePrice": 850.00,
      "averageRating": 4.7,
      "propertyIds": [101, 201, 301, 401, 501, 601, 701, 801, 901, 1001]
    },
    "4-star": {
      "count": 456,
      "averagePrice": 485.50,
      "averageRating": 4.3,
      "propertyIds": [102, 202, 302, 402, 502, 602, 702, 802, 902, 1002]
    },
    "3-star": {
      "count": 567,
      "averagePrice": 285.75,
      "averageRating": 3.9,
      "propertyIds": [103, 203, 303, 403, 503, 603, 703, 803, 903, 1003]
    },
    "2-star": {
      "count": 234,
      "averagePrice": 165.25,
      "averageRating": 3.4,
      "propertyIds": [104, 204, 304, 404, 504, 604, 704, 804, 904, 1004]
    },
    "1-star": {
      "count": 76,
      "averagePrice": 95.00,
      "averageRating": 2.8,
      "propertyIds": [105, 205, 305, 405, 505, 605, 705, 805, 905, 1005]
    }
  },
  "cityDistribution": {
    "sanaa": {
      "count": 567,
      "averagePrice": 345.00,
      "topAmenities": ["wifi", "restaurant", "spa"],
      "propertyIds": [12, 34, 56, 78, 90, 112, 134, 156, 178, 200]
    },
    "jeddah": {
      "count": 423,
      "averagePrice": 425.50,
      "topAmenities": ["pool", "beach", "wifi"],
      "propertyIds": [15, 37, 59, 81, 103, 125, 147, 169, 191, 213]
    },
    "makkah": {
      "count": 289,
      "averagePrice": 485.75,
      "topAmenities": ["prayer_room", "halal_food", "wifi"],
      "propertyIds": [18, 40, 62, 84, 106, 128, 150, 172, 194, 216]
    }
  },
  "commonAmenities": {
    "wifi": {
      "availability": 0.98,
      "count": 1536,
      "propertyIds": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    },
    "restaurant": {
      "availability": 0.85,
      "count": 1332,
      "propertyIds": [2, 4, 6, 8, 10, 12, 14, 16, 18, 20]
    },
    "room_service": {
      "availability": 0.75,
      "count": 1175,
      "propertyIds": [3, 6, 9, 12, 15, 18, 21, 24, 27, 30]
    },
    "concierge": {
      "availability": 0.65,
      "count": 1019,
      "propertyIds": [4, 8, 12, 16, 20, 24, 28, 32, 36, 40]
    }
  },
  "unitTypeBreakdown": {
    "standard_room": {
      "count": 4567,
      "averageCapacity": 2,
      "averagePrice": 245.00,
      "properties": 1234
    },
    "deluxe_room": {
      "count": 2345,
      "averageCapacity": 2,
      "averagePrice": 345.00,
      "properties": 867
    },
    "suite": {
      "count": 1234,
      "averageCapacity": 4,
      "averagePrice": 585.00,
      "properties": 567
    },
    "executive_suite": {
      "count": 567,
      "averageCapacity": 6,
      "averagePrice": 850.00,
      "properties": 234
    }
  }
}
```

## 🗓️ 5. فهرس التوفر (Availability Index)

### `IndexFiles/Properties/availability/2024-02.json`
```json
{
  "metadata": {
    "month": "2024-02",
    "totalProperties": 5432,
    "totalUnits": 23456,
    "lastUpdated": "2024-01-15T14:30:00Z",
    "occupancyRate": 0.67
  },
  "dailyAvailability": {
    "2024-02-01": {
      "availableProperties": 1876,
      "availableUnits": 8234,
      "averagePrice": 285.50,
      "propertyIds": [1, 3, 5, 7, 9, 11, 13, 15, 17, 19]
    },
    "2024-02-14": {
      "availableProperties": 1234,
      "availableUnits": 5678,
      "averagePrice": 385.75,
      "propertyIds": [2, 6, 10, 14, 18, 22, 26, 30, 34, 38]
    },
    "2024-02-29": {
      "availableProperties": 2145,
      "availableUnits": 9876,
      "averagePrice": 245.25,
      "propertyIds": [4, 8, 12, 16, 20, 24, 28, 32, 36, 40]
    }
  },
  "weekendAvailability": {
    "weekends": {
      "averageAvailable": 1456,
      "priceIncrease": 1.25,
      "popularCities": ["sanaa", "jeddah"]
    },
    "weekdays": {
      "averageAvailable": 2234,
      "priceDecrease": 0.85,
      "popularCities": ["dammam", "khobar"]
    }
  },
  "priceRangeAvailability": {
    "0-100": {
      "availableCount": 345,
      "occupancyRate": 0.45,
      "propertyIds": [23, 45, 67, 89, 111, 133, 155, 177, 199, 221]
    },
    "100-300": {
      "availableCount": 1234,
      "occupancyRate": 0.65,
      "propertyIds": [34, 56, 78, 100, 122, 144, 166, 188, 210, 232]
    },
    "300-500": {
      "availableCount": 876,
      "occupancyRate": 0.75,
      "propertyIds": [45, 67, 89, 111, 133, 155, 177, 199, 221, 243]
    },
    "500+": {
      "availableCount": 421,
      "occupancyRate": 0.85,
      "propertyIds": [56, 78, 100, 122, 144, 166, 188, 210, 232, 254]
    }
  }
}
```

## 🔍 6. فهرس البحث النصي (Text Search Trie)

### `IndexFiles/Properties/text-search/trie_structure.json`
```json
{
  "metadata": {
    "totalWords": 12345,
    "totalProperties": 5432,
    "lastUpdated": "2024-01-15T14:30:00Z",
    "language": "ar_SA"
  },
  "trie": {
    "ف": {
      "ن": {
        "د": {
          "ق": {
            "isComplete": true,
            "propertyIds": [12, 34, 56, 78, 90],
            "weight": 0.95
          }
        }
      }
    },
    "ر": {
      "ي": {
        "ا": {
          "ض": {
            "isComplete": true,
            "propertyIds": [1, 15, 33, 67, 89],
            "weight": 0.88
          }
        }
      }
    },
    "ج": {
      "د": {
        "ة": {
          "isComplete": true,
          "propertyIds": [5, 23, 41, 59, 77],
          "weight": 0.82
        }
      }
    }
  },
  "popularTerms": {
    "فندق": {
      "frequency": 1567,
      "propertyIds": [12, 34, 56, 78, 90, 112, 134, 156, 178, 200]
    },
    "شقة": {
      "frequency": 2345,
      "propertyIds": [23, 45, 67, 89, 111, 133, 155, 177, 199, 221]
    },
    "فيلا": {
      "frequency": 876,
      "propertyIds": [34, 56, 78, 100, 122, 144, 166, 188, 210, 232]
    },
    "منتجع": {
      "frequency": 234,
      "propertyIds": [45, 67, 89, 111, 133, 155, 177, 199, 221, 243]
    }
  },
  "synonyms": {
    "فندق": ["نزل", "بيت_ضيافة", "لوكاندة"],
    "شقة": ["وحدة_سكنية", "استوديو", "دوبلكس"],
    "فيلا": ["بيت", "منزل", "قصر"],
    "منتجع": ["ريزورت", "مجمع_سياحي"]
  }
}
```

## 🎯 7. فهرس التقييمات (Ratings Index)

### `IndexFiles/Properties/ratings/high_rated.json`
```json
{
  "metadata": {
    "ratingRange": "4.0-5.0",
    "totalProperties": 2341,
    "averageRating": 4.45,
    "totalReviews": 45678,
    "lastUpdated": "2024-01-15T14:30:00Z"
  },
  "ratingBrackets": {
    "4.8-5.0": {
      "count": 234,
      "averagePrice": 565.75,
      "propertyIds": [101, 201, 301, 401, 501, 601, 701, 801, 901, 1001]
    },
    "4.5-4.7": {
      "count": 567,
      "averagePrice": 425.50,
      "propertyIds": [102, 202, 302, 402, 502, 602, 702, 802, 902, 1002]
    },
    "4.2-4.4": {
      "count": 876,
      "averagePrice": 335.25,
      "propertyIds": [103, 203, 303, 403, 503, 603, 703, 803, 903, 1003]
    },
    "4.0-4.1": {
      "count": 664,
      "averagePrice": 285.00,
      "propertyIds": [104, 204, 304, 404, 504, 604, 704, 804, 904, 1004]
    }
  },
  "reviewCategories": {
    "cleanliness": {
      "averageScore": 4.6,
      "topProperties": [101, 201, 301, 401, 501]
    },
    "service": {
      "averageScore": 4.4,
      "topProperties": [102, 202, 302, 402, 502]
    },
    "location": {
      "averageScore": 4.3,
      "topProperties": [103, 203, 303, 403, 503]
    },
    "value": {
      "averageScore": 4.2,
      "topProperties": [104, 204, 304, 404, 504]
    }
  }
}
```