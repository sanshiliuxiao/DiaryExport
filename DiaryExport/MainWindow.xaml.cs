using DiaryExport.EFCore;
using DiaryExport.ModelDtos;
using DiaryExport.Models;
using DiaryExport.ViewModels;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiaryExport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string exportPath = String.Empty;
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 滚动条自动滚动
        /// </summary>
        private void AutoScroll()
        {
            StatusList.ScrollIntoView(StatusList.Items[StatusList.Items.Count - 1]);
        }

        private void SettingExportPath(object sender, RoutedEventArgs e)
        {
            var ookiiDialog = new VistaFolderBrowserDialog();
            if (ookiiDialog.ShowDialog() == true)
            {
                ExportPath.Text = ookiiDialog.SelectedPath;
                exportPath = ookiiDialog.SelectedPath;
            }

        }

        private  void Login(object sender, RoutedEventArgs e)
        {
            if (Email.Text.Equals(string.Empty) || Password.Password.Equals(string.Empty))
            {
                MessageBox.Show("请输入账号密码");
                return;
            }
            var loginModel = new LoginModel();

            loginModel.Email = Email.Text.Trim(); ;
            loginModel.Password = Password.Password.Trim();
            var count = 0;
            int.TryParse(ExportCount.Text, out count);
            count = count == 0 ? int.MaxValue : count;
            loginModel.ExportCount = count;

            DiaryContext context = new DiaryContext();
            this.MainNode.DataContext = new MainViewModel(loginModel, context);
        }
        private  void ExportSqlite(object sender, RoutedEventArgs e)
        {
        }
        private void ExportFile(object sender, RoutedEventArgs e)
        {

            if (ExportPath.Text == "" || exportPath.Equals(string.Empty))
            {
                MessageBox.Show("请选择导出路径：");
                return;
            }
        }
    }
}
