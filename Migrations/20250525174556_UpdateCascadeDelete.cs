using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab2_DB.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Видаляємо зовнішні ключі
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Funds",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_PublishingHouses",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Libraries",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Funds_Departments",
                table: "Funds");

            migrationBuilder.DropForeignKey(
                name: "FK_IssuedBooks_Books1",
                table: "IssuedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_IssuedBooks_Requests",
                table: "IssuedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Librarians",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Readers",
                table: "Requests");

            // Додаємо зовнішні ключі з новою поведінкою (Cascade)
            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors",
                table: "Books",
                column: "AuthorBook",
                principalTable: "Authors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Funds",
                table: "Books",
                column: "FundBook",
                principalTable: "Funds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_PublishingHouses",
                table: "Books",
                column: "PublisherBook",
                principalTable: "PublishingHouses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Libraries",
                table: "Departments",
                column: "Library",
                principalTable: "Libraries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Funds_Departments",
                table: "Funds",
                column: "DepartmentFund",
                principalTable: "Departments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssuedBooks_Books1",
                table: "IssuedBooks",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssuedBooks_Requests",
                table: "IssuedBooks",
                column: "RequestID",
                principalTable: "Requests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Librarians",
                table: "Requests",
                column: "PassNumberLibrarian",
                principalTable: "Librarians",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Readers",
                table: "Requests",
                column: "CardNumberReader",
                principalTable: "Readers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Видаляємо зовнішні ключі
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Funds",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_PublishingHouses",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Libraries",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Funds_Departments",
                table: "Funds");

            migrationBuilder.DropForeignKey(
                name: "FK_IssuedBooks_Books1",
                table: "IssuedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_IssuedBooks_Requests",
                table: "IssuedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Librarians",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Readers",
                table: "Requests");

            // Відновлюємо зовнішні ключі з попередньою поведінкою (Restrict)
            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors",
                table: "Books",
                column: "AuthorBook",
                principalTable: "Authors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Funds",
                table: "Books",
                column: "FundBook",
                principalTable: "Funds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_PublishingHouses",
                table: "Books",
                column: "PublisherBook",
                principalTable: "PublishingHouses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Libraries",
                table: "Departments",
                column: "Library",
                principalTable: "Libraries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Funds_Departments",
                table: "Funds",
                column: "DepartmentFund",
                principalTable: "Departments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssuedBooks_Books1",
                table: "IssuedBooks",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssuedBooks_Requests",
                table: "IssuedBooks",
                column: "RequestID",
                principalTable: "Requests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Librarians",
                table: "Requests",
                column: "PassNumberLibrarian",
                principalTable: "Librarians",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Readers",
                table: "Requests",
                column: "CardNumberReader",
                principalTable: "Readers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}