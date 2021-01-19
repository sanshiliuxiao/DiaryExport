using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiaryExport.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiaryInfos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Deleteddate = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Mood = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Space = table.Column<string>(type: "TEXT", nullable: true),
                    Ts = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    Date_word = table.Column<string>(type: "TEXT", nullable: true),
                    weather = table.Column<string>(type: "TEXT", nullable: true),
                    Createddate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Weekday = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Useremail = table.Column<string>(type: "TEXT", nullable: true),
                    Diary_count = table.Column<int>(type: "INTEGER", nullable: false),
                    Word_count = table.Column<int>(type: "INTEGER", nullable: false),
                    Desription = table.Column<string>(type: "TEXT", nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiaryInfos");

            migrationBuilder.DropTable(
                name: "UserInfos");
        }
    }
}
