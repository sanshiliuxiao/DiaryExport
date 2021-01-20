using DiaryExport.EFCore;
using DiaryExport.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryExport.Servies
{
    public class DbServices
    {
        private readonly DiaryContext _context;
        public DbServices(DiaryContext context)
        {
            _context = context;
        }
        public UserInfo GetUserInfoById(string id)
        {
            return _context.UserInfos.FirstOrDefault(u => u.Id == id);
        }
        public async Task AddOneUserInfoAsync(UserInfo info)
        {
            await _context.UserInfos.AddAsync(info);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateOneUserInfoAsync(UserInfo info)
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddOneDiaryInfoAsync(DiaryInfo info)
        {
            await _context.DiaryInfos.AddAsync(info);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAllDiaryInfoAsync(string id)
        {
            var allData = await _context.DiaryInfos.Where(d => d.User == id).ToListAsync();
            _context.DiaryInfos.RemoveRange(allData);
            await _context.SaveChangesAsync();
        }
        public int GetCurrentUserInSqliteFilteDeletedDiaryCount(string id, string filterContent)
        {
            return _context.DiaryInfos.Where(d => d.User == id && d.Content == filterContent).Count();
        }

        public int GetCurrentUserInSqliteDiaryCount(string id)
        {
            return _context.DiaryInfos.Where(d => d.User == id).Count();
        }
        public async Task<List<DiaryInfo>> GetCurrentUserInSqliteAllData(string id)
        {
            var allDiary = await _context.DiaryInfos.Where(d => d.User == id).ToListAsync();
            return allDiary;
        }
        public async Task<DiaryInfo> GetLatestDiary(string id)
        {
            var diary = await _context.DiaryInfos.Where(d => d.User == id).OrderByDescending(t => t.Createddate).FirstOrDefaultAsync();
            return diary ;
        }
        public async Task<DiaryInfo> GetOldestDiary(string id)
        {
            var diary = await _context.DiaryInfos.Where(d => d.User == id).OrderBy(t => t.Createddate).FirstOrDefaultAsync();
            return diary;
        }
    }
}
