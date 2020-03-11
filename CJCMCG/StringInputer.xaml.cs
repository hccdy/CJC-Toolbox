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
    /// StringInputer.xaml 的交互逻辑
    /// </summary>
    public partial class StringInputer : Window
    {
        public bool tuichu_zhengchangly;
        public StringInputer()
        {
            tuichu_zhengchangly = false;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tuichu_zhengchangly = true;
            Close();
        }
    }
}
