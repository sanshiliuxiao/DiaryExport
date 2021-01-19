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
        private readonly DiaryContext _diaryContext;
        private readonly string _baseUrl = "https://ohshenghuo.com/api";
        private List<int> httpStatusCodesWorthRetrying = new List<int>(new[] { 408, 500, 502, 503, 504 });
        public int Timeout { get; set; } = 4000; // 4 s
        public UserModel User { get; set; } = new UserModel();

        public delegate void TipDelegate(String tip);
        public event TipDelegate TipEvent = Console.WriteLine;


        // Declare the delegate 
        public delegate void LoginEventHandler(string tip);
        public event LoginEventHandler LoginEvent = Console.WriteLine;

        public delegate void ExportDiaryStatusEventHandler(string tip);
        public event ExportDiaryStatusEventHandler ExportDiaryStatusEvent = Console.WriteLine;

        public ExportDiaryServies(LoginModel loginModel,DiaryContext diaryContext)
        {
            _loginModel = loginModel;
            _diaryContext = diaryContext;
        }
        public async Task<UserModel> Login()
        {
            var postData = new
            {
                csrfmiddlewaretoken = _loginModel.Csrf,
                email = _loginModel.Email,
                password = _loginModel.Password
            };
            Func<Task<IFlurlResponse>> func = () => new Flurl.Url(_baseUrl).AppendPathSegment("/login/").PostUrlEncodedAsync(postData);

            User = await RetryPolicy(func).ReceiveJson<UserModel>();

            if (User.Token != "token ")
            {
                _loginModel.Token += User.Token;
                LoginEvent?.Invoke("登录成功");
                ExportDiaryStatusEvent?.Invoke("登录成功");
                return User;
            }
            else
            {
                LoginEvent?.Invoke("登陆失败，请检测账号密码或网络");
                ExportDiaryStatusEvent?.Invoke("登陆失败，请检测账号密码或网络");
                return User;
            }
        }
        public async Task<DiaryModel> GetLatestDiary()
        {
            Func<Task<IFlurlResponse>> func = () => new Flurl.Url(_baseUrl)
                                                        .AppendPathSegment("/diary/latest/")
                                                        .WithHeader("auth", User.Token).GetAsync();
            var diaryModel = await RetryPolicy(func).ReceiveJson<DiaryModel>();

            ExportDiaryStatusEvent?.Invoke($"获取最新一篇日记成功：id: {diaryModel.Diary.Id}");
            return diaryModel;
        }

        public async Task<DiaryModel> GetDiaryByPrev(string id)
        {
            Func<Task<IFlurlResponse>> func = () => new Url(_baseUrl).AppendPathSegment($"/diary/prev/{id}/").WithHeader("auth", User.Token).GetAsync();
            var diaryModel = await RetryPolicy(func).ReceiveJson<DiaryModel>();

            ExportDiaryStatusEvent?.Invoke($"获取上一篇日记成功：id: {diaryModel.Diary.Id}");
            return diaryModel;
        }

        public async Task<TOut> RetryPolicy<TOut>(Func<Task<TOut>> func)
        {
            var retryCount = 0;
            TOut data;
            data = await Policy
                .Handle<FlurlHttpException>(ex =>
                {
                    if (ex.Call.Response == null)
                    {
                        return true;
                    }

                    return httpStatusCodesWorthRetrying.Contains((int)ex.Call.Response.StatusCode);
                })
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(6)
                })
                .ExecuteAsync(() =>
                {
                    retryCount++;

                    ExportDiaryStatusEvent?.Invoke($"正在第 { retryCount.ToString() }次尝试");
                    return func();
                });
            return data;
        }

    }
}
