using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiaryExport.Migrations
{
    public partial class ChangePartPropertyName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Useremail",
                table: "UserInfos",
                newName: "UserEmail");

            migrationBuilder.RenameColumn(
                name: "Word_count",
                table: "UserInfos",
                newName: "WordCount");

            migrationBuilder.RenameColumn(
                name: "Diary_count",
                table: "UserInfos",
                newName: "DiaryCount");

            migrationBuilder.RenameColumn(
                name: "Desription",
                table: "UserInfos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "weather",
                table: "DiaryInfos",
                newName: "Weather");

            migrationBuilder.RenameColumn(
                name: "Date_word",
                table: "DiaryInfos",
                newName: "User");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserInfos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Createdtime",
                table: "DiaryInfos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DataWord",
                table: "DiaryInfos",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "Createdtime",
                table: "DiaryInfos");

            migrationBuilder.DropColumn(
                name: "DataWord",
                table: "DiaryInfos");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "UserInfos",
                newName: "Useremail");

            migrationBuilder.RenameColumn(
                name: "WordCount",
                table: "UserInfos",
                newName: "Word_count");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserInfos",
                newName: "Desription");

            migrationBuilder.RenameColumn(
                name: "DiaryCount",
                table: "UserInfos",
                newName: "Diary_count");

            migrationBuilder.RenameColumn(
                name: "Weather",
                table: "DiaryInfos",
                newName: "weather");

            migrationBuilder.RenameColumn(
                name: "User",
                table: "DiaryInfos",
                newName: "Date_word");
        }
    }
}
