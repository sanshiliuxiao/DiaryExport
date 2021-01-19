using DiaryExport.EFCore;
using DiaryExport.Models;
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
        public int GetCurrentUserInSqliteDiaryCount(string id)
        {
            return _context.DiaryInfos.Where(d => d.User == id).Count();
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
    }
}
