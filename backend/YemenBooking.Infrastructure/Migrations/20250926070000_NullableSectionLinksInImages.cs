using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YemenBooking.Infrastructure.Migrations
{
    public partial class NullableSectionLinksInImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SectionImages: make SectionId nullable and set delete behavior to SetNull
            migrationBuilder.DropForeignKey(
                name: "FK_SectionImages_Sections_SectionId",
                table: "SectionImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "SectionId",
                table: "SectionImages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionImages_Sections_SectionId",
                table: "SectionImages",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // PropertyInSectionImages: make PropertyInSectionId nullable and SetNull
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyInSectionImages_PropertyInSections_PropertyInSectionId",
                table: "PropertyInSectionImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "PropertyInSectionId",
                table: "PropertyInSectionImages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyInSectionImages_PropertyInSections_PropertyInSectionId",
                table: "PropertyInSectionImages",
                column: "PropertyInSectionId",
                principalTable: "PropertyInSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // UnitInSectionImages: make UnitInSectionId nullable and SetNull
            migrationBuilder.DropForeignKey(
                name: "FK_UnitInSectionImages_UnitInSections_UnitInSectionId",
                table: "UnitInSectionImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitInSectionId",
                table: "UnitInSectionImages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitInSectionImages_UnitInSections_UnitInSectionId",
                table: "UnitInSectionImages",
                column: "UnitInSectionId",
                principalTable: "UnitInSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionImages_Sections_SectionId",
                table: "SectionImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "SectionId",
                table: "SectionImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SectionImages_Sections_SectionId",
                table: "SectionImages",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyInSectionImages_PropertyInSections_PropertyInSectionId",
                table: "PropertyInSectionImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "PropertyInSectionId",
                table: "PropertyInSectionImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyInSectionImages_PropertyInSections_PropertyInSectionId",
                table: "PropertyInSectionImages",
                column: "PropertyInSectionId",
                principalTable: "PropertyInSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropForeignKey(
                name: "FK_UnitInSectionImages_UnitInSections_UnitInSectionId",
                table: "UnitInSectionImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitInSectionId",
                table: "UnitInSectionImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitInSectionImages_UnitInSections_UnitInSectionId",
                table: "UnitInSectionImages",
                column: "UnitInSectionId",
                principalTable: "UnitInSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

