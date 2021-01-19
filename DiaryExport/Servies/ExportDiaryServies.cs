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
    public class ExportDiaryServies
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

        public ExportDiaryServies(LoginModel loginModel)
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
                _loginModel.Token += User.Token;
                LoginEvent?.Invoke("登录成功");
                ExportDiaryStatusEvent?.Invoke("登录成功");
                ResetTryCount();
                return User;
  
            }
            catch (Exception)
            {
                LoginEvent?.Invoke("登陆失败，请检测账号密码或网络");
                ExportDiaryStatusEvent?.Invoke("登陆失败，请检测账号密码或网络");
                ExportDiaryStatusEvent?.Invoke("正在重新尝试登录");
                await Task.Delay(_currentTryCount * 1000);
                _currentTryCount++;
                if (_currentTryCount < _maxTryCount)
                {
                    return await Login();
                  
                }
                ExportDiaryStatusEvent?.Invoke("彻底的失败了！");
                return null;
            }
        }
        public async Task<DiaryModel> GetLatestDiary()
        {
            try
            {
                Func<Task<IFlurlResponse>> func = () => new Flurl.Url(_baseUrl)
                                                        .AppendPathSegment("/diary/latest/")
                                                        .WithHeader("auth", User.Token).GetAsync();
                var diaryModel = await func().ReceiveJson<DiaryModel>();
                ExportDiaryStatusEvent?.Invoke($"获取最新一篇日记成功：id: {diaryModel.Diary.Id}");
                ResetTryCount();
                return diaryModel;
            }
            catch (Exception)
            {
                ExportDiaryStatusEvent?.Invoke($"获取最新一篇日记失败...正在重新获取");
                await Task.Delay(_currentTryCount * 1000);
                _currentTryCount++;
                if (_currentTryCount < _maxTryCount)
                {
                    return await GetLatestDiary();
                }
                ExportDiaryStatusEvent?.Invoke($"获取最新一篇日记失败...终止获取");
                return null;
            }
        }
        public async Task<DiaryModel> GetDiaryByPrev(string id)
        {
            Func<Task<IFlurlResponse>> func = () => new Url(_baseUrl).AppendPathSegment($"/diary/prev/{id}/").WithHeader("auth", User.Token).GetAsync();

            try
            {
                var diaryModel = await func().ReceiveJson<DiaryModel>();
                ExportDiaryStatusEvent?.Invoke($"获取上一篇日记成功：id: {diaryModel.Diary.Id}");
                ResetTryCount();
                return diaryModel;
            }
            catch (Exception)
            {
                ExportDiaryStatusEvent?.Invoke($"获取上一篇日记失败...正在重新请求");
                await Task.Delay(_currentTryCount * 1000);
                _currentTryCount++;
                if (_currentTryCount < _maxTryCount)
                {
                    return await GetDiaryByPrev(id);
                }
                ExportDiaryStatusEvent?.Invoke($"获取上一篇日记失败...终止请求");
                return null;
            }

        }
    }
}
