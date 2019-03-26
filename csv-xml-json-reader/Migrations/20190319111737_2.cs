using Microsoft.EntityFrameworkCore.Migrations;

namespace csv_xml_json_reader.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Orders",
                newName: "requestId");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Orders",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Orders",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Orders",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Orders",
                newName: "clientId");

            migrationBuilder.AlterColumn<string>(
                name: "price",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "requestId",
                table: "Orders",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "Orders",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Orders",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Orders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "clientId",
                table: "Orders",
                newName: "ClientId");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
