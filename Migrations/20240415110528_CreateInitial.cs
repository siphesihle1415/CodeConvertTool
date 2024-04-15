using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeConverterTool.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "developers",
                columns: table => new
                {
                    dev_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__develope__8AB90B4949C53C0F", x => x.dev_id);
                });

            migrationBuilder.CreateTable(
                name: "scripttypelookup",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scriptty__2C0005986CC1562B", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "scripts",
                columns: table => new
                {
                    script_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dev_id = table.Column<int>(type: "int", nullable: false),
                    script_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    script_s3_uri = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    script_type = table.Column<int>(type: "int", nullable: false),
                    script_version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__scripts__EDFCC9DF1DA27B3B", x => x.script_id);
                    table.ForeignKey(
                        name: "FK__scripts__dev_id__276EDEB3",
                        column: x => x.dev_id,
                        principalTable: "developers",
                        principalColumn: "dev_id");
                    table.ForeignKey(
                        name: "FK__scripts__script___286302EC",
                        column: x => x.script_type,
                        principalTable: "scripttypelookup",
                        principalColumn: "type_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_scripts_dev_id",
                table: "scripts",
                column: "dev_id");

            migrationBuilder.CreateIndex(
                name: "IX_scripts_script_type",
                table: "scripts",
                column: "script_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scripts");

            migrationBuilder.DropTable(
                name: "developers");

            migrationBuilder.DropTable(
                name: "scripttypelookup");
        }
    }
}
