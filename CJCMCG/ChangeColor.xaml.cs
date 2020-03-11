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

namespace CJCMCG
{
    /// <summary>
    /// ChangeColor.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeColor : Window
    {
        public bool okclicked;

        public ChangeColor()
        {
            okclicked = false;
            InitializeComponent();
        }

        private double GetYuv(double R,double G,double B)
        {
            return 0.299 * R + 0.587 * G + 0.114 * B;
        }

        private void Rval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!Rval.Value.HasValue)
            {
                return;
            }
            Rslider.Value = ((double)Rval.Value);
            if (okkey == null)
            {
                return;
            }
            okkey.Background = new SolidColorBrush(Color.FromArgb((byte)Aval.Value, (byte)Rval.Value, (byte)Gval.Value, (byte)Bval.Value));
            if(GetYuv(255 - ((double)Aval.Value) / 255.0 * (255 - (double)Rval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Gval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Bval.Value)) < 100)
            {
                okkey.Foreground= new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void Rslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Rval.Value = ((int)Rslider.Value);
            if (okkey == null)
            {
                return;
            }
            okkey.Background = new SolidColorBrush(Color.FromArgb((byte)Aval.Value, (byte)Rval.Value, (byte)Gval.Value, (byte)Bval.Value));
            if (GetYuv(255 - ((double)Aval.Value) / 255.0 * (255 - (double)Rval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Gval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Bval.Value)) < 100)
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void Gval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!Gval.Value.HasValue)
            {
                return;
            }
            Gslider.Value = ((double)Gval.Value);
            if (okkey == null)
            {
                return;
            }
            okkey.Background = new SolidColorBrush(Color.FromArgb((byte)Aval.Value, (byte)Rval.Value, (byte)Gval.Value, (byte)Bval.Value));
            if (GetYuv(255 - ((double)Aval.Value) / 255.0 * (255 - (double)Rval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Gval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Bval.Value)) < 100)
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void Gslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Gval.Value = ((int)Gslider.Value);
            if (okkey == null)
            {
                return;
            }
            okkey.Background = new SolidColorBrush(Color.FromArgb((byte)Aval.Value, (byte)Rval.Value, (byte)Gval.Value, (byte)Bval.Value));
            if (GetYuv(255 - ((double)Aval.Value) / 255.0 * (255 - (double)Rval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Gval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Bval.Value)) < 100)
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void Bval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!Bval.Value.HasValue)
            {
                return;
            }
            Bslider.Value = ((double)Bval.Value);
            if (okkey == null)
            {
                return;
            }
            okkey.Background = new SolidColorBrush(Color.FromArgb((byte)Aval.Value, (byte)Rval.Value, (byte)Gval.Value, (byte)Bval.Value));
            if (GetYuv(255 - ((double)Aval.Value) / 255.0 * (255 - (double)Rval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Gval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Bval.Value)) < 100)
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void Bslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Bval.Value = ((int)Bslider.Value);
            if (okkey == null)
            {
                return;
            }
            okkey.Background = new SolidColorBrush(Color.FromArgb((byte)Aval.Value, (byte)Rval.Value, (byte)Gval.Value, (byte)Bval.Value));
            if (GetYuv(255 - ((double)Aval.Value) / 255.0 * (255 - (double)Rval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Gval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Bval.Value)) < 100)
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void Aval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!Aval.Value.HasValue)
            {
                return;
            }
            Aslider.Value = ((double)Aval.Value);
            if (okkey == null)
            {
                return;
            }
            okkey.Background = new SolidColorBrush(Color.FromArgb((byte)Aval.Value, (byte)Rval.Value, (byte)Gval.Value, (byte)Bval.Value));
            if (GetYuv(255 - ((double)Aval.Value) / 255.0 * (255 - (double)Rval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Gval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Bval.Value)) < 100)
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void Aslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Aval.Value = ((int)Aslider.Value);
            if (okkey == null)
            {
                return;
            }
            okkey.Background = new SolidColorBrush(Color.FromArgb((byte)Aval.Value, (byte)Rval.Value, (byte)Gval.Value, (byte)Bval.Value));
            if (GetYuv(255 - ((double)Aval.Value) / 255.0 * (255 - (double)Rval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Gval.Value), 255 - ((double)Aval.Value) / 255.0 * (255 - (double)Bval.Value)) < 100)
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                okkey.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void okkey_Click(object sender, RoutedEventArgs e)
        {
            okclicked = true;
            Close();
        }
    }
}
