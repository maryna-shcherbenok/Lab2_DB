using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab2_DB.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    FullNameAuthor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumberAuthor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmailAuthor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Librarians",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    FullNameLibrarian = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumberLibrarian = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmailLibrarian = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Librarians", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    TitleLibrary = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumberLibrary = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmailLibrary = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LibraryAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LeaderLibrary = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PublishingHouses",
                columns: table => new
                {
                    ID = table.Column<long>(type: "int", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    TitlePH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumberPH = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddressPH = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmailPH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishingHouses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Readers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    FullNameReader = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumberReader = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmailReader = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleReader = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlaceStudyOrWorkReader = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    TitleDepartment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LeaderDepartment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Library = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Departments_Libraries",
                        column: x => x.Library,
                        principalTable: "Libraries",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    PassNumberLibrarian = table.Column<long>(type: "bigint", nullable: false),
                    CardNumberReader = table.Column<long>(type: "bigint", nullable: false),
                    CreationDateRequest = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ISBN = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Requests_Librarians",
                        column: x => x.PassNumberLibrarian,
                        principalTable: "Librarians",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Requests_Readers",
                        column: x => x.CardNumberReader,
                        principalTable: "Readers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    TitleFund = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TypeFund = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalNumberCopies = table.Column<int>(type: "int", nullable: false),
                    DepartmentFund = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Funds_Departments",
                        column: x => x.DepartmentFund,
                        principalTable: "Departments",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    ISBN = table.Column<long>(type: "bigint", nullable: false),
                    TitleBook = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AuthorBook = table.Column<long>(type: "bigint", nullable: false),
                    GenreBook = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AvailabilityStatusBook = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumberPages = table.Column<int>(type: "int", nullable: false),
                    FundBook = table.Column<long>(type: "bigint", nullable: false),
                    PublisherBook = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Books_Authors",
                        column: x => x.AuthorBook,
                        principalTable: "Authors",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Books_Funds",
                        column: x => x.FundBook,
                        principalTable: "Funds",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Books_PublishingHouses",
                        column: x => x.PublisherBook,
                        principalTable: "PublishingHouses",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "IssuedBooks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
    .Annotation("SqlServer:Identity", "1, 1"),

                    RequestID = table.Column<long>(type: "bigint", nullable: false),
                    BookID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedBooks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_IssuedBooks_Books1",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_IssuedBooks_Requests",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorBook",
                table: "Books",
                column: "AuthorBook");

            migrationBuilder.CreateIndex(
                name: "IX_Books_FundBook",
                table: "Books",
                column: "FundBook");

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherBook",
                table: "Books",
                column: "PublisherBook");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Library",
                table: "Departments",
                column: "Library");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_DepartmentFund",
                table: "Funds",
                column: "DepartmentFund");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedBooks_BookID",
                table: "IssuedBooks",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedBooks_RequestID",
                table: "IssuedBooks",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CardNumberReader",
                table: "Requests",
                column: "CardNumberReader");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_PassNumberLibrarian",
                table: "Requests",
                column: "PassNumberLibrarian");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssuedBooks");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Funds");

            migrationBuilder.DropTable(
                name: "PublishingHouses");

            migrationBuilder.DropTable(
                name: "Librarians");

            migrationBuilder.DropTable(
                name: "Readers");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Libraries");
        }
    }
}
