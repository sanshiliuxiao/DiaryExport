using DiaryExport.EFCore;
using DiaryExport.ModelDtos;
using DiaryExport.Models;
using Flurl;
using Flurl.Http;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DiaryExport.Servies
{
    public class ExportDiaryServices
    {
        private readonly LoginModel _loginModel;
        private readonly string _baseUrl = "https://ohshenghuo.com/api";
        public UserModel User { get; set; } = new UserModel();

        public int _currentTryCount = 0;
        public int _maxTryCount = 3;

        public delegate void TipDelegate(String tip);
        public event TipDelegate TipEvent = Console.WriteLine;


        // Declare the delegate 
        public delegate void LoginEventHandler(string tip);
        public event LoginEventHandler LoginEvent = Console.WriteLine;

        public delegate void ExportDiaryStatusEventHandler(string tip);
        public event ExportDiaryStatusEventHandler ExportDiaryStatusEvent = Console.WriteLine;

        public ExportDiaryServices(LoginModel loginModel)
        {
            _loginModel = loginModel ?? new LoginModel();
        }

        public void ResetTryCount()
        {
            _currentTryCount = 0;
        }
        public async Task<UserModel> Login()
        {
            var postData = new
            {
                csrfmiddlewaretoken = _loginModel.Csrf,
                email = _loginModel.Email,
                password = _loginModel.Password
            };
            try
            {
                User = await new Flurl.Url(_baseUrl).AppendPathSegment("/login/").PostUrlEncodedAsync(postData).ReceiveJson<UserModel>();
                _loginModel.Token = $"token {User.Token}";
                LoginEvent?.Invoke("登录成功");
                ExportDiaryStatusEvent?.Invoke("登录成功");
                ResetTryCount();
                return User;
  
            }
            catch (Exception)
            {
                _currentTryCount++;
                LoginEvent?.Invoke("登陆失败，请检测账号密码或网络");
                ExportDiaryStatusEvent?.Invoke("登陆失败，请检测账号密码或网络");
                ExportDiaryStatusEvent?.Invoke($"正在第 {_currentTryCount} 次 尝试重新登录");
                
                await Task.Delay(_currentTryCount * 1000);

                if (_currentTryCount < _maxTryCount)
                {
                    return await Login();
                }
                ExportDiaryStatusEvent?.Invoke("彻底的失败了！");
                ResetTryCount();
                return null;
            }
        }
        public async Task<DiaryModel> GetLatestDiary()
        {
            try
            {
                var diaryModel = await new Flurl.Url(_baseUrl)
                                                .AppendPathSegment("/diary/latest/")
                                                .WithHeader("auth", _loginModel.Token)
                                                .GetAsync()
                                                .ReceiveJson<DiaryModel>();
                ExportDiaryStatusEvent?.Invoke($"获取最新一篇日记成功：id: {diaryModel.Diary.Id}");
                ResetTryCount();
                return diaryModel;
            }
            catch (Exception)
            {
                _currentTryCount++;
                ExportDiaryStatusEvent?.Invoke($"获取最新一篇日记失败...正在 {_currentTryCount} 尝试");
                await Task.Delay(_currentTryCount * 1000);
                
                if (_currentTryCount < _maxTryCount)
                {
                    return await GetLatestDiary();
                }
                ExportDiaryStatusEvent?.Invoke($"获取最新一篇日记失败...终止获取");
                ResetTryCount();
                return null;
            }
        }
        public async Task<DiaryModel> GetDiaryPrevById(string id)
        {
            try
            {
                var diaryModel = await new Flurl.Url(_baseUrl)
                                            .AppendPathSegment($"/diary/prev/{id}/")
                                            .WithHeader("auth", _loginModel.Token)
                                            .GetAsync()
                                            .ReceiveJson<DiaryModel>();
                if (diaryModel.Diary != null)
                {
                    ExportDiaryStatusEvent?.Invoke($"获取上一篇日记成功：id: {diaryModel.Diary.Id}");
                    
                }
                ResetTryCount();
                return diaryModel;
            }
            catch (Exception)
            {
                _currentTryCount++;
                ExportDiaryStatusEvent?.Invoke($"获取上一篇日记失败...正在 {_currentTryCount} 尝试");
                await Task.Delay(_currentTryCount * 1000);
                
                if (_currentTryCount < _maxTryCount)
                {
                    return await GetDiaryPrevById(id);
                }
                ExportDiaryStatusEvent?.Invoke($"获取上一篇日记失败...终止请求");
                ResetTryCount();
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">eg: 2021-01-24</param>
        /// <returns></returns>
        public async Task<DiaryModel> GetDiaryByDate(string date)
        {
            try
            {
                var diaryModel = await new Flurl.Url(_baseUrl)
                                            .AppendPathSegment($"/diary/")
                                            .SetQueryParams(new { date=date })
                                            .WithHeader("auth", _loginModel.Token)
                                            .GetAsync()
                                            .ReceiveJson<DiaryModel>();
                if (diaryModel.Diary != null)
                {
                    ExportDiaryStatusEvent?.Invoke($"获取日记成功 date:{date} id: {diaryModel.Diary.Id}");

                }
                ResetTryCount();
                return diaryModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _currentTryCount++;
                ExportDiaryStatusEvent?.Invoke($"获取上一篇日记失败...正在 {_currentTryCount} 尝试");
                await Task.Delay(_currentTryCount * 1000);

                if (_currentTryCount < _maxTryCount)
                {
                    return await GetDiaryByDate(date);
                }
                ExportDiaryStatusEvent?.Invoke($"获取上一篇日记失败...终止请求");
                ResetTryCount();
                return null;
            }
        }
    }
}
