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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for PopupWaiter.xaml
    /// </summary>
    public partial class PopupWaiter : Window
    {
        public PopupWaiter()
        {
            InitializeComponent();
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 360;
            da.Duration = new Duration(TimeSpan.FromSeconds(3));
            da.RepeatBehavior = RepeatBehavior.Forever;
            RotateTransform rt = new RotateTransform();
            imageWaiting.RenderTransform = rt;
            imageWaiting.RenderTransformOrigin = new Point(0.5, 0.5);
            rt.BeginAnimation(RotateTransform.AngleProperty, da);
        }
    }
}
