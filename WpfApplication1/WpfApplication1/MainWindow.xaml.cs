using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.CompilerServices;
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
using WpfApplication1.Database.Context;
using WpfApplication1.Database.Model;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyDatabaseContext myDbContext = new MyDatabaseContext();
        //ObservableCollection
        ObservableCollection<UserProgress> listUserProgresses;
        PopupWaiter pWaiter;
        FootballWindow footballWindow;

        ReportData reportData;

        Task ldb;

        public MainWindow()
        {
            InitializeComponent();
            //
            reportData = new ReportData();
            //
            loadDatabaseWithUI();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        bool _shown = false;

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            //
            if (_shown)
                return;
            //
            loadDatabaseWithUIDone();
        }

        private void updateReport()
        {
            reportData.setTotalMember(listUserProgresses.Count);
            int tD = 0;
            int tK = 0;
            int tN = 0;
            foreach (var lvi in listViewProgress.Items)
            {
                UserProgress up = lvi as UserProgress;
                if (up.IsDa)
                    tD++;
                else if (up.IsKhong)
                    tK++;
                else
                    tN++;
            }
            reportData.setTotalDa(tD);
            reportData.setTotalKhong(tK);
            reportData.setTotalNone(tN);
        }

        private void loadDatabaseWithUI()
        {
            ldb = LoadDatabase();
        }
        private async void loadDatabaseWithUIDone()
        {
            pWaiter = new PopupWaiter();
            pWaiter.Owner = Window.GetWindow(this);
            pWaiter.Show();
            this.IsEnabled = false;
            //
            await ldb;
            listViewProgress.ItemsSource = listUserProgresses;
            this.IsEnabled = true;
            pWaiter.Close();
            //
            updateReport();
            // set value to report
            tabItemStatistic.DataContext = reportData;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //
            this.pWaiter.Close();
        }

        private async Task LoadDatabase()
        {
            await Task.Factory.StartNew(() =>
            {
                ((IObjectContextAdapter)myDbContext).ObjectContext.Refresh(System.Data.Entity.Core.Objects.RefreshMode.StoreWins, myDbContext.Users);
                List<User> list = myDbContext.Users.ToList();
                list.Sort(new Comparison<User>((User user1, User user2) =>
                {
                    if (user1.FirstName == user2.FirstName)
                        return user1.LastName.CompareTo(user2.LastName);
                    return user1.FirstName.CompareTo(user2.FirstName);
                }));
                //
                listUserProgresses = new ObservableCollection<UserProgress>();
                int stt = 1;
                foreach (var user in list)
                {
                    UserProgress.State us;
                    if (user.IsDa) us = UserProgress.State.Da;
                    else if (user.IsKhong) us = UserProgress.State.Khong;
                    else us = UserProgress.State.Chuahoi;
                    listUserProgresses.Add(new UserProgress(user.FirstName, user.LastName, us) { Id = user.Id, SequenceNumber = stt });
                    stt++;
                }
            });
        }

        private void listViewProgress_listViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                var up = item.Content as UserProgress;
                //MessageBox.Show(up.FirstName + " " + up.Progress);
            }
        }

        public class UserProgress : INotifyPropertyChanged
        {
            public int SequenceNumber { get; set; }
            public int Id { get; set; }
            public String FirstName { get; set; }
            public String LastName { get; set; }
            private bool isDa;
            private bool isKhong;
            private bool isNone;

            private void NotifyPropertyChanged(String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            public void ForceNotifyPropertyChanged(String propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public bool IsDa
            {
                get { return isDa; }
                set
                {
                    isDa = value;
                    if (isDa == true)
                    {
                        isKhong = false;
                        isNone = false;
                        NotifyCheckList();
                    }
                }
            }
            public bool IsKhong
            {
                get { return isKhong; }
                set
                {
                    isKhong = value;
                    if (isKhong)
                    {
                        isDa = false;
                        isNone = false;
                        NotifyCheckList();
                    }
                }
            }
            public bool IsNone
            {
                get { return isNone; }
                set
                {
                    isNone = value;
                    if (isNone)
                    {
                        isDa = false;
                        isKhong = false;
                        NotifyCheckList();
                    }
                }
            }

            protected void NotifyCheckList()
            {
                NotifyPropertyChanged("IsDa");
                NotifyPropertyChanged("IsKhong");
                NotifyPropertyChanged("IsNone");
            }

            public UserProgress(string firstName, string lastName, State state)
            {
                this.FirstName = firstName;
                this.LastName = lastName;
                switch (state)
                {
                    case State.Da:
                        IsDa = true;
                        break;
                    case State.Khong:
                        IsKhong = true;
                        break;
                    case State.Chuahoi:
                    default:
                        IsNone = true;
                        break;
                }
            }

            public enum State
            {
                Da = 0,
                Khong = 1,
                Chuahoi = 2,
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public class ReportData : INotifyPropertyChanged
        {
            private int _totalDa;
            private int _totalKhong;
            private int _totalNone;
            private int _totalMember;

            public void setTotalDa(int newValue)
            {
                this._totalDa = newValue;
                notifyChanged("TotalDa");
                notifyChanged("PercentageDa");
            }
            public void setTotalKhong(int newValue)
            {
                this._totalKhong = newValue;
                notifyChanged("TotalKhong");
                notifyChanged("PercentageKhong");
            }
            public void setTotalNone(int newValue)
            {
                this._totalNone = newValue;
                notifyChanged("TotalNone");
                notifyChanged("PercentageNone");
            }
            public void setTotalMember(int newValue)
            {
                this._totalMember = newValue;
                notifyChanged("TotalDa");
                notifyChanged("TotalKhong");
                notifyChanged("TotalNone");
                notifyChanged("PercentageDa");
                notifyChanged("PercentageKhong");
                notifyChanged("PercentageNone");
            }

            public String TotalDa
            {
                get { return _totalDa.ToString("00") + "/" + _totalMember.ToString("00"); }
            }

            public int PercentageDa
            {
                get { return (int)(100.0 * _totalDa / _totalMember); }
            }
            public String TotalKhong
            {
                get { return _totalKhong.ToString("00") + "/" + _totalMember.ToString("00"); }
            }
            public int PercentageKhong
            {
                get { return (int)(100.0 * _totalKhong / _totalMember); }
            }
            public String TotalNone
            {
                get { return _totalNone.ToString("00") + "/" + _totalMember.ToString("00"); }
            }
            public int PercentageNone
            {
                get { return (int)(100.0 * _totalNone / _totalMember); }
            }

            protected void notifyChanged(String PropertyName){
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private bool isDataChanged = false;

        public void button_footballer_click(object sender, EventArgs e)
        {
            footballWindow = new FootballWindow();
            footballWindow.Owner = Window.GetWindow(this);
            footballWindow.UsersUpdate += footballWindow_UsersUpdate;
            footballWindow.Closed += footballWindow_Closed;
            footballWindow.Show();
        }

        void footballWindow_Closed(object sender, EventArgs e)
        {
            if (isDataChanged)
            {
                loadDatabaseWithUI();
                loadDatabaseWithUIDone();
            }
            isDataChanged = false;
        }

        void footballWindow_UsersUpdate(object sender, User user, UsersUpdateAction action)
        {
            switch (action)
            {
                case UsersUpdateAction.Delete:
                    isDataChanged = true;
                    break;
                case UsersUpdateAction.Add:
                    isDataChanged = true;
                    break;
                case UsersUpdateAction.Change:
                    isDataChanged = true;
                    break;
                default:
                    break;
            }
        }

        private void checkBox_isDa_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            UserProgress up = (sender as CheckBox).DataContext as UserProgress;
            User user = myDbContext.Users.First(m => m.Id == up.Id);
            user.IsDa = up.IsDa;
            user.IsKhong = up.IsKhong;
            user.IsNone = up.IsNone;
            //
            myDbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
            myDbContext.SaveChanges();
            //
            updateReport();
        }
        private void checkBox_isKhong_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            UserProgress up = (sender as CheckBox).DataContext as UserProgress;
            User user = myDbContext.Users.First(m => m.Id == up.Id);
            user.IsDa = up.IsDa;
            user.IsKhong = up.IsKhong;
            user.IsNone = up.IsNone;
            //
            myDbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
            myDbContext.SaveChanges();
            //
            updateReport();
        }
        private void checkBox_isNone_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            UserProgress up = (sender as CheckBox).DataContext as UserProgress;
            User user = myDbContext.Users.First(m => m.Id == up.Id);
            user.IsDa = up.IsDa;
            user.IsKhong = up.IsKhong;
            user.IsNone = up.IsNone;
            //
            myDbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
            myDbContext.SaveChanges();
            //
            updateReport();
        }

        private void MenuItem_menuItemFootballer_Click(object sender, RoutedEventArgs e)
        {
            footballWindow = new FootballWindow();
            footballWindow.Owner = Window.GetWindow(this);
            footballWindow.UsersUpdate += footballWindow_UsersUpdate;
            footballWindow.Closed += footballWindow_Closed;
            footballWindow.Show();
        }        
    }
}
