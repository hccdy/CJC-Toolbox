using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        public void CJCAMM_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start(System.Windows.Forms.Application.ExecutablePath, "CJCAMM " + ((ComboBoxItem)Language.SelectedItem).Uid);
        }
        public void CJCOR_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start(System.Windows.Forms.Application.ExecutablePath, "CJCOR " + ((ComboBoxItem)Language.SelectedItem).Uid);
        }
        public void CJCMCG_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start(System.Windows.Forms.Application.ExecutablePath, "CJCMCG " + ((ComboBoxItem)Language.SelectedItem).Uid);
        }
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = @"Resources\" + ((ComboBoxItem)Language.SelectedItem).Uid + ".xaml";
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
