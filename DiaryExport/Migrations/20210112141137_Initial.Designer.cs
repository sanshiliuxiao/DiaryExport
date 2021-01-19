﻿// <auto-generated />
using System;
using DiaryExport.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiaryExport.Migrations
{
    [DbContext(typeof(DiaryContext))]
    [Migration("20210112141137_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("DiaryExport.Models.DiaryInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Createddate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Date_word")
                        .HasColumnType("TEXT");

                    b.Property<string>("Deleteddate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mood")
                        .HasColumnType("TEXT");

                    b.Property<string>("Space")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Ts")
                        .HasColumnType("TEXT");

                    b.Property<string>("Weekday")
                        .HasColumnType("TEXT");

                    b.Property<string>("weather")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DiaryInfos");
                });

            modelBuilder.Entity("DiaryExport.Models.UserInfo", b =>
                {
                    b.Property<string>("Avatar")
                        .HasColumnType("TEXT");

                    b.Property<string>("Desription")
                        .HasColumnType("TEXT");

                    b.Property<int>("Diary_count")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Useremail")
                        .HasColumnType("TEXT");

                    b.Property<int>("Word_count")
                        .HasColumnType("INTEGER");

                    b.ToTable("UserInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
