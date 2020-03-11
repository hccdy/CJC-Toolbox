using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CJC_Toolbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void CJCAMM_Clicked(object sender,RoutedEventArgs e)
        {
            Process.Start(System.Windows.Forms.Application.ExecutablePath, "CJCAMM " + ((string)((ComboBoxItem)Language.SelectedItem).Uid));
        }
        public void CJCOR_Clicked(object sender,RoutedEventArgs e)
        {
            Process.Start(System.Windows.Forms.Application.ExecutablePath, "CJCOR " + ((string)((ComboBoxItem)Language.SelectedItem).Uid));
        }
        public void CJCMCG_Clicked(object sender,RoutedEventArgs e)
        {
            Process.Start(System.Windows.Forms.Application.ExecutablePath, "CJCMCG " + ((string)((ComboBoxItem)Language.SelectedItem).Uid));
        }
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = @"Resources\" + ((string)((ComboBoxItem)Language.SelectedItem).Uid) + ".xaml";
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
