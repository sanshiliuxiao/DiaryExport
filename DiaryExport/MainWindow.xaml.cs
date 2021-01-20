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
        public MainWindow()
        {
            InitializeComponent();
            var mainViewModel = new MainViewModel();
            mainViewModel.AutoScrollEvent += AutoScroll;
            this.MainNode.DataContext = mainViewModel;
        }
        /// <summary>
        /// 滚动条自动滚动
        /// </summary>
        private void AutoScroll()
        {
            if (StatusList.Items.Count != 0)
            {
                StatusList.ScrollIntoView(StatusList.Items[StatusList.Items.Count - 1]);
            }
        }

    }
}
