﻿using Microsoft.Win32;
using System;
using System.Drawing;
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
using static CJCMCG.MainWindow;
using System.IO;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using System.Diagnostics;
using SharpCompress.Archives.GZip;
using System.IO.Compression;
using SharpCompress.Compressors.Xz;
using System.Collections;
using System.Threading;

namespace CJCMCG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    struct pairli
    {
        public long x;
        public double y;
        public int trk, cnt;
        public pairli(long a, double b, int c, int d)
        {
            x = a;
            y = b;
            trk = c;
            cnt = d;
        }
        public static bool operator <(pairli fx, pairli fy)
        {
            if (fx.x != fy.x)
            {
                return fx.x < fy.x;
            }
            else if (fx.trk != fy.trk)
            {
                return fx.trk < fy.trk;
            }
            else if (fx.cnt != fy.cnt)
            {
                return fx.cnt < fy.cnt;
            }
            else
            {
                return false;
            }
        }
        public static bool operator >(pairli fx, pairli fy)
        {
            if (fx.x != fy.x)
            {
                return fx.x > fy.x;
            }
            else if (fx.trk != fy.trk)
            {
                return fx.trk > fy.trk;
            }
            else if (fx.cnt != fy.cnt)
            {
                return fx.cnt > fy.cnt;
            }
            else
            {
                return false;
            }
        }
    }
    struct pairls
    {
        public long x;
        public String y;
        public int trk, cnt;
        public pairls(long a, String b, int c, int d)
        {
            x = a;
            y = b;
            trk = c;
            cnt = d;
        }
        public static bool operator <(pairls fx, pairls fy)
        {
            if (fx.x != fy.x)
            {
                return fx.x < fy.x;
            }
            else if (fx.trk != fy.trk)
            {
                return fx.trk < fy.trk;
            }
            else if (fx.cnt != fy.cnt)
            {
                return fx.cnt < fy.cnt;
            }
            else
            {
                return false;
            }
        }
        public static bool operator >(pairls fx, pairls fy)
        {
            if (fx.x != fy.x)
            {
                return fx.x > fy.x;
            }
            else if (fx.trk != fy.trk)
            {
                return fx.trk > fy.trk;
            }
            else if (fx.cnt != fy.cnt)
            {
                return fx.cnt > fy.cnt;
            }
            else
            {
                return false;
            }
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static string file;

        private void pres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)pres.SelectedItem).Uid == "1080x320")
            {
                width.Value = 1080;
                height.Value = 320;
            }
            else if (((ComboBoxItem)pres.SelectedItem).Uid == "720P")
            {
                width.Value = 1280;
                height.Value = 720;
            }
            else if (((ComboBoxItem)pres.SelectedItem).Uid == "1080P")
            {
                width.Value = 1920;
                height.Value = 1080;
            }
            else if (((ComboBoxItem)pres.SelectedItem).Uid == "1440P")
            {
                width.Value = 2560;
                height.Value = 1440;
            }
            else if (((ComboBoxItem)pres.SelectedItem).Uid == "2160P")
            {
                width.Value = 3840;
                height.Value = 2160;
            }
            else if (((ComboBoxItem)pres.SelectedItem).Uid == "2880P")
            {
                width.Value = 5120;
                height.Value = 2880;
            }
            else if (((ComboBoxItem)pres.SelectedItem).Uid == "4320P")
            {
                width.Value = 7680;
                height.Value = 4320;
            }
            else if (((ComboBoxItem)pres.SelectedItem).Uid == "8640P")
            {
                width.Value = 15360;
                height.Value = 8640;
            }
            else if (((ComboBoxItem)pres.SelectedItem).Uid == "17280P")
            {
                width.Value = 30720;
                height.Value = 17280;
            }
        }
        bool fileselected = false;
        [STAThread]
        public void filename_Click(object sender, RoutedEventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "Midi files or zipped files (*.mid, *.midi, *.xz, *.7z, *.zip, *.gz, *.tar, *.rar)|*.mid; *.midi; *.xz; *.7z; *.zip; *.gz; *.tar; *.rar";
            if ((bool)open.ShowDialog())
            {
                filename.Content = open.FileName;
                fileselected = true;
            }
        }

        private void pats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pats.SelectedItem == null)
            {
                return;
            }
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\CJCMCG\" + ((ComboBoxItem)pats.SelectedItem).Uid, true);
            fsize.Value = Convert.ToInt32(reg.GetValue("FontSize"));
            string fface = (string)reg.GetValue("FontFace");
            foreach(ComboBoxItem item in font.Items)
            {
                if (item.Uid == fface)
                {
                    font.SelectedItem = item;
                    break;
                }
            }
            string pattt = (string)reg.GetValue("Pattern");
            pattt = pattt.Replace("\\10", "\n").Replace("\\23", "\\");
            pat.Text = pattt;
            long coll = (int)(reg.GetValue("Color"));
            UInt32 col = Convert.ToUInt32(coll < 0 ? coll + (1L << 32) : coll);
            color.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(col / 256 / 256 / 256), (byte)(col / 256 / 256 % 256), (byte)(col / 256 % 256), (byte)(col % 256)));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                Process ffmpeg = new Process();
                ffmpeg.StartInfo = new ProcessStartInfo("ffmpeg", "-help");
                ffmpeg.StartInfo.RedirectStandardInput = false;
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.RedirectStandardError = false;
                ffmpeg.Start();
            } catch (Exception)
            {
                MessageBox.Show("There was an error finding ffmpeg.\nIs ffmpeg.exe in the same folder as this program?");
                Close();
            }
            System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection();
            foreach (System.Drawing.FontFamily family in fonts.Families)
            {
                ComboBoxItem cbb = new ComboBoxItem();
                cbb.Content = family.Name;
                cbb.Uid = family.Name;
                font.Items.Add(cbb);
                if (family.Name == "Arial")
                {
                    font.SelectedItem = cbb;
                }
            }
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("Software", true);
            if (!reg.GetSubKeyNames().Contains("CJCMCG"))
            {
                reg.CreateSubKey("CJCMCG");
            }
            reg = reg.OpenSubKey("CJCMCG");
            string[] nm = reg.GetSubKeyNames();
            foreach (string s in nm)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Uid = s;
                item.Content = s;
                pats.Items.Add(item);
            }
        }

        public void color_Click(object sender, RoutedEventArgs e)
        {
            ChangeColor change = new ChangeColor();
            System.Windows.Media.Color col = ((SolidColorBrush)color.Background).Color;
            change.Aval.Value = col.A;
            change.Aslider.Value = col.A;
            change.Rval.Value = col.R;
            change.Rslider.Value = col.R;
            change.Gval.Value = col.G;
            change.Gslider.Value = col.G;
            change.Bval.Value = col.B;
            change.Bslider.Value = col.B;
            change.ShowDialog();
            if (change.okclicked)
            {
                color.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)change.Aval.Value, (byte)change.Rval.Value, (byte)change.Gval.Value, (byte)change.Bval.Value));
            }
        }

        public void help_Click(object sender, RoutedEventArgs e)
        {
            Helpfile help = new Helpfile();
            help.Show();
        }

        private void Ere_Click(object sender, RoutedEventArgs e)
        {
            EditReg edit = new EditReg();
            System.Windows.Media.Color col = ((SolidColorBrush)color.Background).Color;
            edit.col = Convert.ToUInt32(col.A * 256L * 256 * 256 + col.R * 256 * 256 + col.G * 256 + col.B);
            edit.ff = ((ComboBoxItem)font.SelectedItem).Uid;
            edit.fs = (int)fsize.Value;
            edit.pat = pat.Text.Replace("\\", "\\23").Replace("\n", "\\10");
            edit.ShowDialog();
            pats.Items.Clear();
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\CJCMCG");
            string[] nm = reg.GetSubKeyNames();
            foreach (string s in nm)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Uid = s;
                item.Content = s;
                pats.Items.Add(item);
            }
        }

        public static string filein;
        Process startNewSSFF(int W, int H, int F, string path)
        {
            Process ffmpeg = new Process();
            string args = "" +
                    " -s " + W.ToString() + "x" + H.ToString() + " -y -vsync 2 -threads " + Environment.ProcessorCount.ToString() + " -r " + Convert.ToString(F)
                    + " -i - -vcodec png";
            args += " \"" + path + "\"";
            ffmpeg.StartInfo = new ProcessStartInfo("ffmpeg", args);
            ffmpeg.StartInfo.RedirectStandardInput = true;
            ffmpeg.StartInfo.CreateNoWindow = true;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.RedirectStandardError = false;
            ffmpeg.Start();
            return ffmpeg;
        }
        Process ff;
        static bool CanDec(string s)
        {
            return s.EndsWith(".mid") || s.EndsWith(".xz") || s.EndsWith(".zip") || s.EndsWith(".7z") || s.EndsWith(".rar") || s.EndsWith(".tar") || s.EndsWith(".gz");
        }
        static Stream AddXZLayer(Stream input)
        {
            try
            {
                Process xz = new Process();
                xz.StartInfo = new ProcessStartInfo("xz", "-dc --threads=0")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                xz.Start();
                Task.Run(() =>
                {
                    input.CopyTo(xz.StandardInput.BaseStream);
                    xz.StandardInput.Close();
                });
                return xz.StandardOutput.BaseStream;
            }
            catch (Exception)
            {
                Console.WriteLine("xz.exe not found, trying internal decompress with lower speed and lower compatibility...");
                return new XZStream(input);
            }
        }
        static Stream AddZipLayer(Stream input)
        {
            var zip = new System.IO.Compression.ZipArchive(input, ZipArchiveMode.Read);
            foreach (var entry in zip.Entries)
            {
                if (CanDec(entry.Name))
                {
                    filein = entry.Name;
                    return entry.Open();
                }
            }
            throw new Exception("No compatible file found in the .zip");
        }
        static Stream AddRarLayer(Stream input)
        {
            var zip = RarArchive.Open(input);
            foreach (var entry in zip.Entries)
            {
                if (CanDec(entry.Key))
                {
                    filein = entry.Key;
                    return entry.OpenEntryStream();
                }
            }
            throw new Exception("No compatible file found in the .rar");
        }
        static Stream Add7zLayer(Stream input)
        {
            var zip = SevenZipArchive.Open(input);
            foreach (var entry in zip.Entries)
            {
                if (CanDec(entry.Key))
                {
                    filein = entry.Key;
                    return entry.OpenEntryStream();
                }
            }
            throw new Exception("No compatible file found in the .7z");
        }
        static Stream AddTarLayer(Stream input)
        {
            var zip = TarArchive.Open(input);
            foreach (var entry in zip.Entries)
            {
                if (CanDec(entry.Key))
                {
                    filein = entry.Key;
                    return entry.OpenEntryStream();
                }
            }
            throw new Exception("No compatible file found in the .tar");
        }
        static Stream AddGZLayer(Stream input)
        {
            var zip = GZipArchive.Open(input);
            foreach (var entry in zip.Entries)
            {
                if (CanDec(entry.Key))
                {
                    filein = entry.Key;
                    return entry.OpenEntryStream();
                }
            }
            throw new Exception("No compatible file found in the .gz");
        }
        int toint(int x)
        {
            return x < 0 ? x + 256 : x;
        }
        string fonsel;
        int fonsz;
        SolidColorBrush colsel;
        public void newframe(int W, int H, string pat)
        {
            using (var fon = new Font(fonsel, fonsz))
            {
                var img = new Bitmap(W, H, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (var gfx = Graphics.FromImage(img))
                {
                    gfx.FillRectangle(System.Drawing.Brushes.Transparent, 0, 0, W, H);
                    Dispatcher.Invoke(new Action(() => {
                        using (var textBrush = new SolidBrush(System.Drawing.Color.FromArgb(colsel.Color.A, colsel.Color.R, colsel.Color.G, colsel.Color.B)))
                        {
                            gfx.DrawString(pat, fon, textBrush, new PointF(0, 0));
                        }
                    }));
                    gfx.Flush();
                }
                byte[] shit = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));
                ff.StandardInput.BaseStream.Write(shit, 0, shit.Length);
            }
        }
        int resol;
        string patt;
        public string getstring(long nc, long an, double bp, double tm, long np, long po, long ti, long at, String lrcs = "")
        {
            string ss = patt;
            ss = ss.Replace("{0}", Convert.ToString(nc));
            ss = ss.Replace("{0,}", (nc).ToString("N0"));
            ss = ss.Replace("{1}", Convert.ToString(an));
            ss = ss.Replace("{1,}", (an).ToString("N0"));
            ss = ss.Replace("{1-0}", Convert.ToString(an - nc));
            ss = ss.Replace("{1-0,}", (an - nc).ToString("N0"));
            ss = ss.Replace("{2}", Convert.ToString(Math.Round(bp * 10) / 10));
            ss = ss.Replace("{3}", Convert.ToInt32(tm * 100) / 100 + "." + (Convert.ToInt32(tm * 100) % 100 / 10) + (Convert.ToInt32(tm * 100) % 10));
            ss = ss.Replace("{4}", Convert.ToString(np));
            ss = ss.Replace("{4,}", (np).ToString("N0"));
            ss = ss.Replace("{5}", Convert.ToString(po));
            ss = ss.Replace("{5,}", (po).ToString("N0"));
            ss = ss.Replace("{6}", Convert.ToString(ti));
            ss = ss.Replace("{6,}", (ti).ToString("N0"));
            ss = ss.Replace("{7}", Convert.ToString(at));
            ss = ss.Replace("{7,}", (at).ToString("N0"));
            ss = ss.Replace("{7-6}", Convert.ToString(at - ti));
            ss = ss.Replace("{7-6,}", (at - ti).ToString("N0"));
            ss = ss.Replace("{8}", Convert.ToString(ti / resol + 1));
            ss = ss.Replace("{9}", Convert.ToString(at / resol + 1));
            ss = ss.Replace("{9-8}", Convert.ToString(at / resol - ti / resol));
            ss = ss.Replace("{A}", Convert.ToString(resol));
            ss = ss.Replace("{A,}", resol.ToString("N0"));
            ss = ss.Replace("{B}", lrcs);
            return ss;
        }
        private class AhhShitPairliCompare : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                pairli X = (pairli)x, Y = (pairli)y;
                if (X < Y)
                {
                    return -1;
                }
                else if (X > Y)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        private class AhhShitPairlsCompare : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                pairls X = (pairls)x, Y = (pairls)y;
                if (X < Y)
                {
                    return -1;
                }
                else if (X > Y)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public void progress()
        {
            if (!fileselected)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    filename.IsEnabled = true;
                    prog.IsEnabled = true;
                }));
                return;
            }
            string fileout = "";
            SaveFileDialog getFileout = new SaveFileDialog();
            getFileout.Filter = "MOV files (*.mov)|*.mov";
            if ((bool)getFileout.ShowDialog())
            {
                fileout = getFileout.FileName;
            }
            else
            {
                return;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                patt = pat.Text;
                fonsel = (string)((ComboBoxItem)font.SelectedItem).Content;
                fonsz = (int)fsize.Value;
                colsel = ((SolidColorBrush)color.Background);
            }));
            int W = 0, H = 0, F = 0, desv = 0;
            Dispatcher.Invoke(new Action(() =>
            {
                W = (int)width.Value; H = (int)height.Value; F = (int)fps.Value;
                desv = (int)des.Value;
            }));
            Stream inppp = File.Open(filein, FileMode.Open, FileAccess.Read, FileShare.Read);
            while (!filein.EndsWith(".mid"))
            {
                if (filein.EndsWith(".xz"))
                {
                    inppp = AddXZLayer(inppp);
                    filein = filein.Substring(0, filein.Length - 3);
                }
                else if (filein.EndsWith(".zip"))
                {
                    inppp = AddZipLayer(inppp);
                }
                else if (filein.EndsWith(".rar"))
                {
                    inppp = AddRarLayer(inppp);
                }
                else if (filein.EndsWith(".7z"))
                {
                    inppp = Add7zLayer(inppp);
                }
                else if (filein.EndsWith(".tar"))
                {
                    inppp = AddTarLayer(inppp);
                }
                else if (filein.EndsWith(".gz"))
                {
                    inppp = AddGZLayer(inppp);
                }
            }
            BufferedStream inpp = new BufferedStream(inppp, 16777216);
            int ReadByte()
            {
                int b = inpp.ReadByte();
                if (b == -1) throw new Exception("Unexpected file end");
                return b;
            }
            for (int i = 0; i < 4; ++i)
            {
                ReadByte();
            }
            for (int i = 0; i < 4; ++i)
            {
                ReadByte();
            }
            ReadByte();
            ReadByte();
            int trkcnt;
            trkcnt = (toint(ReadByte()) * 256) + toint(ReadByte());
            resol = (toint(ReadByte()) * 256) + toint(ReadByte());
            ArrayList bpm = new ArrayList();
            bpm.Add(new pairli(0, 5000000000.0 / resol, -1, 0));
            long noteall = 0;
            int nowtrk = 1;
            int alltic = 0;
            int allticreal = 0;
            ArrayList nts = new ArrayList(), nto = new ArrayList();
            ArrayList lrcs = new ArrayList();
            lrcs.Add(new pairls(0, "", -1, -1));
            for (int trk = 0; trk < trkcnt; trk++)
            {
                int bpmcnt = 0;
                int lrccnt = 0;
                long notes = 0;
                long leng = 0;
                ReadByte();
                ReadByte();
                ReadByte();
                ReadByte();
                for (int i = 0; i < 4; i++)
                {
                    leng = leng * 256 + toint(ReadByte());
                }
                int lstcmd = 256;
                Dispatcher.Invoke(new Action(() =>
                {
                    string str = prog.Uid;
                    str = str.Replace("{trackcount}", (trk + 1).ToString() + "/" + trkcnt.ToString()).Replace("{tracksize}", leng.ToString("N0"));
                    prog.Content = str;
                }));
                int getnum()
                {
                    int ans = 0;
                    int ch = 256;
                    while (ch >= 128)
                    {
                        ch = toint(ReadByte());
                        leng--;
                        ans = ans * 128 + (ch & 0b01111111);
                    }
                    return ans;
                }
                int get()
                {
                    if (lstcmd != 256)
                    {
                        int lstcmd2 = lstcmd;
                        lstcmd = 256;
                        return lstcmd2;
                    }
                    leng--;
                    return toint(ReadByte());
                }
                int TM = 0;
                int prvcmd = 256;
                while (true)
                {
                    int added = getnum();
                    TM += added;
                    int cmd = ReadByte();
                    leng--;
                    if (cmd < 128)
                    {
                        lstcmd = cmd;
                        cmd = prvcmd;
                    }
                    prvcmd = cmd;
                    int cm = cmd & 0b11110000;
                    if (cm == 0b10010000)
                    {
                        get();
                        ReadByte();
                        leng--;
                        while (nts.Count <= TM)
                        {
                            nts.Add(0L);
                        }
                        nts[TM] = (Convert.ToInt64(nts[TM]) + 1L);
                        notes++;
                    }
                    else if (cm == 0b10000000)
                    {
                        get();
                        ReadByte();
                        leng--;
                        while (nto.Count <= TM)
                        {
                            nto.Add(0L);
                        }
                        nto[TM] = (Convert.ToInt64(nto[TM]) + 1L);
                    }
                    else if (cm == 0b11000000 || cm == 0b11010000 || cmd == 0b11110011)
                    {
                        get();
                    }
                    else if (cm == 0b11100000 || cm == 0b10110000 || cmd == 0b11110010 || cm == 0b10100000)
                    {
                        get();
                        ReadByte();
                        leng--;
                    }
                    else if (cmd == 0b11110000)
                    {
                        if (get() == 0b11110111)
                        {
                            continue;
                        }
                        do
                        {
                            leng--;
                        } while (ReadByte() != 0b11110111);
                    }
                    else if (cmd == 0b11110100 || cmd == 0b11110001 || cmd == 0b11110101 || cmd == 0b11111001 || cmd == 0b11111101 || cmd == 0b11110110 || cmd == 0b11110111 || cmd == 0b11111000 || cmd == 0b11111010 || cmd == 0b11111100 || cmd == 0b11111110)
                    {
                    }
                    else if (cmd == 0b11111111)
                    {
                        cmd = get();
                        if (cmd == 0)
                        {
                            ReadByte(); ReadByte(); ReadByte();
                            leng -= 3;
                        }
                        else if (cmd >= 1 && cmd <= 10 && cmd != 5 || cmd == 0x7f)
                        {
                            long ff = getnum();
                            while (ff-- > 0)
                            {
                                ReadByte();
                                leng--;
                            }
                        }
                        else if (cmd == 0x20 || cmd == 0x21)
                        {
                            ReadByte(); ReadByte(); leng -= 2;
                        }
                        else if (cmd == 0x2f)
                        {
                            ReadByte();
                            leng--;
                            if (TM > allticreal)
                            {
                                allticreal = TM;
                            }
                            TM -= added;
                            break;
                        }
                        else if (cmd == 0x51)
                        {
                            bpmcnt++;
                            ReadByte();
                            leng--;
                            int BPM = get();
                            BPM = BPM * 256 + get();
                            BPM = BPM * 256 + get();
                            bpm.Add(new pairli(TM, 10000.0 * BPM / resol, trk, bpmcnt));
                        }
                        else if (cmd == 5)
                        {
                            Encoding gb2312 = Encoding.GetEncoding("GB2312");
                            Encoding def = Encoding.GetEncoding("UTF-8");
                            lrccnt++;
                            int ff = (int)getnum();
                            byte[] S = new byte[ff];
                            int cnt = 0;
                            while (ff-- > 0)
                            {
                                S[cnt++] = Convert.ToByte(ReadByte());
                                leng--;
                            }
                            S = Encoding.Convert(gb2312, def, S);
                            lrcs.Add(new pairls(TM, def.GetString(S), trk, lrccnt));
                        }
                        else if (cmd == 0x54 || cmd == 0x58)
                        {
                            ReadByte(); ReadByte(); ReadByte(); ReadByte(); ReadByte();
                            leng -= 5;
                        }
                        else if (cmd == 0x59)
                        {
                            ReadByte(); ReadByte(); ReadByte();
                            leng -= 3;
                        }
                        else if (cmd == 0x0a)
                        {
                            int ss = get();
                            while (ss-- > 0)
                            {
                                ReadByte();
                                leng--;
                            }
                        }
                    }
                }
                while (leng > 0)
                {
                    ReadByte();
                    leng--;
                }
                if (TM > alltic)
                {
                    alltic = TM;
                }
                while (nts.Count <= TM)
                {
                    nts.Add(0);
                }
                while (nto.Count <= TM)
                {
                    nto.Add(0);
                }
                noteall += notes;
                nowtrk++;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                prog.SetResourceReference(ContentControl.ContentProperty, "ReadFinished");
                string str = (string)prog.Content;
                str = str.Replace("{notecnt}", noteall.ToString("N0"));
                prog.Content = str;
            }));
            bpm.Sort(new AhhShitPairliCompare());
            lrcs.Sort(new AhhShitPairlsCompare());
            double[] tmc = new double[bpm.Count];
            tmc[0] = 0;
            for (int i = 1; i < bpm.Count; i++)
            {
                tmc[i] = tmc[i - 1] + (((pairli)bpm[i]).x - ((pairli)bpm[i - 1]).x) * ((pairli)bpm[i - 1]).y / 10000.0;
            }
            ff = startNewSSFF(W, H, F, fileout);
            int tmdf = 0;
            for (int i = 0; i < F * desv; i++)
            {
                string s = getstring(0, noteall, 120, 0, 0, 0, 0, alltic);
                newframe(W, H, s);
                Dispatcher.Invoke(new Action(() =>
                {
                    prog.SetResourceReference(ContentControl.ContentProperty, "FrameRender");
                    string str = (string)prog.Content;
                    str = str.Replace("{frames}", tmdf.ToString());
                    prog.Content = str;
                    preview.Text = s;
                }));
                tmdf++;
            }
            long tmnow = 0;
            long[] history = new long[F];
            long poly = 0;
            int now = 0;
            long notecnt = 0;
            int tmm = 0, nowlrc = 0, bpmptr = 0;
            while (now < nts.Count)
            {
                for (; now <= tmm; now++)
                {
                    if (now >= nts.Count)
                    {
                        break;
                    }
                    notecnt += Convert.ToInt64(nts[now]);
                    poly += Convert.ToInt64(nts[now]);
                    poly -= Convert.ToInt64(nto[now]);
                }
                while (nowlrc < lrcs.Count - 1 && ((pairls)lrcs[nowlrc + 1]).x <= tmm)
                {
                    nowlrc++;
                }
                string s = getstring(notecnt, noteall, 600000000000.0 / resol / ((pairli)bpm[bpmptr]).y, 1.0 * (tmdf - F * desv) / F, notecnt - history[tmdf % F], poly, tmm > alltic ? alltic : Convert.ToInt64(tmm), alltic, ((pairls)lrcs[nowlrc]).y);
                newframe(W, H, s);
                history[tmdf % F] = notecnt;
                Dispatcher.Invoke(new Action(() =>
                {
                    prog.SetResourceReference(ContentControl.ContentProperty, "FrameRender");
                    string str = (string)prog.Content;
                    str = str.Replace("{frames}", tmdf.ToString());
                    prog.Content = str;
                    preview.Text = s;
                }));
                tmdf++;
                tmnow = Convert.ToInt64((tmdf - F * desv) * 1000000.0 / F);
                while (bpmptr < bpm.Count - 1 && tmc[bpmptr + 1] < Convert.ToDouble(tmnow))
                {
                    bpmptr++;
                }
                tmm = Convert.ToInt32(((pairli)bpm[bpmptr]).x + (tmnow - tmc[bpmptr]) * 10000.0 / ((pairli)bpm[bpmptr]).y);
            }
            for (int i = 0; i < 5 * F; i++)
            {
                string s = getstring(noteall, noteall, 600000000000.0 / resol / ((pairli)bpm[bpmptr]).y, 1.0 * (tmdf - F * desv) / F, notecnt - history[tmdf % F], 0, alltic, alltic);
                newframe(W, H, s);
                Dispatcher.Invoke(new Action(() =>
                {
                    prog.SetResourceReference(ContentControl.ContentProperty, "FrameRender");
                    string str = (string)prog.Content;
                    str = str.Replace("{frames}", tmdf.ToString());
                    prog.Content = str;
                    preview.Text = s;
                }));
                history[tmdf % F] = noteall;
                tmdf++;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                prog.SetResourceReference(ContentControl.ContentProperty, "Finished");
                preview.Text = "";
            }));
            ff.StandardInput.Close();
            Dispatcher.Invoke(new Action(() =>
            {
                filename.IsEnabled = true;
                prog.IsEnabled = true;
            }));
        }
        public void progress_Click(object sender,RoutedEventArgs e)
        {
            filein = (string)filename.Content;
            filename.IsEnabled = false;
            prog.IsEnabled = false;
            Thread thread = new Thread(progress);
            thread.Start();
        }
    }
}
