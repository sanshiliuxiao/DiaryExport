using DiaryExport.Commands;
using DiaryExport.EFCore;
using DiaryExport.ModelDtos;
using DiaryExport.Models;
using DiaryExport.Servies;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        private readonly ExportDiaryServies _exportDiaryServies;
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

        private int _allExportCount = 0;
        public int AllExportCount
        {
            get { return _allExportCount; }
            set 
            {
                _allExportCount = value;
                OnPropertyChanged(nameof(AllExportCount));
            }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand SelectExportFolderCommand { get; set; }
        public ICommand ExportToFileCommand { get; set; }
        public ICommand ReExportSqliteCommand { get; set; }
        public ICommand CoExportSqliteCommand { get; set; }

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

        public MainViewModel()
        {
            LoginModel = new LoginModel
            {
                Email = "",
                Password = "",
                ExportCount = int.MaxValue,
                Count = 0,
            };
            _dbServices = new DbServices(new DiaryContext());
            _exportDiaryServies = new ExportDiaryServies(_loginModel);
            _exportDiaryServies.LoginEvent += (tip) => { LoginTip = tip; };
            _exportDiaryServies.ExportDiaryStatusEvent += (tip) =>{ Application.Current.Dispatcher.Invoke(() => { ExportDiaryStatus.Add(tip); }); };
            LoginCommand = new RelayCommand(ToLogin);
            SelectExportFolderCommand = new RelayCommand(SelectExportFolder);
            ExportToFileCommand = new RelayCommand(ExportToFile);
            ReExportSqliteCommand = new RelayCommand(ReExportSqlite);
            CoExportSqliteCommand = new RelayCommand(CoExportSqlite);
        }

        private void CoExportSqlite()
        {
            if (!_ifLogined)
            {
                ExportDiaryStatus.Add("请先登陆");
                MessageBox.Show("请先登陆");
            }
            // 获取到属于 他的日记数量 和 最新的日记
        }

        private void ReExportSqlite()
        {
            if (!_ifLogined)
            {
                ExportDiaryStatus.Add("请先登陆");
                MessageBox.Show("请先登陆");
            }
            // 导出至 Sqlite 
        }

        private void ExportToFile()
        {
            var count = _dbServices.GetCurrentUserInSqliteDiaryCount(UserModel.UserConfig.Id);
            if (count == 0)
            {
                MessageBox.Show("请先导出至 Sqlite 数据库");
                return;
            }
            // 获取所有数据并导出到指定文件夹
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
            if (LoginModel.Count != 0)
            {
                LoginModel.ExportCount = LoginModel.Count;
            }
            else
            {
                LoginModel.ExportCount = int.MaxValue;
            }
            if (LoginModel.Email.Equals(string.Empty) || LoginModel.Password.Equals(string.Empty))
            {
                MessageBox.Show("请输入账号密码");
                return;
            }

            ExportDiaryStatus.Add("正在登陆中...");
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
                var curUser = UserModel.UserConfig;
                _ifLogined = true;
                var user = _dbServices.GetUserInfoById(UserModel.UserConfig.Id);
                if (user == null)
                {
                    await _dbServices.AddOneUserInfoAsync(curUser);
                    ExportDiaryStatus.Add("用户信息已存入 SQLite 数据库");
                }
                else
                {
                    user.Id = curUser.Id;
                    user.Name = curUser.Name;
                    user.UserEmail = curUser.UserEmail;
                    user.WordCount = curUser.WordCount;
                    user.Avatar = user.Avatar;
                    user.Description = user.Description;
                    user.DiaryCount = user.DiaryCount;
                    await _dbServices.UpdateOneUserInfoAsync(user);
                }
                AllExportCount = user.DiaryCount;
                ExportedCount = _dbServices.GetCurrentUserInSqliteDiaryCount(user.Id);
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
