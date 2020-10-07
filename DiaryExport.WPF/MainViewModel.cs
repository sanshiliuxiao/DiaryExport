using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DiaryExport.WPF.Annotations;

namespace DiaryExport.WPF
{
    public class MainViewModel: INotifyPropertyChanged
    {

        private ObservableCollection<string> listStatus = new ObservableCollection<string> { };
        public ObservableCollection<string> ListStatus
        {
            get { return listStatus; }
            set
            {
                listStatus = value;
                OnPropertyChanged(nameof(listStatus));
            }
        }

        public void AddItemToStatus(string strStatus)
        {
            ListStatus.Add(strStatus);
            FocusLastItem();
        }

        /// <summary>
        /// 委托定义，用于控制界面元素
        /// </summary>
        public delegate void ScrollToEnd();
        public ScrollToEnd FocusLastItem = null;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}