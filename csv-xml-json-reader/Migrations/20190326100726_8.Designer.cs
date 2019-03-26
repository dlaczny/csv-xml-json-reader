﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using csv_xml_json_reader.Models;

namespace csv_xml_json_reader.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190326100726_8")]
    partial class _8
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("csv_xml_json_reader.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("clientId")
                        .IsRequired()
                        .HasMaxLength(6);

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<decimal>("price")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 38, scale: 17)))
                        .HasColumnType("money");

                    b.Property<int>("quantity");

                    b.Property<long>("requestId");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("csv_xml_json_reader.Models.OrderModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("clientId")
                        .IsRequired()
                        .HasMaxLength(6);

                    b.Property<long>("requestId");

                    b.HasKey("id");

                    b.ToTable("OrderModel");
                });

            modelBuilder.Entity("csv_xml_json_reader.Models.OrderModelDetails", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("OrderModelid");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<decimal>("price")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 38, scale: 17)))
                        .HasColumnType("money");

                    b.Property<int>("quantity");

                    b.HasKey("id");

                    b.HasIndex("OrderModelid");

                    b.ToTable("OrderModelDetails");
                });

            modelBuilder.Entity("csv_xml_json_reader.Models.OrderModelDetails", b =>
                {
                    b.HasOne("csv_xml_json_reader.Models.OrderModel")
                        .WithMany("OrderModelDetails")
                        .HasForeignKey("OrderModelid");
                });
#pragma warning restore 612, 618
        }
    }
}