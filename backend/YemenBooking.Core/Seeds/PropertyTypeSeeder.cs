using System;
using System.Collections.Generic;
using Bogus;
using YemenBooking.Core.Entities;

namespace YemenBooking.Core.Seeds
{
    /// <summary>
    /// مولد البيانات الأولية لكائن PropertyType
    /// </summary>
    public class PropertyTypeSeeder : ISeeder<PropertyType>
    {
        public IEnumerable<PropertyType> SeedData()
        {
            return new Faker<PropertyType>()
                .RuleFor(pt => pt.Id, f => Guid.NewGuid())
                .RuleFor(pt => pt.Name, f => f.PickRandom(new[] { "Hotel", "Chalet", "Rest House", "Villa", "Apartment" }))
                .RuleFor(pt => pt.Description, f => f.Lorem.Sentence())
                .RuleFor(pt => pt.DefaultAmenities, f => "[]")
                .Generate(5);
        }
    }
} 