using System.Collections.Generic;
using Bogus;
using YemenBooking.Core.Entities;

namespace YemenBooking.Core.Seeds
{
    /// <summary>
    /// مولد البيانات الأولية لكائن Amenity
    /// </summary>
    public class AmenitySeeder : ISeeder<Amenity>
    {
        public IEnumerable<Amenity> SeedData()
        {
            return new Faker<Amenity>()
                .RuleFor(a => a.Id, f => f.Random.Guid()) // سيتم ضبط EF لإصدار Id تلقائياً
                .RuleFor(a => a.Name, f => f.Commerce.ProductName())
                .RuleFor(a => a.Description, f => f.Lorem.Sentence())
                .Generate(10);
        }
    }
} 