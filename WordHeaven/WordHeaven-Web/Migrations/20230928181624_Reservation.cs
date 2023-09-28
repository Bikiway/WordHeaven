using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WordHeaven_Web.Migrations
{
    public partial class Reservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookFoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanedBook = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookReturned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanTimeLimit = table.Column<int>(type: "int", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false),
                    BookReturnedByClient = table.Column<bool>(type: "bit", nullable: false),
                    ClientDidntReturnTheBook = table.Column<bool>(type: "bit", nullable: false),
                    PayTaxesLoan = table.Column<int>(type: "int", nullable: false),
                    PayedTaxesLoan = table.Column<bool>(type: "bit", nullable: false),
                    RenewBookLoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationsDetailTemp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bookId = table.Column<int>(type: "int", nullable: true),
                    coverId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanedBook = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookReturned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Request = table.Column<int>(type: "int", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationsDetailTemp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationsDetailTemp_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationsDetailTemp_Books_bookId",
                        column: x => x.bookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationsDetailTemp_Books_coverId",
                        column: x => x.coverId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationsDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bookNameId = table.Column<int>(type: "int", nullable: true),
                    coverImageId = table.Column<int>(type: "int", nullable: true),
                    LoanedBook = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookReturned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Request = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationsDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationsDetail_Books_bookNameId",
                        column: x => x.bookNameId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationsDetail_Books_coverImageId",
                        column: x => x.coverImageId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationsDetail_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_userId",
                table: "Reservations",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsDetail_bookNameId",
                table: "ReservationsDetail",
                column: "bookNameId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsDetail_coverImageId",
                table: "ReservationsDetail",
                column: "coverImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsDetail_ReservationId",
                table: "ReservationsDetail",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsDetailTemp_bookId",
                table: "ReservationsDetailTemp",
                column: "bookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsDetailTemp_coverId",
                table: "ReservationsDetailTemp",
                column: "coverId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationsDetailTemp_userId",
                table: "ReservationsDetailTemp",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationsDetail");

            migrationBuilder.DropTable(
                name: "ReservationsDetailTemp");

            migrationBuilder.DropTable(
                name: "Reservations");
        }
    }
}
