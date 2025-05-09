﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecureFileSender.Api.Data;

#nullable disable

namespace SecureFileSender.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250413194128_AddAdminIdToFilterDashboards1")]
    partial class AddAdminIdToFilterDashboards1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            modelBuilder.Entity("SecureFileSender.Api.Models.AdminUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AdminUsers");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AdminUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AdminUserId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.EmailSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AdminUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SenderDisplayName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SenderEmail")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SmtpServer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("UseSSL")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AdminUserId")
                        .IsUnique();

                    b.ToTable("EmailSettings");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.SharedFileLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AdminUserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DownloadCount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasscodeHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AdminUserId");

                    b.HasIndex("CustomerId");

                    b.ToTable("SharedFileLinks");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.UploadedFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AdminUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CompressedFileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalFileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("SharedFileLinkId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AdminUserId");

                    b.HasIndex("SharedFileLinkId");

                    b.ToTable("UploadedFiles");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.Customer", b =>
                {
                    b.HasOne("SecureFileSender.Api.Models.AdminUser", "AdminUser")
                        .WithMany()
                        .HasForeignKey("AdminUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdminUser");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.EmailSettings", b =>
                {
                    b.HasOne("SecureFileSender.Api.Models.AdminUser", null)
                        .WithOne("EmailSettings")
                        .HasForeignKey("SecureFileSender.Api.Models.EmailSettings", "AdminUserId");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.SharedFileLink", b =>
                {
                    b.HasOne("SecureFileSender.Api.Models.AdminUser", "AdminUser")
                        .WithMany()
                        .HasForeignKey("AdminUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SecureFileSender.Api.Models.Customer", "Customer")
                        .WithMany("SharedLinks")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdminUser");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.UploadedFile", b =>
                {
                    b.HasOne("SecureFileSender.Api.Models.AdminUser", "AdminUser")
                        .WithMany()
                        .HasForeignKey("AdminUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SecureFileSender.Api.Models.SharedFileLink", null)
                        .WithMany("Files")
                        .HasForeignKey("SharedFileLinkId");

                    b.Navigation("AdminUser");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.AdminUser", b =>
                {
                    b.Navigation("EmailSettings");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.Customer", b =>
                {
                    b.Navigation("SharedLinks");
                });

            modelBuilder.Entity("SecureFileSender.Api.Models.SharedFileLink", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
