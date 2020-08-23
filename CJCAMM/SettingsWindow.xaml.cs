using System.Windows;
using static CJC_Advanced_Midi_Merger.MainWindow;

namespace CJC_Advanced_Midi_Merger
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public Sts stt;
        public SettingsWindow()
        {
            InitializeComponent();
        }
        public SettingsWindow(Sts st)
        {
            InitializeComponent();
            stt = st;
            if (stt.ImpBpm)
            {
                ImpBpm.IsChecked = true;
            }
            if (stt.ImpMrg)
            {
                ImpMrg.IsChecked = true;
            }
            if (stt.RemoveBpm)
            {
                RemBpm.IsChecked = true;
            }
            if (stt.RemEpt)
            {
                RemEptt.IsChecked = true;
            }
            if (stt.TrsPpq)
            {
                trsppq.IsChecked = true;
            }
            if (stt.RemPB)
            {
                RemPB.IsChecked = true;
            }
            if (stt.RemPC)
            {
                RemPC.IsChecked = true;
            }
            offset.Value = stt.offst;
            minvol.Value = stt.minvol - 1;
        }
        public void okclick(object sender, RoutedEventArgs e)
        {
            stt.ImpBpm = (bool)ImpBpm.IsChecked;
            stt.ImpMrg = (bool)ImpMrg.IsChecked;
            stt.RemoveBpm = (bool)RemBpm.IsChecked;
            stt.RemEpt = (bool)RemEptt.IsChecked;
            stt.TrsPpq = (bool)trsppq.IsChecked;
            stt.RemPB = (bool)RemPB.IsChecked;
            stt.RemPC = (bool)RemPC.IsChecked;
            stt.offst = (int)offset.Value;
            stt.minvol = (int)minvol.Value + 1;
            Close();
        }
    }
}
