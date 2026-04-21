using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "EmailIndex",
                table: "Users",
                newName: "IX_Users_NormalizedEmail");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 9, 18, 45, 55, 419, DateTimeKind.Utc).AddTicks(4394), new DateTime(2026, 4, 9, 18, 45, 55, 419, DateTimeKind.Utc).AddTicks(4397) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 9, 18, 45, 55, 419, DateTimeKind.Utc).AddTicks(4403), new DateTime(2026, 4, 9, 18, 45, 55, 419, DateTimeKind.Utc).AddTicks(4403) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 9, 18, 45, 55, 419, DateTimeKind.Utc).AddTicks(4406), new DateTime(2026, 4, 9, 18, 45, 55, 419, DateTimeKind.Utc).AddTicks(4406) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                newName: "EmailIndex");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 9, 7, 10, 52, 873, DateTimeKind.Utc).AddTicks(6359), new DateTime(2026, 4, 9, 7, 10, 52, 873, DateTimeKind.Utc).AddTicks(6363) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 9, 7, 10, 52, 873, DateTimeKind.Utc).AddTicks(6371), new DateTime(2026, 4, 9, 7, 10, 52, 873, DateTimeKind.Utc).AddTicks(6371) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 9, 7, 10, 52, 873, DateTimeKind.Utc).AddTicks(6373), new DateTime(2026, 4, 9, 7, 10, 52, 873, DateTimeKind.Utc).AddTicks(6374) });
        }
    }
}
