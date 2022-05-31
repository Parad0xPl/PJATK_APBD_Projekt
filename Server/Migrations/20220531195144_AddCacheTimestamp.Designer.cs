﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server.Entities;

#nullable disable

namespace Server.Migrations
{
    [DbContext(typeof(StockContext))]
    [Migration("20220531195144_AddCacheTimestamp")]
    partial class AddCacheTimestamp
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.5");

            modelBuilder.Entity("Server.Entities.Account", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasAnnotation("SqlServer:IdentityIncrement", 1)
                        .HasAnnotation("SqlServer:IdentitySeed", 1L)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Server.Entities.ObservedStock", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StockId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AccountId", "StockId");

                    b.HasIndex("StockId");

                    b.ToTable("Watchlist");
                });

            modelBuilder.Entity("Server.Entities.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasAnnotation("SqlServer:IdentityIncrement", 1)
                        .HasAnnotation("SqlServer:IdentitySeed", 1L)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountID")
                        .IsUnique();

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Server.Entities.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("RequestJSON")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Ticker")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("Server.Entities.ObservedStock", b =>
                {
                    b.HasOne("Server.Entities.Account", null)
                        .WithMany("Watchlist")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Server.Entities.Stock", null)
                        .WithMany("Observers")
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Server.Entities.RefreshToken", b =>
                {
                    b.HasOne("Server.Entities.Account", "Account")
                        .WithOne()
                        .HasForeignKey("Server.Entities.RefreshToken", "AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Server.Entities.Account", b =>
                {
                    b.Navigation("Watchlist");
                });

            modelBuilder.Entity("Server.Entities.Stock", b =>
                {
                    b.Navigation("Observers");
                });
#pragma warning restore 612, 618
        }
    }
}
