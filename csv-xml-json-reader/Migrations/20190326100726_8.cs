using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace csv_xml_json_reader.Migrations
{
    public partial class _8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Orders",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "clientId",
                table: "Orders",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "OrderModel",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    clientId = table.Column<string>(maxLength: 6, nullable: false),
                    requestId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderModel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrderModelDetails",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 255, nullable: false),
                    quantity = table.Column<int>(nullable: false),
                    price = table.Column<decimal>(type: "money", nullable: false),
                    OrderModelid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderModelDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrderModelDetails_OrderModel_OrderModelid",
                        column: x => x.OrderModelid,
                        principalTable: "OrderModel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderModelDetails_OrderModelid",
                table: "OrderModelDetails",
                column: "OrderModelid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderModelDetails");

            migrationBuilder.DropTable(
                name: "OrderModel");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Orders",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "clientId",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 6);
        }
    }
}
