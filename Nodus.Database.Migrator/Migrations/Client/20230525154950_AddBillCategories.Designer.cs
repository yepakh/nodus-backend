﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nodus.Database.Migrator.Contexts;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Client
{
    [DbContext(typeof(ClientContext))]
    [Migration("20230525154950_AddBillCategories")]
    partial class AddBillCategories
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Nodus.Database.Models.Customer.Bill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BillCategoryId")
                        .HasColumnType("int")
                        .HasColumnName("BillCategoryID");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorID");

                    b.Property<DateTime>("DateTimeCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTimeEdited")
                        .HasColumnType("datetime2");

                    b.Property<string>("Desciption")
                        .HasColumnType("nvarchar(1024)");

                    b.Property<Guid>("EditorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("EditorID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("StatusId")
                        .HasColumnType("int")
                        .HasColumnName("StatusID");

                    b.Property<double?>("Summary")
                        .HasColumnType("float");

                    b.Property<int>("TripId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BillCategoryId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("EditorId");

                    b.HasIndex("StatusId");

                    b.HasIndex("TripId");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.BillCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BillCategories");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.BillStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("BillStatuses");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BillId")
                        .HasColumnType("int")
                        .HasColumnName("BillID");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorID");

                    b.Property<DateTime>("DateTimeCreated")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DeactivatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("DeactivatorID");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("nvarchar(1024)");

                    b.HasKey("Id");

                    b.HasIndex("BillId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("DeactivatorId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.HistoricalBill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BillId")
                        .HasColumnType("int")
                        .HasColumnName("BillID");

                    b.Property<DateTime>("DateTimeEdit")
                        .HasColumnType("datetime2");

                    b.Property<string>("Desciption")
                        .HasColumnType("nvarchar(1024)");

                    b.Property<Guid>("EditorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("EditorID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("StatusId")
                        .HasColumnType("int")
                        .HasColumnName("StatusID");

                    b.Property<double?>("Summary")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("BillId");

                    b.HasIndex("EditorId");

                    b.HasIndex("StatusId");

                    b.ToTable("HistoricalBills");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.Trip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("CreatorID");

                    b.Property<DateTime>("DateTimeCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(1024)");

                    b.Property<DateTime>("EndOfTrip")
                        .HasColumnType("datetime2");

                    b.Property<double?>("MaxBudget")
                        .HasColumnType("float");

                    b.Property<double?>("MinBudget")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)");

                    b.Property<DateTime>("StartOfTrip")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Trips");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.UserDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserDetails");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.UserTrip", b =>
                {
                    b.Property<int>("TripId")
                        .HasColumnType("int")
                        .HasColumnName("TripID");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.Property<bool>("CanUploadBills")
                        .HasColumnType("bit");

                    b.HasKey("TripId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserTrips");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.Bill", b =>
                {
                    b.HasOne("Nodus.Database.Models.Customer.BillCategory", "BillCategory")
                        .WithMany("Bills")
                        .HasForeignKey("BillCategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.UserDetails", "Creator")
                        .WithMany("CreatedBills")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.UserDetails", "Editor")
                        .WithMany("EditedBills")
                        .HasForeignKey("EditorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.BillStatus", "Status")
                        .WithMany("Bills")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.Trip", "Trip")
                        .WithMany("Bills")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BillCategory");

                    b.Navigation("Creator");

                    b.Navigation("Editor");

                    b.Navigation("Status");

                    b.Navigation("Trip");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.Document", b =>
                {
                    b.HasOne("Nodus.Database.Models.Customer.Bill", "Bill")
                        .WithMany("Documents")
                        .HasForeignKey("BillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.UserDetails", "Creator")
                        .WithMany("CreatedDocuments")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.UserDetails", "Deactivator")
                        .WithMany("DocumentsDeactivated")
                        .HasForeignKey("DeactivatorId");

                    b.Navigation("Bill");

                    b.Navigation("Creator");

                    b.Navigation("Deactivator");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.HistoricalBill", b =>
                {
                    b.HasOne("Nodus.Database.Models.Customer.Bill", "Bill")
                        .WithMany("HistoricalBills")
                        .HasForeignKey("BillId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.UserDetails", "Editor")
                        .WithMany("HistoricalBillsEdited")
                        .HasForeignKey("EditorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.BillStatus", "Status")
                        .WithMany("HistoricalBills")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Bill");

                    b.Navigation("Editor");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.Trip", b =>
                {
                    b.HasOne("Nodus.Database.Models.Customer.UserDetails", "Creator")
                        .WithMany("CreatedTrips")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.UserTrip", b =>
                {
                    b.HasOne("Nodus.Database.Models.Customer.Trip", "Trip")
                        .WithMany("UserTrips")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Customer.UserDetails", "User")
                        .WithMany("UserTrips")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Trip");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.Bill", b =>
                {
                    b.Navigation("Documents");

                    b.Navigation("HistoricalBills");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.BillCategory", b =>
                {
                    b.Navigation("Bills");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.BillStatus", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("HistoricalBills");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.Trip", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("UserTrips");
                });

            modelBuilder.Entity("Nodus.Database.Models.Customer.UserDetails", b =>
                {
                    b.Navigation("CreatedBills");

                    b.Navigation("CreatedDocuments");

                    b.Navigation("CreatedTrips");

                    b.Navigation("DocumentsDeactivated");

                    b.Navigation("EditedBills");

                    b.Navigation("HistoricalBillsEdited");

                    b.Navigation("UserTrips");
                });
#pragma warning restore 612, 618
        }
    }
}
