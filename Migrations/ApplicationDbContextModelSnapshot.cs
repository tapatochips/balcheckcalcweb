﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using balcheckcalcweb.Data;

#nullable disable

namespace balcheckcalcweb.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("balcheckcalcweb.Models.CheckHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CalculationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PolicyCount")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserAlias")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("CheckHistories");
                });

            modelBuilder.Entity("balcheckcalcweb.Models.CheckHistoryDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("CheckHistoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CurrentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EffectiveDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Installment")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PolicyNumber")
                        .HasColumnType("int");

                    b.Property<decimal>("RevisedAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CheckHistoryId");

                    b.ToTable("CheckHistoryDetails");
                });

            modelBuilder.Entity("balcheckcalcweb.Models.CheckHistoryDetail", b =>
                {
                    b.HasOne("balcheckcalcweb.Models.CheckHistory", "CheckHistory")
                        .WithMany("PolicyDetails")
                        .HasForeignKey("CheckHistoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CheckHistory");
                });

            modelBuilder.Entity("balcheckcalcweb.Models.CheckHistory", b =>
                {
                    b.Navigation("PolicyDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
