﻿// <auto-generated />
using Mango.Services.ProductAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Mango.Services.ProductAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231003200800_NewProduct")]
    partial class NewProduct
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-rc.1.23419.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Mango.Services.ProductAPI.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageLocalPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("ProductId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            CategoryName = "Breakfast",
                            Description = "Sandwich with eggs, ham and light sauce.",
                            ImageUrl = "https://picography.co/wp-content/uploads/2023/07/picography-food-sandwich-breakfast-cook-768x673.jpg",
                            Name = "Breakfast Sandwich",
                            Price = 3.9900000000000002
                        },
                        new
                        {
                            ProductId = 2,
                            CategoryName = "Appetizer",
                            Description = "Tacos with cheese, corn, chicken, lettuce, green pepper.",
                            ImageUrl = "https://picography.co/wp-content/uploads/2022/06/picography-plated-tacos-768x432.jpg",
                            Name = "Tacos",
                            Price = 11.99
                        },
                        new
                        {
                            ProductId = 3,
                            CategoryName = "Appetizer",
                            Description = "Scallops made in a pan.",
                            ImageUrl = "https://picography.co/wp-content/uploads/2021/01/picography-seared-scallops-in-pan-768x513.jpg",
                            Name = "Scallops",
                            Price = 14.99
                        },
                        new
                        {
                            ProductId = 4,
                            CategoryName = "Dessert",
                            Description = "Waffles with different kind of berries and strawberry topping.",
                            ImageUrl = "https://picography.co/wp-content/uploads/2019/07/picography-breakfast-flatlay-with-fruit-and-waffles-768x1015.jpg",
                            Name = "Waffles",
                            Price = 8.9900000000000002
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
