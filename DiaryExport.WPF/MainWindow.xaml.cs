using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DiaryExport.Core;
using DiaryExport.ExportDiaryToFile;
using Ookii.Dialogs.Wpf;


namespace DiaryExport.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool blockMultipleClick = false;
        private string txtExportPath = string.Empty;
        private string jsonExportPath = string.Empty;
        private MainViewModel mainViewModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.MainNode.DataContext = mainViewModel;
            mainViewModel.FocusLastItem += AutoScroll;
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
                txtExportPath = $@"{ookiiDialog.SelectedPath}\ExportDiary.txt";
                jsonExportPath = $@"{ookiiDialog.SelectedPath}\ExportDiary.json";
            }

        }

        private void ResetAllStatus(object sender, RoutedEventArgs e)
        {
            Email.Text = "";
            Password.Password = "";
            ExportCount.Text = "";
            ExportPath.Text = "";
            LoginTip.Text = "";

            blockMultipleClick = false;
            txtExportPath = "";
            jsonExportPath = "";
        }
        private async void StartExportDiary(object sender, RoutedEventArgs e)
        {
            if (Email.Text.Equals(string.Empty) || Password.Password.Equals(string.Empty))
            {
                MessageBox.Show("请输入账号密码");
                return;
            }
            if (ExportPath.Text == "" || txtExportPath.Equals(string.Empty) || jsonExportPath.Equals(string.Empty))
            {
                MessageBox.Show("请选择导出路径：");
                return;
            }

            var email = Email.Text.Trim();
            var password = Password.Password.Trim();
            var count = 0; 
            int.TryParse(ExportCount.Text, out count);
            count= count == 0 ? int.MaxValue : count;
            var diaryInfoList = new ExportDiaryInfosModel();
            var diaryInfoListWithUserInfo = new ExportDiaryInfosWithUserInfoModel();
            var diaryExport = new Core.DiaryExport(email, password, count);
            await diaryExport.Login();

            if (diaryExport.LoginCheck)
            {
                if (blockMultipleClick)
                {
                    MessageBox.Show("正在导出，请勿点击");
                    return;
                }

                blockMultipleClick = true;

                LoginTip.Text = "登陆成功，准备导出";
                diaryInfoList = await diaryExport.ExportAllDiaryInfo(mainViewModel.AddItemToStatus);
                
                if (diaryInfoList != null && diaryInfoList.DiaryInfos.Count != 0)
                {
                    var exportDiaryToTxt = new ExportDiaryToTxtFile(txtExportPath, diaryInfoList);
                    exportDiaryToTxt.StartExport();

                    var exportDiaryToJson = new ExportDiaryToJsonFile(jsonExportPath, diaryInfoList);
                    exportDiaryToJson.StartExport();
                }

                blockMultipleClick = false;
                LoginTip.Text = "导出完毕";

            }
            else
            {
                LoginTip.Text = "登陆失败，请检查账号密码及网络";
            }

        }
    }
}
