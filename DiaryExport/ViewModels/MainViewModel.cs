using DiaryExport.EFCore;
using DiaryExport.ModelDtos;
using DiaryExport.Models;
using DiaryExport.Servies;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace DiaryExport.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private readonly LoginModel _loginModel;
        private readonly DiaryContext _context;
        private readonly ExportDiaryServies _servies;
        public UserModel _userModel;
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

        public MainViewModel(LoginModel loginModel, DiaryContext context)
        {
            _loginModel = loginModel;
            _context = context;

            _servies = new ExportDiaryServies(loginModel, context);

            _servies.LoginEvent += (tip) => { LoginTip = tip; };
            _servies.ExportDiaryStatusEvent += (tip) =>{ ExportDiaryStatus.Add(tip);};
             new Action(async () => {
                 ExportDiaryStatus.Add("正在登陆中...");
                 LoginTip = "正在登陆中...";
                 UserModel = await _servies.Login();

             }).Invoke() ;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
