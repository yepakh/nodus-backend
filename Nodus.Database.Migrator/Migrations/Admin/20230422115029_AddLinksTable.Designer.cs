﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nodus.Database.Migrator.Contexts;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Admin
{
    [DbContext(typeof(AdminContext))]
    [Migration("20230422115029_AddLinksTable")]
    partial class AddLinksTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Nodus.Database.Models.Admin.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ConnectionString")
                        .IsRequired()
                        .HasColumnType("varchar(512)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.Feature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Features");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.Link", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateExpires")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsEpxired")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("CompanyId")
                        .IsRequired()
                        .HasColumnType("int")
                        .HasColumnName("CompanyID");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.RoleFeature", b =>
                {
                    b.Property<int>("FeatureId")
                        .HasColumnType("int")
                        .HasColumnName("FeatureID");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    b.HasKey("FeatureId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleFeatures");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.Link", b =>
                {
                    b.HasOne("Nodus.Database.Models.Admin.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.Role", b =>
                {
                    b.HasOne("Nodus.Database.Models.Admin.Company", "Company")
                        .WithMany("Roles")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.RoleFeature", b =>
                {
                    b.HasOne("Nodus.Database.Models.Admin.Feature", "Feature")
                        .WithMany("RoleFeatures")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Nodus.Database.Models.Admin.Role", "Role")
                        .WithMany("RoleFeatures")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feature");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.User", b =>
                {
                    b.HasOne("Nodus.Database.Models.Admin.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.Company", b =>
                {
                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.Feature", b =>
                {
                    b.Navigation("RoleFeatures");
                });

            modelBuilder.Entity("Nodus.Database.Models.Admin.Role", b =>
                {
                    b.Navigation("RoleFeatures");

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
