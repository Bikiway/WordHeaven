using Microsoft.EntityFrameworkCore.Migrations;

namespace WordHeaven_Web.Migrations
{
    public partial class updateBookOnReservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookName",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "BookNameId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BookNameId",
                table: "Reservations",
                column: "BookNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Books_BookNameId",
                table: "Reservations",
                column: "BookNameId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Books_BookNameId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_BookNameId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "BookNameId",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "BookName",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
