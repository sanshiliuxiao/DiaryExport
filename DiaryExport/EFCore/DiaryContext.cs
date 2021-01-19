using DiaryExport.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiaryExport.EFCore
{
    public class DiaryContext: DbContext
    {
        public DbSet<DiaryInfo> DiaryInfos { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DiaryDB.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
