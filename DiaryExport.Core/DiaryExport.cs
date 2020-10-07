using Flurl;
using Flurl.Http;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiaryExport.Core
{
    public class DiaryExport
    {
        private readonly string _email;
        private readonly string _password;
        private readonly int _exportCount;
        private readonly string _csrfmiddlewaretoken = "ZcJkWtMdfyjBNKxp3ms0i8REkTJcSKw4";
        private readonly string _baseUrl = "https://ohshenghuo.com/api";
        private readonly int _timeout = 4000; // 4 秒
        private string _token;
        private int _currentExportCount = 0;  // 存在被删除的日记，但是还是会有数据返回，不应该添加。

        private DiaryInfo currentDiaryInfo = new DiaryInfo();
        private string currentDiaryInfoId = "";
        private ExportDiaryInfosModel diaryInfos = new ExportDiaryInfosModel();
        private ExportDiaryInfosWithUserInfoModel diaryInfosWithUserInfo = new ExportDiaryInfosWithUserInfoModel();
        private UserModel userModel = new UserModel();
        public bool LoginCheck { get; set; } = false;

        private List<int> httpStatusCodesWorthRetrying = new List<int>(new[] { 408, 500, 502, 503, 504 });
        public DiaryExport(string email, string password, int exportCount = int.MaxValue)

        {
            _email = email;
            _password = password;
            _exportCount = exportCount;


            FlurlHttp.Configure(settings =>
            {
                settings.Timeout = new TimeSpan(0, 0, 5);
            });
        }

        public async Task<T> RetryPolicy<T>(Action<int> consoleLog, Func<Task<T>> request)
        {
            var retryCount = 0;
            T data;
            data = await Policy
                .Handle<FlurlHttpException>(ex =>
                    httpStatusCodesWorthRetrying.Contains((int)ex.Call.Response.StatusCode))
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                })
                .ExecuteAsync(() =>
                {

                    retryCount++;
                    consoleLog(retryCount);
                    return request();
                });
            return data;
        }
        public async Task Login()
        {
            var data = new
            {
                csrfmiddlewaretoken = _csrfmiddlewaretoken,
                email = _email,
                password = _password
            };

            userModel = await RetryPolicy<UserModel>((tryCount) => Console.WriteLine($"第 {tryCount} 次尝试登陆"), () => new Url(_baseUrl).AppendPathSegment("/login/").PostUrlEncodedAsync(data)
                 .ReceiveJson<UserModel>());

            if (userModel != null && userModel.Token != null)
            {
                _token = $@"token {userModel.Token}";
                Console.WriteLine("登陆成功");
                LoginCheck = true;
            }
            else
            {
                Console.WriteLine("登陆成功，请检查账号密码或查看错误信息");
                LoginCheck = false;
            }
        }

        public async Task<DiaryModel> GetLatestDiary()
        {
            var diaryModel = await RetryPolicy<DiaryModel>((tryCount) => Console.WriteLine($"第 {tryCount} 次尝试获取最新一篇日记"), () => new Url(_baseUrl).AppendPathSegment("/diary/latest/").WithHeader("auth", _token).GetJsonAsync<DiaryModel>());

            return diaryModel;
        }

        public async Task<DiaryModel> GetLatestDiary(string token)
        {
            var diaryModel = await RetryPolicy<DiaryModel>((tryCount) => Console.WriteLine($"第 {tryCount} 次尝试获取最新一篇日记"), () => new Url(_baseUrl).AppendPathSegment("/diary/latest/").WithHeader("auth", token).GetJsonAsync<DiaryModel>());

            return diaryModel;
        }

        public async Task<DiaryModel> GetDiaryByDate(string date)
        {
            var diaryModel = await new Url(_baseUrl).AppendPathSegment("/diary/").SetQueryParam("date", date).WithHeader("auth", _token).GetJsonAsync<DiaryModel>();
            return diaryModel;
        }
        public async Task<DiaryModel> GetDiaryByDate(string token, string date)
        {
            var diaryModel = await new Url(_baseUrl).AppendPathSegment("/diary/").SetQueryParam("date", date).WithHeader("auth", token).GetJsonAsync<DiaryModel>();
            return diaryModel;
        }

        public async Task<DiaryModel> GetDiaryByPrev(string id)
        {

            var diaryModel = await RetryPolicy<DiaryModel>((tryCount) => Console.WriteLine($"基于 ID 为 {id} 的日记，第 {tryCount} 次尝试获取前一篇日记"), () => new Url(_baseUrl).AppendPathSegment($"/diary/prev/{id}/").WithHeader("auth", _token).GetJsonAsync<DiaryModel>());

            return diaryModel;

        }
        public async Task<DiaryModel> GetDiaryByPrev(string token, string id)
        {

            var diaryModel = await RetryPolicy<DiaryModel>((tryCount) => Console.WriteLine($"基于 ID 为 {id} 的日记，第 {tryCount} 次尝试获取前一篇日记"), () => new Url(_baseUrl).AppendPathSegment($"/diary/prev/{id}/").WithHeader("auth", token).GetJsonAsync<DiaryModel>());

            return diaryModel;
        }

        public async Task<ExportDiaryInfosModel> ExportAllDiaryInfo()
        {

            var latestDiary = await GetLatestDiary();
            currentDiaryInfo = latestDiary.Diary;
            if (currentDiaryInfo != null && _currentExportCount < _exportCount)
            {
                _currentExportCount++;
                currentDiaryInfoId = currentDiaryInfo.Id;
                diaryInfos.DiaryInfos.Add(currentDiaryInfo);
                Console.WriteLine($"成功获取第 {_currentExportCount} 篇日记 Id: {currentDiaryInfoId}");
                await Task.Delay(new TimeSpan(0, 0, 0, 0, 500));
            }
            while (currentDiaryInfo != null && _currentExportCount < _exportCount)
            {
                var diaryPrev = await GetDiaryByPrev(currentDiaryInfoId);
                currentDiaryInfo = diaryPrev.Diary;



                if (currentDiaryInfo != null)
                {
                    _currentExportCount++;
                    currentDiaryInfoId = currentDiaryInfo.Id;
                    if (currentDiaryInfo.Content.Equals("deleted") || !currentDiaryInfo.Deleteddate.Equals("None"))
                    {
                        _currentExportCount--;
                        Console.WriteLine($" ID: {currentDiaryInfo.Id } 的日记已被删除，获取下一篇日记");
                        continue;
                    }
                    diaryInfos.DiaryInfos.Add(currentDiaryInfo);
                    Console.WriteLine($"成功获取第 {_currentExportCount} 篇日记 Id: {currentDiaryInfoId}");
                    await Task.Delay(new TimeSpan(0, 0, 0, 0, 500));
                }
                else
                {
                    break;
                }
            }

            return diaryInfos;
        }

        public async Task<ExportDiaryInfosModel> ExportAllDiaryInfo(Action<string> UiTip)
        {

            var latestDiary = await GetLatestDiary();
            currentDiaryInfo = latestDiary.Diary;
            if (currentDiaryInfo != null && _currentExportCount < _exportCount)
            {
                _currentExportCount++;
                currentDiaryInfoId = currentDiaryInfo.Id;
                diaryInfos.DiaryInfos.Add(currentDiaryInfo);
                Console.WriteLine($"成功获取第 {_currentExportCount} 篇日记 Id: {currentDiaryInfoId}");
                UiTip($"成功获取第 {_currentExportCount} 篇日记 Id: {currentDiaryInfoId}");
                await Task.Delay(new TimeSpan(0, 0, 0, 0, 500));
            }
            while (currentDiaryInfo != null && _currentExportCount < _exportCount)
            {
                var diaryPrev = await GetDiaryByPrev(currentDiaryInfoId);
                currentDiaryInfo = diaryPrev.Diary;

                if (currentDiaryInfo != null)
                {
                    _currentExportCount++;
                    currentDiaryInfoId = currentDiaryInfo.Id;
                    if (currentDiaryInfo.Content.Equals("deleted") || !currentDiaryInfo.Deleteddate.Equals("None"))
                    {
                        _currentExportCount--;
                        Console.WriteLine($" ID: {currentDiaryInfo.Id } 的日记被您删除，获取下一篇日记");
                        UiTip($" ID: {currentDiaryInfo.Id} 的日记被您删除，获取下一篇日记");
                        continue;
                    }
                    diaryInfos.DiaryInfos.Add(currentDiaryInfo);
                    Console.WriteLine($"成功获取第 {_currentExportCount} 篇日记 Id: {currentDiaryInfoId}");
                    UiTip($"成功获取第 {_currentExportCount} 篇日记 Id: {currentDiaryInfoId}");
                    await Task.Delay(new TimeSpan(0, 0, 0, 0, 500));
                }
                else
                {
                    break;
                }
            }

            return diaryInfos;
        }

        public async Task<ExportDiaryInfosWithUserInfoModel> ExportAllDiaryInfoWithUserInfo()
        {
            var diaryInfos = await ExportAllDiaryInfo();

            diaryInfosWithUserInfo.UserInfo = userModel.User_config;
            diaryInfosWithUserInfo.DiaryInfos = diaryInfos.DiaryInfos;

            return diaryInfosWithUserInfo;
        }
    }
}