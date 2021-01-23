using DiaryExport.Commands;
using DiaryExport.EFCore;
using DiaryExport.ModelDtos;
using DiaryExport.Models;
using DiaryExport.Servies;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiaryExport.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private  LoginModel _loginModel;
        public LoginModel LoginModel
        {
            get
            {
                return _loginModel;
            }
            set
            {
                _loginModel = value;
                OnPropertyChanged(nameof(LoginModel));
            }

        }
        private readonly DbServices _dbServices;
        private readonly ExportDiaryServices _exportDiaryServies;
        public UserModel _userModel = new UserModel();
        public UserModel UserModel
        {
            get
            {
                return _userModel;
            }
            set
            {
                _userModel = value;
                OnPropertyChanged(nameof(UserModel));

            }
        }
        private string _loginTip;
        public string LoginTip 
        {
            get
            {
                return _loginTip;
            }
            set
            {
                _loginTip = value;
                OnPropertyChanged(nameof(LoginTip));

            }
        }
        private bool _ifLogined;
        private string _exportPath = AppDomain.CurrentDomain.BaseDirectory;
        public string ExportPath
        {
            get { return _exportPath; }
            set 
            {
                _exportPath = value;
                OnPropertyChanged(nameof(ExportPath));
            }
        }

        private int _exportedCount = 0;
        public int ExportedCount
        {
            get { return _exportedCount; }
            set
            {
                _exportedCount = value;
                OnPropertyChanged(nameof(ExportedCount));
            }
        }

        private int _deletedCount = 0;
        public int DeletedCount
        {
            get { return _deletedCount; }
            set
            {
                _deletedCount = value;
                OnPropertyChanged(nameof(DeletedCount));
            }
        }
        private int _existedCount = 0;
        public int ExistedCount
        {
            get { return _existedCount; }
            set
            {
                _existedCount = value;
                OnPropertyChanged(nameof(ExistedCount));
            }
        }
        public int _finallyCount = 0;
        public int FinallyCount
        {
            get { return _finallyCount; }
            set
            {
                _finallyCount = value;
                OnPropertyChanged(nameof(FinallyCount));
            }
        }

        private DiaryInfo _currentExportDiaryInfo = new DiaryInfo();
        private int _currentExportDiaryCount = 0;
        public int CurrentExportDiaryCount
        {
            get { return _currentExportDiaryCount; }
            set
            {
                _currentExportDiaryCount = value;
                OnPropertyChanged(nameof(CurrentExportDiaryCount));
            }
        }

        public bool _ifToBreakExportDiary ;
        public bool _ifExporting;

        public ICommand LoginCommand { get; set; }
        public ICommand SelectExportFolderCommand { get; set; }
        public ICommand ExportToFileCommand { get; set; }
        public ICommand ReExportSqliteCommand { get; set; }
        public ICommand InExportSqliteCommand { get; set; }
        public RelayCommand CoExportSqliteCommand { get; private set; }
        public ICommand BrExportSqliteCommand { get; set; }

        private ObservableCollection<string> _exportDiaryStatus = new ObservableCollection<string> { };
        public ObservableCollection<string> ExportDiaryStatus
        {
            get { return _exportDiaryStatus; }
            set
            {
                _exportDiaryStatus = value;
                OnPropertyChanged(nameof(ExportDiaryStatus));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public event Action AutoScrollEvent;

        public MainViewModel()
        {
            LoginModel = new LoginModel(string.Empty, string.Empty);
            _dbServices = new DbServices(new DiaryContext());
            _exportDiaryServies = new ExportDiaryServices(_loginModel);
            _exportDiaryServies.LoginEvent += (tip) => { LoginTip = tip; };
            _exportDiaryServies.ExportDiaryStatusEvent += (tip) =>{ Application.Current.Dispatcher.Invoke(() => { ExportDiaryStatusChange(tip); }); };
            LoginCommand = new RelayCommand(ToLogin);
            SelectExportFolderCommand = new RelayCommand(SelectExportFolder);
            ExportToFileCommand = new RelayCommand(ExportToFile);
            ReExportSqliteCommand = new RelayCommand(ReExportSqlite);
            InExportSqliteCommand = new RelayCommand(InExportSqlite);
            CoExportSqliteCommand = new RelayCommand(CoExportSqlite);
            BrExportSqliteCommand = new RelayCommand(BrExportSqlite);
        }

        private void BrExportSqlite()
        {
            MessageBoxResult dr = MessageBox.Show("是否终止", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                _ifToBreakExportDiary = true;
                RevExporting();
            }
        }
        private void BrRevExportSqlite()
        {
            _ifToBreakExportDiary = false;
        }
        private void Exporting()
        {
            _ifExporting = true;
        }
        private void RevExporting()
        {
            _ifExporting = false;
        }

        private void ExportDiaryStatusChange(string tip)
        {
            ExportDiaryStatus.Add(tip);
            AutoScrollEvent?.Invoke();
        }
        private void ExportDiaryStatusClear()
        {
            ExportDiaryStatus.Clear();
            AutoScrollEvent?.Invoke();
        }
        private async void InExportSqlite()
        {
            if (!_ifLogined)
            {
                ExportDiaryStatusChange("请先登陆");
                MessageBox.Show("请先登陆");
                return;
            }
            if (_ifExporting)
            {
                MessageBox.Show("请先手动终止，再次点击");
                return;
            }
            Exporting();
            ExportDiaryStatusClear();


            // 获取到 最新一篇日记 // 获取到数据库里的第一篇日记,进行 id 比对
            var latestdiaryModel = await _exportDiaryServies.GetLatestDiary();
            var latestDairyModelFromSqlite = await _dbServices.GetLatestDiary(UserModel.UserConfig.Id);

            if (latestdiaryModel.Diary.Id == latestDairyModelFromSqlite.Id)
            {
                ExportDiaryStatusChange("最新日记 ID 比对成功， 无需增量更新");
                RevExporting();
            }
            else
            {
                _currentExportDiaryInfo = latestdiaryModel.Diary;
                await _dbServices.AddOneDiaryInfoAsync(_currentExportDiaryInfo);
                ExportDiaryStatusChange("保存最新日记，正在增量导出");
                do
                {
                    if (_ifToBreakExportDiary)
                    {
                        ExportDiaryStatusChange($"手动终止导出");
                        BrRevExportSqlite();
                        return;
                    }
                    DiaryModel diaryModel = await _exportDiaryServies.GetDiaryPrevById(_currentExportDiaryInfo.Id);


                    if (diaryModel.Diary.Id == latestDairyModelFromSqlite.Id)
                    {
                        ExportDiaryStatusChange($"增量更新完成");
                        RevExporting();
                        return;
                    }

                    if (diaryModel == null || diaryModel.Diary == null)
                    {

                        ExportDiaryStatusChange("导出结束");
                        MessageBox.Show("导出结束");
                        RevExporting();
                        return;
                    }
                    _currentExportDiaryInfo = diaryModel.Diary;
                    CurrentExportDiaryCount++;
                    await _dbServices.AddOneDiaryInfoAsync(_currentExportDiaryInfo);
                    ExportedCount = _dbServices.GetCurrentUserInSqliteDiaryCount(UserModel.UserConfig.Id);
                    DeletedCount = _dbServices.GetCurrentUserInSqliteFilteDeletedDiaryCount(UserModel.UserConfig.Id, "deleted");
                    FinallyCount = ExportedCount - DeletedCount;
                    ExportDiaryStatusChange($"正在导出第 {CurrentExportDiaryCount} 篇， 累计导出 {ExportedCount} 篇");
                    await Task.Delay(new TimeSpan(0, 0, 0, 0, 500));


                } while (_currentExportDiaryInfo != null && CurrentExportDiaryCount < LoginModel.ExportCount);
            }


        }

        private async void ReExportSqlite()
        {

            if (!_ifLogined)
            {
                ExportDiaryStatusChange("请先登陆");
                MessageBox.Show("请先登陆");
                return;
            }
            if (_ifExporting)
            {
                MessageBox.Show("请先手动终止，再次点击");
                return;
            }
            Exporting();
            ExportDiaryStatusClear();
            if (ExportedCount > 0)
            {
                MessageBoxResult dr = MessageBox.Show("此操作将会清空已导出的日记，确认继续？", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr != MessageBoxResult.OK)
                {
                    RevExporting();
                    BrRevExportSqlite();
                    return;
                }
                await _dbServices.DeleteAllDiaryInfoAsync(UserModel.UserConfig.Id);
            }
            do
            {
                if (_ifToBreakExportDiary)
                {
                    ExportDiaryStatusChange($"手动终止导出");
                    BrRevExportSqlite();
                    break;
                }
                DiaryModel diaryModel;
                if (CurrentExportDiaryCount == 0)
                {
                    diaryModel = await _exportDiaryServies.GetLatestDiary();
                }
                else
                {
                    diaryModel = await _exportDiaryServies.GetDiaryPrevById(_currentExportDiaryInfo.Id);
                }

                if (diaryModel == null || diaryModel.Diary == null)
                {
                    ExportDiaryStatusChange("导出结束");
                    MessageBox.Show("导出结束");
                    RevExporting();
                    return;
                }
                _currentExportDiaryInfo = diaryModel.Diary;
                CurrentExportDiaryCount++;
                await _dbServices.AddOneDiaryInfoAsync(_currentExportDiaryInfo);
                ExportedCount = _dbServices.GetCurrentUserInSqliteDiaryCount(UserModel.UserConfig.Id);
                DeletedCount = _dbServices.GetCurrentUserInSqliteFilteDeletedDiaryCount(UserModel.UserConfig.Id, "deleted");
                FinallyCount = ExportedCount - DeletedCount;
                ExportDiaryStatusChange($"正在导出第 {CurrentExportDiaryCount} 篇， 累计导出 {ExportedCount} 篇");
                await Task.Delay(new TimeSpan(0, 0, 0, 0, 500));
            } while (_currentExportDiaryInfo != null && CurrentExportDiaryCount < LoginModel.ExportCount);

        }
        private async void CoExportSqlite()
        {
            // 继续上此的任务
            if (!_ifLogined)
            {
                ExportDiaryStatusChange("请先登陆");
                MessageBox.Show("请先登陆");
                return;
            }
            if (_ifExporting)
            {
                MessageBox.Show("请先手动终止，再次点击");
                return;
            }
            Exporting();
            ExportDiaryStatusClear();
            // 获取到数据库里的日期最早的一篇日记
            var latestDairyModelFromSqlite = await _dbServices.GetOldestDiary(UserModel.UserConfig.Id);
            if (latestDairyModelFromSqlite != null)
            {
                DiaryInfo _currentExportDiaryInfo = latestDairyModelFromSqlite;
                do
                {
                    if (_ifToBreakExportDiary)
                    {
                        ExportDiaryStatusChange($"手动终止导出");
                        BrRevExportSqlite();
                        return;
                    }
                    DiaryModel diaryModel = await _exportDiaryServies.GetDiaryPrevById(_currentExportDiaryInfo.Id);
                    if (diaryModel == null || diaryModel.Diary == null)
                    {
                        ExportDiaryStatusChange("导出结束");
                        MessageBox.Show("导出结束");
                        RevExporting();
                        return;
                    }
                    _currentExportDiaryInfo = diaryModel.Diary;
                    CurrentExportDiaryCount++;
                    await _dbServices.AddOneDiaryInfoAsync(_currentExportDiaryInfo);
                    ExportedCount = _dbServices.GetCurrentUserInSqliteDiaryCount(UserModel.UserConfig.Id);
                    DeletedCount = _dbServices.GetCurrentUserInSqliteFilteDeletedDiaryCount(UserModel.UserConfig.Id, "deleted");
                    FinallyCount = ExportedCount - DeletedCount;
                    ExportDiaryStatusChange($"正在导出第 {CurrentExportDiaryCount} 篇， 累计导出 {ExportedCount} 篇");
                    await Task.Delay(new TimeSpan(0, 0, 0, 0, 500));
                } while (_currentExportDiaryInfo != null);
            }
        }
        private async void ExportToFile()
        {
            if (!_ifLogined)
            {
                ExportDiaryStatusChange("请先登陆");
                MessageBox.Show("请先登陆");
                return;
            }
            if (_ifExporting)
            {
                MessageBox.Show("请先手动终止，再次点击");
                return;
            }
            var count = _dbServices.GetCurrentUserInSqliteDiaryCount(UserModel.UserConfig.Id);
            if (count == 0)
            {
                MessageBox.Show("请先导出至 Sqlite 数据库");
                return;
            }
            // 获取所有数据并导出到指定文件夹 

            var exportJsonFilePath = $@"{_exportPath}\exportDiary.json";
            var exportTxtFilePath = $@"{_exportPath}\exportDiary.txt";
            var data = await _dbServices.GetCurrentUserInSqliteAllData(UserModel.UserConfig.Id);

            ExportDiaryToJsonFile(exportJsonFilePath, data);
            ExportDiaryToTxtFile(exportTxtFilePath, data);

            ExportDiaryStatusChange("已导出为 JSON 文件");

            ExportDiaryStatusChange("已导出为 TXT 文件");

            MessageBox.Show($"已导出: {_exportPath}");

            AutoScrollEvent?.Invoke();
        }

        private void ExportDiaryToJsonFile(string path, List<DiaryInfo> data)
        {

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
        private void ExportDiaryToTxtFile(string path, List<DiaryInfo> data)
        {
            var txt = "";
            foreach (var diaryInfo in data)
            {
                if (diaryInfo.Deleteddate.Equals("None") || !diaryInfo.Content.Equals("deleted"))
                {
                    txt += Environment.NewLine;
                    txt += $"标题: {diaryInfo.Title}" + Environment.NewLine;
                    txt += $"创建日期: {diaryInfo.Createddate}" + Environment.NewLine;
                    txt += $"更新日期: {diaryInfo.Ts}" + Environment.NewLine;
                    txt += $"内容： " + Environment.NewLine + Environment.NewLine;
                    txt += diaryInfo.Content + Environment.NewLine + Environment.NewLine;
                    txt += "----------我是分隔符----------" + Environment.NewLine;
                    txt += Environment.NewLine;
                }

            }
            File.WriteAllText(path, txt, Encoding.UTF8);
        }

        private void SelectExportFolder()
        {
            var ookiiDialog = new VistaFolderBrowserDialog();
            if (ookiiDialog.ShowDialog() == true)
            {
                ExportPath = ookiiDialog.SelectedPath;
            }
        }

        public async void ToLogin()
        {
            if (LoginModel.Email.Equals(string.Empty) || LoginModel.Password.Equals(string.Empty))
            {
                MessageBox.Show("请输入账号密码");
                return;
            }

            ExportDiaryStatusChange("正在登陆中...");
            LoginTip = "正在登陆中...";

            _exportDiaryServies.ResetTryCount();
            UserModel = await _exportDiaryServies.Login();
            if (UserModel == null)
            {
                LoginTip = "请检查账号密码及网络...";
                return;
            }
            if (UserModel.UserConfig != null)
            {
                ExportedCount = _dbServices.GetCurrentUserInSqliteDiaryCount(UserModel.UserConfig.Id);
                DeletedCount = _dbServices.GetCurrentUserInSqliteFilteDeletedDiaryCount(UserModel.UserConfig.Id,"deleted");
                FinallyCount = ExportedCount - DeletedCount;
                ExistedCount = UserModel.UserConfig.DiaryCount;
                CurrentExportDiaryCount = 0;
                _ifLogined = true;
                var user = _dbServices.GetUserInfoById(UserModel.UserConfig.Id);
                if (user == null)
                {
                    await _dbServices.AddOneUserInfoAsync(UserModel.UserConfig);
                    ExportDiaryStatusChange("用户信息已存入 SQLite 数据库");
                }
                else
                {
                    var curUser = UserModel.UserConfig;
                    user.Id = curUser.Id;
                    user.Name = curUser.Name;
                    user.UserEmail = curUser.UserEmail;
                    user.WordCount = curUser.WordCount;
                    user.Avatar = user.Avatar;
                    user.Description = user.Description;
                    user.DiaryCount = user.DiaryCount;
                    await _dbServices.UpdateOneUserInfoAsync(user);
                }
                ExportedCount = _dbServices.GetCurrentUserInSqliteDiaryCount(user.Id);
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
