using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using System.Windows.Shapes;
using WpfApplication1.Database.Context;
using WpfApplication1.Database.Model;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for FootballWindow.xaml
    /// </summary>
    public partial class FootballWindow : Window
    {
        MyDatabaseContext myDbContext = new MyDatabaseContext();

        private ObservableCollection<User> users;

        internal ObservableCollection<User> Users
        {
            get { return users; }
        }

        public FootballWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            loadDatabase();
            //
            listViewMain.ItemsSource = users;
        }

        //
        private void loadDatabase()
        {
            users = new ObservableCollection<User>(myDbContext.Users);
        }

        internal event UsersUpdateHanlder UsersUpdate;

        private void TextBox_FirstName_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            notifyUpdated((User)e.Source, UsersUpdateAction.Change);
        }
        private void TextBox_LastName_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            notifyUpdated((User)e.Source, UsersUpdateAction.Change);
        }

        private void notifyUpdated(User user, UsersUpdateAction action)
        {
            if (UsersUpdate != null)
                UsersUpdate(this, user, action);
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            User user = ((bt.Content as Grid).Children[1] as TextBlock).DataContext as User;
            if (MessageBox.Show("Do you want to delete " + user.FullName + " ?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Exclamation)
                == MessageBoxResult.No)
                return;
            // remove
            myDbContext.Entry(user).State = EntityState.Deleted;
            myDbContext.Users.Remove(user);
            myDbContext.SaveChanges();
            users.Remove(user);
            notifyUpdated(user, UsersUpdateAction.Delete);
        }

        private void LVI_FirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            User user = (sender as TextBox).DataContext as User;
            user.FirstName = (sender as TextBox).Text;
            myDbContext.Entry(user).State = EntityState.Modified;
            myDbContext.SaveChanges();
            notifyUpdated(user, UsersUpdateAction.Change);
        }
        private void LVI_LastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            User user = (sender as TextBox).DataContext as User;
            user.LastName = (sender as TextBox).Text;
            myDbContext.Entry(user).State = EntityState.Modified;
            myDbContext.SaveChanges();
            notifyUpdated(user, UsersUpdateAction.Change);
        }

        

        private void btAddNew_Click(object sender, RoutedEventArgs e)
        {
            UserDialog uDialog = new UserDialog();
            uDialog.ShowDialog();
            if (uDialog.DialogResult.HasValue && uDialog.DialogResult.Value)
            {
                User user = new User()
                {
                    FirstName = uDialog.FirstName,
                    LastName = uDialog.LastName,
                    IsDa = uDialog.IsDa,
                    IsKhong = uDialog.IsKhong,
                    IsNone = uDialog.IsNone
                };
                //
                myDbContext.Users.Add(user);
                myDbContext.SaveChanges();
                //
                loadDatabase();
                listViewMain.ItemsSource = users;
                //
                notifyUpdated(user, UsersUpdateAction.Add);
            }
            else // cancel
            {
                //Do nothing
            }
        }

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    internal delegate void UsersUpdateHanlder(object sender, User user, UsersUpdateAction action);

    public enum UsersUpdateAction
    {
        Delete = 0,
        Add = 1,
        Change = 2,
    }
}
