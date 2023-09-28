using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WordHeaven_Web.Migrations
{
    public partial class ReservationsAltered : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationsDetail_Books_coverImageId",
                table: "ReservationsDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationsDetailTemp_Books_coverId",
                table: "ReservationsDetailTemp");

            migrationBuilder.DropColumn(
                name: "BookFoto",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "coverId",
                table: "ReservationsDetailTemp",
                newName: "StoreNameId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationsDetailTemp_coverId",
                table: "ReservationsDetailTemp",
                newName: "IX_ReservationsDetailTemp_StoreNameId");

            migrationBuilder.RenameColumn(
                name: "coverImageId",
                table: "ReservationsDetail",
                newName: "StoreNameId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationsDetail_coverImageId",
                table: "ReservationsDetail",
                newName: "IX_ReservationsDetail_StoreNameId");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "BookCover",
                table: "ReservationsDetailTemp",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "BookCover",
                table: "ReservationsDetail",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientFirstName",
                table: "ReservationsDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientLastName",
                table: "ReservationsDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "BookCover",
                table: "Reservations",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "StoreNameId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_userId",
                table: "Stores",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_StoreNameId",
                table: "Reservations",
                column: "StoreNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Stores_StoreNameId",
                table: "Reservations",
                column: "StoreNameId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationsDetail_Stores_StoreNameId",
                table: "ReservationsDetail",
                column: "StoreNameId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationsDetailTemp_Stores_StoreNameId",
                table: "ReservationsDetailTemp",
                column: "StoreNameId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_userId",
                table: "Stores",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Stores_StoreNameId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationsDetail_Stores_StoreNameId",
                table: "ReservationsDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationsDetailTemp_Stores_StoreNameId",
                table: "ReservationsDetailTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_userId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_userId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_StoreNameId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "BookCover",
                table: "ReservationsDetailTemp");

            migrationBuilder.DropColumn(
                name: "BookCover",
                table: "ReservationsDetail");

            migrationBuilder.DropColumn(
                name: "ClientFirstName",
                table: "ReservationsDetail");

            migrationBuilder.DropColumn(
                name: "ClientLastName",
                table: "ReservationsDetail");

            migrationBuilder.DropColumn(
                name: "BookCover",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "StoreNameId",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "StoreNameId",
                table: "ReservationsDetailTemp",
                newName: "coverId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationsDetailTemp_StoreNameId",
                table: "ReservationsDetailTemp",
                newName: "IX_ReservationsDetailTemp_coverId");

            migrationBuilder.RenameColumn(
                name: "StoreNameId",
                table: "ReservationsDetail",
                newName: "coverImageId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationsDetail_StoreNameId",
                table: "ReservationsDetail",
                newName: "IX_ReservationsDetail_coverImageId");

            migrationBuilder.AddColumn<string>(
                name: "BookFoto",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationsDetail_Books_coverImageId",
                table: "ReservationsDetail",
                column: "coverImageId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationsDetailTemp_Books_coverId",
                table: "ReservationsDetailTemp",
                column: "coverId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
