using Microsoft.EntityFrameworkCore.Migrations;

namespace WordHeaven_Web.Migrations
{
    public partial class forgotTheUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_userId",
                table: "Employee",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AspNetUsers_userId",
                table: "Employee",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers_userId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_userId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Employee");
        }
    }
}
