using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CJC_Toolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void LanguageChange(string s)
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = @"Resources\" + s + ".xaml";
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
        private void StartApplication(object sender,StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                MainWindow window = new MainWindow();
                window.Show();
            }
            else
            {
                string Lang = "en-us";
                if (e.Args.Length >= 2)
                {
                    Lang = e.Args[1];
                }
                if (e.Args[0] == "CJCAMM")
                {
                    CJC_Advanced_Midi_Merger.MainWindow window = new CJC_Advanced_Midi_Merger.MainWindow();
                    LanguageChange(Lang);
                    window.Show();
                }
                else if (e.Args[0] == "CJCOR")
                {
                    CJCOR.MainWindow window = new CJCOR.MainWindow();
                    LanguageChange(Lang);
                    window.Show();
                }
                else if (e.Args[0] == "CJCMCG")
                {
                    CJCMCG.MainWindow window = new CJCMCG.MainWindow();
                    LanguageChange(Lang);
                    window.Show();
                }
            }
        }
    }
}
