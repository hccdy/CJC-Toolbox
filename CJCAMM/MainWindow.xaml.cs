using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static CJC_Advanced_Midi_Merger.MainWindow;

namespace CJC_Advanced_Midi_Merger
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public class Groups
    {
        public string file;
        public List<Groups> ms;
        public Sts st;
        public int ppq;
    }
    public partial class MainWindow : Window
    {
        public class Sts
        {
            public bool RemoveBpm;
            public bool ImpBpm;
            public bool ImpMrg;
            public bool RemEpt;
            public bool TrsPpq;
            public bool RemPC, RemPB;
            public int offst;
            public int tmpo;
            public int minvol;
        }
        public static int defppq;
        public void AddMidi(object sender, RoutedEventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "Midi files and groups (*.mid, *.midi, *.cjcamm)|*.mid; *.midi; *.cjcamm";
            open.Multiselect = true;
            if ((bool)open.ShowDialog())
            {
                foreach (string i in open.FileNames)
                {
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = i;
                    itm.DataContext = new Sts();
                    MidisAdded.Items.Add(itm);
                }
            }
            else return;
        }
        public void RemoveMidi(object sender, RoutedEventArgs e)
        {
            MidisAdded.Items.Remove(MidisAdded.SelectedItem);
        }
        public void ClearList(object sender, RoutedEventArgs w)
        {
            MessageBoxResult really = MessageBox.Show((string)Clearbutton.DataContext, (string)Clearbutton.Content, MessageBoxButton.YesNo);
            if (really.ToString() == "No")
            {
                return;
            }
            MidisAdded.Items.Clear();
        }
        [STAThread]
        public void OpenSettings(object sender, RoutedEventArgs w)
        {
            if (MidisAdded.SelectedItem == null)
            {
                return;
            }
            SettingsWindow stss = new SettingsWindow((Sts)((ListBoxItem)MidisAdded.SelectedItem).DataContext);
            stss.ShowInTaskbar = false;
            stss.Owner = this;
            stss.ShowDialog();
            ListBoxItem itm = new ListBoxItem();
            itm.Content = ((ListBoxItem)MidisAdded.SelectedItem).Content;
            itm.DataContext = stss.stt;
            MidisAdded.SelectedItem = itm;
        }
        public void GetInfo(object sender, RoutedEventArgs w)
        {
            if (MidisAdded.SelectedItem == null)
            {
                return;
            }
            InfoWindow inf = new InfoWindow();
            inf.filename.Text = (string)((ListBoxItem)MidisAdded.SelectedItem).Content;
            inf.ShowInTaskbar = false;
            inf.Owner = this;
            inf.ShowDialog();
        }
        [STAThread]
        public void SaveGroup(object sender, RoutedEventArgs w)
        {
            Groups grp = new Groups();
            grp.ms = new List<Groups>();
            grp.file = ".cjcamm";
            grp.st = new Sts();
            try
            {
                for (int i = 0; i < MidisAdded.Items.Count; i++)
                {
                    Groups gr = getgroup((string)((ListBoxItem)MidisAdded.Items[i]).Content, 0);
                    gr.st = (Sts)((ListBoxItem)MidisAdded.Items[i]).DataContext;
                    grp.ms.Add(gr);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("One of the group isn't a valid CJCAMM group or it contains an infinite recursion!", "Invalid group", MessageBoxButton.OK);
                return;
            }
            string output;
            SaveWindow sfg = new SaveWindow();
            if (grp.ms.Count > 0)
            {
                sfg.ppq.Value = grp.ms[0].ppq;
            }
            sfg.ShowInTaskbar = false;
            sfg.Owner = this;
            sfg.ShowDialog();
            if (!sfg.ss)
            {
                return;
            }
            var save = new SaveFileDialog();
            save.Filter = "CJCAMM group files (*.cjcamm)|*.cjcamm";
            if ((bool)save.ShowDialog())
            {
                output = save.FileName;
            }
            else
            {
                return;
            }
            Stream gro = File.Open(output, FileMode.Create, FileAccess.Write, FileShare.Write);
            string head = "Hccdymerge  ";
            byte[] Head = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                Head[i] = (byte)head[i];
            }
            gro.Write(Head, 0, 12);
            List<byte> go = new List<byte>();
            go.Add((byte)(Convert.ToInt32(sfg.ppq.Value) / 256));
            go.Add((byte)(Convert.ToInt32(sfg.ppq.Value) % 256));
            go.Add((byte)0);
            go.Add((byte)1);
            for (int i = 0; i < MidisAdded.Items.Count; i++)
            {
                string file = (string)((ListBoxItem)MidisAdded.Items[i]).Content;
                byte[] arr = Encoding.UTF8.GetBytes(file);
                for (int j = 0; j < arr.Length; j++)
                {
                    go.Add((byte)arr[j]);
                }
                go.Add((byte)0);
                Sts st = (Sts)((ListBoxItem)MidisAdded.Items[i]).DataContext;
                if (st.offst > 0)
                {
                    go.Add((byte)'o');
                    go.Add((byte)(st.offst / 256 / 256 / 256));
                    go.Add((byte)(st.offst / 256 / 256 % 256));
                    go.Add((byte)(st.offst / 256 % 256));
                    go.Add((byte)(st.offst % 256));
                }
                if (st.minvol > 0)
                {
                    go.Add((byte)'v');
                    go.Add((byte)(st.minvol - 1));
                }
                if (st.RemoveBpm)
                {
                    go.Add((byte)'R');
                }
                if (st.ImpBpm)
                {
                    go.Add((byte)'B');
                }
                if (st.ImpMrg)
                {
                    go.Add((byte)'M');
                }
                if (st.RemEpt)
                {
                    go.Add((byte)'E');
                }
                if (st.TrsPpq)
                {
                    go.Add((byte)'P');
                }
                if (st.RemPB)
                {
                    go.Add((byte)'W');
                }
                if (st.RemPC)
                {
                    go.Add((byte)'C');
                }
                go.Add((byte)0);
            }
            gro.Write(go.ToArray(), 0, go.Count);
            gro.Close();
        }

        public void LoadGroup(object sender, RoutedEventArgs e)
        {
            string infile;
            var sfg = new OpenFileDialog();
            sfg.Filter = "CJCAMM group files (*.cjcamm)|*.cjcamm";
            if ((bool)sfg.ShowDialog())
            {
                infile = sfg.FileName;
            }
            else
            {
                return;
            }
            Stream gro = File.Open(infile, FileMode.Open, FileAccess.Read, FileShare.Read);
            string head = "Hccdymerge  ";
            for (int i = 0; i < 12; i++)
            {
                if (head[i] != (char)gro.ReadByte())
                {
                    gro.Close();
                    MessageBox.Show("This is not a valid CJCAMM group file!", "Invalid file", MessageBoxButton.OK);
                    return;
                }
            }
            gro.ReadByte(); gro.ReadByte(); gro.ReadByte(); gro.ReadByte();
            List<ListBoxItem> itms = new List<ListBoxItem>();
            while (true)
            {
                string filename = "";
                List<byte> fn = new List<byte>();
                int ch;
                bool end = false;
                while ((ch = gro.ReadByte()) != 0)
                {
                    if (ch < 0)
                    {
                        if (filename == "" && ch == -1)
                        {
                            end = true;
                            break;
                        }
                        gro.Close();
                        MessageBox.Show("This is not a valid CJCAMM group file!", "Invalid file", MessageBoxButton.OK);
                        return;
                    }
                    fn.Add((byte)ch);
                }
                filename = Encoding.UTF8.GetString(fn.ToArray());
                if (end)
                {
                    break;
                }
                ListBoxItem itm = new ListBoxItem();
                itm.Content = filename;
                Sts st = new Sts();
                while ((ch = gro.ReadByte()) != 0)
                {
                    if (ch < 32)
                    {
                        gro.Close();
                        MessageBox.Show("This is not a valid CJCAMM group file!", "Invalid file", MessageBoxButton.OK);
                        return;
                    }
                    if (ch == 'o')
                    {
                        int offst = 0;
                        for (int j = 0; j < 4; j++)
                        {
                            int sh = gro.ReadByte();
                            if (sh == -1)
                            {
                                gro.Close();
                                MessageBox.Show("This is not a valid CJCAMM group file!", "Invalid file", MessageBoxButton.OK);
                                return;
                            }
                            offst = offst * 256 + sh;
                        }
                        st.offst = offst;
                    }
                    if (ch == 'v')
                    {
                        st.minvol = gro.ReadByte();
                        if (st.minvol == -1)
                        {
                            gro.Close();
                            MessageBox.Show("This is not a valid CJCAMM group file!", "Invalid file", MessageBoxButton.OK);
                            return;
                        }
                        st.minvol++;
                    }
                    if (ch == 'B')
                    {
                        st.ImpBpm = true;
                    }
                    if (ch == 'M')
                    {
                        st.ImpMrg = true;
                    }
                    if (ch == 'R')
                    {
                        st.RemoveBpm = true;
                    }
                    if (ch == 'E')
                    {
                        st.RemEpt = true;
                    }
                    if (ch == 'P')
                    {
                        st.TrsPpq = true;
                    }
                    if (ch == 'W')
                    {
                        st.RemPB = true;
                    }
                    if (ch == 'C')
                    {
                        st.RemPC = true;
                    }
                }
                itm.DataContext = st;
                itms.Add(itm);
            }
            gro.Close();
            for (int i = 0; i < itms.Count; i++)
            {
                MidisAdded.Items.Add(itms[i]);
            }
        }

        public Groups getgroup(string filename, int ceng)
        {
            if (ceng > 256)
            {
                throw new Exception();
            }
            Stream ins = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            Groups grp = new Groups();
            grp.ms = new List<Groups>();
            grp.file = filename;
            if (!filename.EndsWith(".cjcamm"))
            {
                for (int i = 0; i < 12; i++)
                {
                    ins.ReadByte();
                }
                int ppq = ins.ReadByte();
                ppq = ppq * 256 + ins.ReadByte();
                grp.ppq = ppq;
                grp.st = new Sts();
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    ins.ReadByte();
                }
                int ppq = ins.ReadByte();
                ppq = ppq * 256 + ins.ReadByte();
                grp.ppq = ppq;
                ins.ReadByte();
                ins.ReadByte();
                while (true)
                {
                    int CH = ins.ReadByte();
                    if (CH == -1)
                    {
                        break;
                    }
                    char ch = (char)CH;
                    string fn = "";
                    while (ch != 0)
                    {
                        fn += ch;
                        ch = (char)ins.ReadByte();
                        if (ch == -1)
                        {
                            ins.Close();
                            throw new Exception();
                        }
                    }
                    Groups ngr = getgroup(fn, ceng + 1);
                    ch = (char)ins.ReadByte();
                    while (ch != 0)
                    {
                        if (ch == 'o')
                        {
                            int offst = 0;
                            for (int j = 0; j < 4; j++)
                            {
                                int sh = ins.ReadByte();
                                if (sh == -1)
                                {
                                    ins.Close();
                                    throw new Exception();
                                }
                                offst = offst * 256 + sh;
                            }
                            ngr.st.offst = offst;
                        }
                        if (ch == 'v')
                        {
                            ngr.st.minvol = ins.ReadByte();
                            if (ngr.st.minvol == -1)
                            {
                                throw new Exception();
                            }
                            ngr.st.minvol++;
                        }
                        if (ch == 'B')
                        {
                            ngr.st.ImpBpm = true;
                        }
                        if (ch == 'M')
                        {
                            ngr.st.ImpMrg = true;
                        }
                        if (ch == 'R')
                        {
                            ngr.st.RemoveBpm = true;
                        }
                        if (ch == 'E')
                        {
                            ngr.st.RemEpt = true;
                        }
                        if (ch == 'P')
                        {
                            ngr.st.TrsPpq = true;
                        }
                        if (ch == 'W')
                        {
                            ngr.st.RemPB = true;
                        }
                        if (ch == 'C')
                        {
                            ngr.st.RemPC = true;
                        }
                        ch = (char)ins.ReadByte();
                        if (ch == -1)
                        {
                            ins.Close();
                            throw new Exception();
                        }
                    }
                    grp.ms.Add(ngr);
                }
            }
            ins.Close();
            return grp;
        }

        public void StartRender(object sender, RoutedEventArgs e)
        {
            Groups grp = new Groups();
            grp.ms = new List<Groups>();
            grp.file = ".cjcamm";
            grp.st = new Sts();
            try
            {
                for (int i = 0; i < MidisAdded.Items.Count; i++)
                {
                    Groups gr = getgroup((string)((ListBoxItem)MidisAdded.Items[i]).Content, 0);
                    gr.st = (Sts)((ListBoxItem)MidisAdded.Items[i]).DataContext;
                    grp.ms.Add(gr);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("One of the group isn't a valid CJCAMM group or it contains an infinite recursion!", "Invalid group", MessageBoxButton.OK);
                return;
            }
            string output;
            StartWindow sfg = new StartWindow();
            if (grp.ms.Count > 0)
            {
                sfg.ppq.Value = grp.ms[0].ppq;
            }
            sfg.ShowInTaskbar = false;
            sfg.Owner = this;
            sfg.ShowDialog();
            grp.ppq = (int)sfg.ppq.Value;
            if (!sfg.ss)
            {
                return;
            }
            var save = new SaveFileDialog();
            save.Filter = "MIDI files (*.mid)|*.mid";
            if ((bool)save.ShowDialog())
            {
                output = save.FileName;
            }
            else
            {
                return;
            }
            try
            {
                RenderWindow rw = new RenderWindow(grp, output);
                rw.ShowInTaskbar = false;
                rw.Owner = this;
                Thread th = new Thread(rw.StartMerge);
                th.IsBackground = true;
                th.Start();
                rw.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Render failed! Please check if this file is being read by other programs!");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragtoList(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i = 0; i < filenames.Length; i++)
            {
                string s = filenames[i];
                if (!s.EndsWith(".mid") && !s.EndsWith(".midi") && !s.EndsWith(".cjcamm"))
                {
                    return;
                }
            }
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void DroptoList(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i = 0; i < filenames.Length; i++)
            {
                string s = filenames[i];
                if (!s.EndsWith(".mid") && !s.EndsWith(".midi") && !s.EndsWith(".cjcamm"))
                {
                    return;
                }
            }
            for (int i = 0; i < filenames.Length; i++)
            {
                string s = filenames[i];
                if (!s.EndsWith(".mid") && !s.EndsWith(".midi") && !s.EndsWith(".cjcamm"))
                {
                    return;
                }
                ListBoxItem it = new ListBoxItem();
                it.Content = s;
                it.DataContext = new Sts();
                MidisAdded.Items.Add(it);
            }
        }

        private void Dragovertolist(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i = 0; i < filenames.Length; i++)
            {
                string s = filenames[i];
                if (!s.EndsWith(".mid") && !s.EndsWith(".midi") && !s.EndsWith(".cjcamm"))
                {
                    return;
                }
            }
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }
}
