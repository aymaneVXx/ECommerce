using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StripeWebhook_PendingCheckout_And_SessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "CheckoutArchives",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PendingCheckouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CartJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
                    StripeSessionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingCheckouts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutArchives_StripeSessionId_ProductId",
                table: "CheckoutArchives",
                columns: new[] { "StripeSessionId", "ProductId" },
                unique: true,
                filter: "[StripeSessionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PendingCheckouts_StripeSessionId",
                table: "PendingCheckouts",
                column: "StripeSessionId",
                unique: true,
                filter: "[StripeSessionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingCheckouts");

            migrationBuilder.DropIndex(
                name: "IX_CheckoutArchives_StripeSessionId_ProductId",
                table: "CheckoutArchives");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "CheckoutArchives");
        }
    }
}
