using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollLedgerSync.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageFingerprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Consumer = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePayrolls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PayrollBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePayrolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePayrolls_PayrollBatches_PayrollBatchId",
                        column: x => x.PayrollBatchId,
                        principalTable: "PayrollBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LedgerEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DebitCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PayrollBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerEntries_PayrollBatches_PayrollBatchId",
                        column: x => x.PayrollBatchId,
                        principalTable: "PayrollBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrolls_Batch_EmployeeCode",
                table: "EmployeePayrolls",
                columns: new[] { "PayrollBatchId", "EmployeeCode" });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntries_Batch_AccountCode",
                table: "LedgerEntries",
                columns: new[] { "PayrollBatchId", "AccountCode" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_EventType",
                table: "OutboxEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_ProcessingWindow",
                table: "OutboxEvents",
                columns: new[] { "ProcessedOnUtc", "OccurredOnUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_SoftDelete_OccurredOn",
                table: "OutboxEvents",
                columns: new[] { "IsDeleted", "OccurredOnUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollBatches_Period",
                table: "PayrollBatches",
                column: "Period");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollBatches_SoftDelete_CreatedOn",
                table: "PayrollBatches",
                columns: new[] { "IsDeleted", "CreatedOnUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollBatches_Status",
                table: "PayrollBatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_SoftDelete_ProcessedOn",
                table: "ProcessedMessages",
                columns: new[] { "IsDeleted", "ProcessedOnUtc" });

            migrationBuilder.CreateIndex(
                name: "UX_ProcessedMessages_Fingerprint_Consumer",
                table: "ProcessedMessages",
                columns: new[] { "MessageFingerprint", "Consumer" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeePayrolls");

            migrationBuilder.DropTable(
                name: "LedgerEntries");

            migrationBuilder.DropTable(
                name: "OutboxEvents");

            migrationBuilder.DropTable(
                name: "ProcessedMessages");

            migrationBuilder.DropTable(
                name: "PayrollBatches");
        }
    }
}
