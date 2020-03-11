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

namespace CJC_Advanced_Midi_Merger
{
    /// <summary>
    /// SaveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SaveWindow : Window
    {
        public bool ss;
        public SaveWindow()
        {
            InitializeComponent();
            ss = false;
        }
        public void okclick(object sender, RoutedEventArgs e)
        {
            ss = true;
            Close();
        }
    }
}
