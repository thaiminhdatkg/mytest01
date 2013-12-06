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
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for UserDialog.xaml
    /// </summary>
    public partial class UserDialog : Window
    {
        //private bool? _dialogResult;

        public String FirstName
        {
            get { return this.txtBox_firstName.Text; }
            set { this.txtBox_firstName.Text = value; }
        }
        public String LastName
        {
            get { return this.txtBox_lastName.Text; }
            set { this.txtBox_lastName.Text = value; }
        }
        public bool IsDa
        {
            get { return this.rb_isDa.IsChecked.HasValue ? this.rb_isDa.IsChecked.Value : false; }
        }
        public bool IsKhong
        {
            get { return this.rb_isKhong.IsChecked.HasValue ? this.rb_isKhong.IsChecked.Value : false; }
        }
        public bool IsNone
        {
            get { return this.rb_isNone.IsChecked.HasValue ? this.rb_isNone.IsChecked.Value : false; }
        }
        //MainWindow.UserProgress.State state;
        public MainWindow.UserProgress.State State
        {
            get
            {
                if (rb_isDa.IsChecked.HasValue)
                    if (rb_isDa.IsChecked.Value)
                        return MainWindow.UserProgress.State.Da;
                if (rb_isKhong.IsChecked.HasValue)
                    if (rb_isKhong.IsChecked.Value)
                        return MainWindow.UserProgress.State.Khong;
                return MainWindow.UserProgress.State.Chuahoi;
            }
            set
            {
                switch (State)
                {
                    case MainWindow.UserProgress.State.Da:
                        rb_isDa.IsChecked = true;
                        break;
                    case MainWindow.UserProgress.State.Khong:
                        rb_isKhong.IsChecked = true;
                        break;
                    case MainWindow.UserProgress.State.Chuahoi:
                    default:
                        rb_isNone.IsChecked = true;
                        break;
                }
            }
        }

        public UserDialog()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = true;
            //this.Hide();
            base.OnClosing(e);
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
