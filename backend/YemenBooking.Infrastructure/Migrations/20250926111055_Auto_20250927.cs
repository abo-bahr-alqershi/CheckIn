using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20250927 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectionItems");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImage",
                table: "Sections",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BackgroundImageId",
                table: "Sections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "Sections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorTheme",
                table: "Sections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColumnsCount",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContentType",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Sections",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayStyle",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Sections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilterCriteria",
                table: "Sections",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Sections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleToGuests",
                table: "Sections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleToRegistered",
                table: "Sections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ItemsToShow",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPrice",
                table: "Sections",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Sections",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinPrice",
                table: "Sections",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinRating",
                table: "Sections",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Sections",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PropertyTypeId",
                table: "Sections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiresPermission",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Sections",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SortCriteria",
                table: "Sections",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Sections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "Sections",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Sections",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitTypeId",
                table: "Sections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PropertyInSectionId",
                table: "PropertyImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "PropertyImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitInSectionId",
                table: "PropertyImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PropertyInSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StarRating = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ReviewsCount = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MainImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromotionalText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Badge = table.Column<int>(type: "int", nullable: true),
                    BadgeColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ViewsFromSection = table.Column<int>(type: "int", nullable: false),
                    ClickCount = table.Column<int>(type: "int", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Metadata = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyInSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyInSections_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyInSections_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TempKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Sizes = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    IsMainImage = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "image"),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    VideoThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionImages_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UnitInSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitTypeIcon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PricingMethod = table.Column<int>(type: "int", nullable: false),
                    AdultsCapacity = table.Column<int>(type: "int", nullable: true),
                    ChildrenCapacity = table.Column<int>(type: "int", nullable: true),
                    MainImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrimaryFieldValues = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    PropertyAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PropertyCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PropertyStarRating = table.Column<int>(type: "int", nullable: false),
                    PropertyAverageRating = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MainAmenities = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CustomFeatures = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromotionalText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Badge = table.Column<int>(type: "int", nullable: true),
                    BadgeColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    NextAvailableDates = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    AvailabilityMessage = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DisplayFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AllowsCancellation = table.Column<bool>(type: "bit", nullable: false),
                    CancellationWindowDays = table.Column<int>(type: "int", nullable: true),
                    MinStayDays = table.Column<int>(type: "int", nullable: true),
                    MaxStayDays = table.Column<int>(type: "int", nullable: true),
                    ViewsFromSection = table.Column<int>(type: "int", nullable: false),
                    ClickCount = table.Column<int>(type: "int", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecentBookingsCount = table.Column<int>(type: "int", nullable: false),
                    Metadata = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitInSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitInSections_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitInSections_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitInSections_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyInSectionImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TempKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyInSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Sizes = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    IsMainImage = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "image"),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    VideoThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyInSectionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyInSectionImages_PropertyInSections_PropertyInSectionId",
                        column: x => x.PropertyInSectionId,
                        principalTable: "PropertyInSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UnitInSectionImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TempKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UnitInSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Sizes = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    IsMainImage = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "image"),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true),
                    VideoThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitInSectionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitInSectionImages_UnitInSections_UnitInSectionId",
                        column: x => x.UnitInSectionId,
                        principalTable: "UnitInSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5940), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5941) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5953), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5953) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5955), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5955) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5957), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5957) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5958), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5959) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5961), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(5969) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(6026), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(6026) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(6028), new DateTime(2025, 9, 26, 11, 10, 54, 372, DateTimeKind.Utc).AddTicks(6028) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3855));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5372), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5373) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5381), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5381) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5383), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5383) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5385), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5385) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5387), new DateTime(2025, 9, 26, 11, 10, 54, 312, DateTimeKind.Utc).AddTicks(5387) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2065), new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2065) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2076), new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2077) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2078), new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2078) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2080), new DateTime(2025, 9, 26, 11, 10, 54, 310, DateTimeKind.Utc).AddTicks(2080) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3926), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3926), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3926) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3929), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3930), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3930) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3780), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3781), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3768) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3792), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3793), new DateTime(2025, 9, 26, 11, 10, 54, 417, DateTimeKind.Utc).AddTicks(3790) });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_BackgroundImageId",
                table: "Sections",
                column: "BackgroundImageId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_PropertyInSectionId",
                table: "PropertyImages",
                column: "PropertyInSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_SectionId",
                table: "PropertyImages",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_UnitInSectionId",
                table: "PropertyImages",
                column: "UnitInSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInSectionImages_PropertyInSectionId",
                table: "PropertyInSectionImages",
                column: "PropertyInSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInSectionImages_TempKey",
                table: "PropertyInSectionImages",
                column: "TempKey");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInSections_PropertyId",
                table: "PropertyInSections",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInSections_SectionId_PropertyId",
                table: "PropertyInSections",
                columns: new[] { "SectionId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionImages_SectionId",
                table: "SectionImages",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionImages_TempKey",
                table: "SectionImages",
                column: "TempKey");

            migrationBuilder.CreateIndex(
                name: "IX_UnitInSectionImages_TempKey",
                table: "UnitInSectionImages",
                column: "TempKey");

            migrationBuilder.CreateIndex(
                name: "IX_UnitInSectionImages_UnitInSectionId",
                table: "UnitInSectionImages",
                column: "UnitInSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitInSections_PropertyId",
                table: "UnitInSections",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitInSections_SectionId_UnitId",
                table: "UnitInSections",
                columns: new[] { "SectionId", "UnitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitInSections_UnitId",
                table: "UnitInSections",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyImages_PropertyInSections_PropertyInSectionId",
                table: "PropertyImages",
                column: "PropertyInSectionId",
                principalTable: "PropertyInSections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyImages_Sections_SectionId",
                table: "PropertyImages",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyImages_UnitInSections_UnitInSectionId",
                table: "PropertyImages",
                column: "UnitInSectionId",
                principalTable: "UnitInSections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_SectionImages_BackgroundImageId",
                table: "Sections",
                column: "BackgroundImageId",
                principalTable: "SectionImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyImages_PropertyInSections_PropertyInSectionId",
                table: "PropertyImages");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyImages_Sections_SectionId",
                table: "PropertyImages");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyImages_UnitInSections_UnitInSectionId",
                table: "PropertyImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_SectionImages_BackgroundImageId",
                table: "Sections");

            migrationBuilder.DropTable(
                name: "PropertyInSectionImages");

            migrationBuilder.DropTable(
                name: "SectionImages");

            migrationBuilder.DropTable(
                name: "UnitInSectionImages");

            migrationBuilder.DropTable(
                name: "PropertyInSections");

            migrationBuilder.DropTable(
                name: "UnitInSections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_BackgroundImageId",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_PropertyImages_PropertyInSectionId",
                table: "PropertyImages");

            migrationBuilder.DropIndex(
                name: "IX_PropertyImages_SectionId",
                table: "PropertyImages");

            migrationBuilder.DropIndex(
                name: "IX_PropertyImages_UnitInSectionId",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "BackgroundImageId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ColorTheme",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ColumnsCount",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "DisplayStyle",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "FilterCriteria",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "IsVisibleToGuests",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "IsVisibleToRegistered",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ItemsToShow",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "MinPrice",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "MinRating",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "PropertyTypeId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "RequiresPermission",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "SortCriteria",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "UnitTypeId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "PropertyInSectionId",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "PropertyImages");

            migrationBuilder.DropColumn(
                name: "UnitInSectionId",
                table: "PropertyImages");

            migrationBuilder.CreateTable(
                name: "SectionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionItems_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionItems_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionItems_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2390), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2391) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2407), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2408) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2411), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2412) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2415), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2416) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2419), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2419) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2422), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2423) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2427), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2427) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2430), new DateTime(2025, 9, 25, 22, 46, 15, 89, DateTimeKind.Utc).AddTicks(2431) });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Code",
                keyValue: "USD",
                column: "LastUpdated",
                value: new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3897));

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4561), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4561) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4581), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4582) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4584), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4584) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4587), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4587) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4589), new DateTime(2025, 9, 25, 22, 46, 15, 29, DateTimeKind.Utc).AddTicks(4589) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2702), new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2703) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2718), new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2718) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2719), new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2719) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2721), new DateTime(2025, 9, 25, 22, 46, 15, 27, DateTimeKind.Utc).AddTicks(2721) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4161), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4161), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4161) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4168), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4168), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(4169) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3782), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3783), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3762) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3803), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3804), new DateTime(2025, 9, 25, 22, 46, 15, 138, DateTimeKind.Utc).AddTicks(3799) });

            migrationBuilder.CreateIndex(
                name: "IX_SectionItems_PropertyId",
                table: "SectionItems",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionItems_SectionId_PropertyId_UnitId",
                table: "SectionItems",
                columns: new[] { "SectionId", "PropertyId", "UnitId" },
                unique: true,
                filter: "[PropertyId] IS NOT NULL AND [UnitId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SectionItems_UnitId",
                table: "SectionItems",
                column: "UnitId");
        }
    }
}
