using System.Windows;

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
