using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CJCMCG
{
    /// <summary>
    /// EditReg.xaml 的交互逻辑
    /// </summary>
    public partial class EditReg : Window
    {
        public uint col;
        public string ff, pat;
        public int fs;
        public EditReg()
        {
            InitializeComponent();
        }
        public void RemoveClicked(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\CJCMCG", true);
            key.DeleteSubKeyTree((string)((ListBoxItem)ls.SelectedItem).Content);
            ls.Items.Clear();
            string[] nm = key.GetSubKeyNames();
            foreach (string s in nm)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = s
                };
                ls.Items.Add(item);
            }
        }
        public void SaveClicked(object sender, RoutedEventArgs e)
        {
            StringInputer inputer = new StringInputer();
            if (ls.SelectedItem != null)
            {
                inputer.str.Text = (string)((ListBoxItem)ls.SelectedItem).Content;
            }
            inputer.ShowDialog();
            if (!inputer.tuichu_zhengchangly || inputer.str.Text == "")
            {
                return;
            }
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\CJCMCG", true);
            if (key.GetSubKeyNames().Contains(inputer.str.Text))
            {
                key.DeleteSubKeyTree(inputer.str.Text);
            }
            key.CreateSubKey(inputer.str.Text);
            key = key.OpenSubKey(inputer.str.Text, true);
            key.SetValue("FontFace", ff, RegistryValueKind.String);
            key.SetValue("FontSize", fs, RegistryValueKind.DWord);
            long cc = col;
            key.SetValue("Color", Convert.ToInt32(cc >= (1L << 31) ? cc - (1L << 32) : cc), RegistryValueKind.DWord);
            key.SetValue("Pattern", pat, RegistryValueKind.String);
            key = Registry.CurrentUser.OpenSubKey(@"Software\CJCMCG");
            ls.Items.Clear();
            string[] nm = key.GetSubKeyNames();
            foreach (string s in nm)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = s
                };
                ls.Items.Add(item);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\CJCMCG");
            string[] nm = reg.GetSubKeyNames();
            foreach (string s in nm)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = s
                };
                ls.Items.Add(item);
            }
        }
    }
}
