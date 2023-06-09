﻿// <auto-generated />
using System;
using RealEstateMarket.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace RealEstateMarket.Migrations
{
    [DbContext(typeof(MarketDbContext))]
    [Migration("20221209201504_V1.0")]
    partial class V10
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("RealEstateMarket.Models.Advertisement", b =>
                {
                    b.Property<int>("AdId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AdId"), 1L, 1);

                    b.Property<string>("BrokerageId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AdId");

                    b.HasIndex("BrokerageId");

                    b.ToTable("Advertisement", (string)null);
                });

            modelBuilder.Entity("RealEstateMarket.Models.Brokerage", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Fee")
                        .HasColumnType("money");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Brokerage", (string)null);
                });

            modelBuilder.Entity("RealEstateMarket.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Client", (string)null);
                });

            modelBuilder.Entity("RealEstateMarket.Models.Subscription", b =>
                {
                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("BrokerageId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ClientId", "BrokerageId");

                    b.HasIndex("BrokerageId");

                    b.ToTable("Subscription", (string)null);
                });

            modelBuilder.Entity("RealEstateMarket.Models.Advertisement", b =>
                {
                    b.HasOne("RealEstateMarket.Models.Brokerage", "Brokerage")
                        .WithMany("Advertisements")
                        .HasForeignKey("BrokerageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brokerage");
                });

            modelBuilder.Entity("RealEstateMarket.Models.Subscription", b =>
                {
                    b.HasOne("RealEstateMarket.Models.Brokerage", "Brokerage")
                        .WithMany("Subscriptions")
                        .HasForeignKey("BrokerageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RealEstateMarket.Models.Client", "Client")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brokerage");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("RealEstateMarket.Models.Brokerage", b =>
                {
                    b.Navigation("Advertisements");

                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("RealEstateMarket.Models.Client", b =>
                {
                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
