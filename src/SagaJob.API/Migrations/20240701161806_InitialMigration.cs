using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SagaJob.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatchStateData",
                columns: table => new
                {
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentState = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    MerchantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BatchType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    JobActiveThreshold = table.Column<int>(type: "int", nullable: false),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    UnprocessedTokenIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessingTokenIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchStateData", x => x.BatchId);
                });

            migrationBuilder.CreateTable(
                name: "JobStateData",
                columns: table => new
                {
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentState = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BatchType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ExceptionMessage = table.Column<string>(type: "varchar(max)", unicode: false, maxLength: 2000, nullable: true),
                    LastJob = table.Column<bool>(type: "bit", nullable: false),
                    Processed = table.Column<bool>(type: "bit", nullable: false),
                    CurrentPage = table.Column<int>(type: "int", nullable: false),
                    PageSize = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStateData", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_JobStateData_BatchStateData_BatchId",
                        column: x => x.BatchId,
                        principalTable: "BatchStateData",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobStateData_BatchId",
                table: "JobStateData",
                column: "BatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobStateData");

            migrationBuilder.DropTable(
                name: "BatchStateData");
        }
    }
}
