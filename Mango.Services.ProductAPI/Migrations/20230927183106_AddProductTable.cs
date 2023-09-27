using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mango.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Breakfast", "Sandwich with eggs, ham and light sauce.", "https://picography.co/wp-content/uploads/2023/07/picography-food-sandwich-breakfast-cook-768x673.jpg", "Breakfast Sandwich", 3.9900000000000002 },
                    { 2, "Appetizer", "Tacos with cheese, corn, chicken, lettuce, green pepper.", "https://picography.co/wp-content/uploads/2022/06/picography-plated-tacos-768x432.jpg", "Tacos", 11.99 },
                    { 3, "Appetizer", "Scallops made in a pan.", "https://picography.co/wp-content/uploads/2021/01/picography-seared-scallops-in-pan-768x513.jpg", "Scallops", 14.99 },
                    { 4, "Dessert", "Waffles with different kind of berries and strawberry topping.", "https://picography.co/wp-content/uploads/2019/07/picography-breakfast-flatlay-with-fruit-and-waffles-768x1015.jpg", "Waffles", 8.9900000000000002 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
