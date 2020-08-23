using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CJC_Advanced_Midi_Merger
{
    /// <summary>
    /// InfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InfoWindow : Window
    {
        long nc = 0, tm = 0;
        public InfoWindow()
        {
            InitializeComponent();
        }
        public void GetInfo()
        {
            string filenameget = "";
            Dispatcher.Invoke(new Action(() =>
            {
                filenameget = filename.Text;
            }));
            BufferedStream buff = new BufferedStream(File.Open(filenameget, FileMode.Open, FileAccess.Read, FileShare.Read), 1048576);
            for (int i = 0; i < 10; i++)
            {
                buff.ReadByte();
            }
            int trkcnt = buff.ReadByte();
            trkcnt = trkcnt * 256 + buff.ReadByte();
            buff.ReadByte(); buff.ReadByte();
            for (int trk = 0; trk < trkcnt; trk++)
            {
                for (int i = 0; i < 4; i++)
                {
                    buff.ReadByte();
                }
                int lstcmd = 256, nxtcmd = 256;
                long len = buff.ReadByte();
                for (int i = 1; i < 4; i++)
                {
                    len = len * 256 + buff.ReadByte();
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    progress.SetResourceReference(ContentControl.ContentProperty, "Parsingtrack");
                    string str = (string)progress.Content;
                    str = str.Replace("{trackcount}", (trk + 1).ToString());
                    str = str.Replace("{tracksize}", len.ToString("N0"));
                    progress.Content = str;
                }));
                byte getbyte()
                {
                    if (nxtcmd < 256)
                    {
                        int nxtcmd2 = nxtcmd;
                        nxtcmd = 256;
                        return (byte)nxtcmd2;
                    }
                    else
                    {
                        len--;
                        return (byte)buff.ReadByte();
                    }
                }
                long TM = 0;
                long readtime()
                {
                    long tm = 0;
                    while (true)
                    {
                        int bt = buff.ReadByte();
                        len--;
                        tm = tm * 128 + (bt & 0b01111111);
                        if (bt < 128)
                        {
                            break;
                        }
                    }
                    return tm;
                }
                while (len > 0)
                {
                    TM += readtime();
                    int cmd = buff.ReadByte();
                    len--;
                    if (cmd < 128)
                    {
                        nxtcmd = cmd;
                        cmd = lstcmd;
                    }
                    int cm = cmd & 0b11110000;
                    if (cm == 0b10010000)
                    {
                        nc++;
                        getbyte(); getbyte();
                    }
                    else if (cm == 0b10000000)
                    {
                        getbyte(); getbyte();
                    }
                    else if (cm == 0b11000000 || cm == 0b11010000 || cmd == 0b11110011)
                    {
                        getbyte();
                    }
                    else if (cm == 0b11100000 || cm == 0b10110000 || cmd == 0b11110010 || cm == 0b10100000)
                    {
                        getbyte(); getbyte();
                    }
                    else if (cmd == 0b11110000)
                    {
                        int ffx = getbyte();
                        do
                        {
                            ffx = getbyte();
                        } while (ffx != 0b11110111);
                    }
                    else if (cmd == 0b11110100 || cmd == 0b11110001 || cmd == 0b11110101 || cmd == 0b11111001 || cmd == 0b11111101 || cmd == 0b11110110 || cmd == 0b11110111 || cmd == 0b11111000 || cmd == 0b11111010 || cmd == 0b11111100 || cmd == 0b11111110)
                    {
                    }
                    else if (cmd == 0b11111111)
                    {
                        cmd = getbyte();
                        if (cmd == 0)
                        {
                            getbyte(); getbyte(); getbyte();
                        }
                        else if (cmd >= 1 && cmd <= 10 || cmd == 0x7f)
                        {
                            long ff = readtime();
                            while (ff-- > 0)
                            {
                                getbyte();
                            }
                        }
                        else if (cmd == 0x20 || cmd == 0x21)
                        {
                            getbyte(); getbyte();
                        }
                        else if (cmd == 0x2f)
                        {
                            getbyte();
                            if (TM > tm)
                            {
                                tm = TM;
                            }
                            break;
                        }
                        else if (cmd == 0x51)
                        {
                            getbyte(); getbyte(); getbyte(); getbyte();
                        }
                        else if (cmd == 0x54 || cmd == 0x58)
                        {
                            getbyte(); getbyte(); getbyte(); getbyte(); getbyte();
                        }
                        else if (cmd == 0x59)
                        {
                            getbyte(); getbyte(); getbyte(); getbyte();
                        }
                    }
                }
                while (len > 0)
                {
                    len--;
                    buff.ReadByte();
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    notecnt.Text = nc.ToString("N0");
                    midilen.Text = tm.ToString("N0");
                }));
            }
            Dispatcher.Invoke(new Action(() =>
            {
                notecnt.Text = nc.ToString("N0");
                midilen.Text = tm.ToString("N0");
                progress.SetResourceReference(ContentControl.ContentProperty, "Finished");
            }));
        }
        public void GetInfoClicked(object sender, RoutedEventArgs e)
        {
            progress.IsEnabled = false;
            Thread th = new Thread(GetInfo);
            th.IsBackground = true;
            th.Start();
        }
        public void GetLessInfo(object sender, RoutedEventArgs e)
        {
            Stream st = File.Open(filename.Text, FileMode.Open, FileAccess.Read, FileShare.Read);
            long bg = st.Position;
            if (!filename.Text.EndsWith(".cjcamm"))
            {
                for (int i = 0; i < 10; i++)
                {
                    st.ReadByte();
                }
                int trk = st.ReadByte();
                trk = trk * 256 + st.ReadByte();
                trkcnt.Text = Convert.ToString(trk);
                int ppq = st.ReadByte();
                ppq = ppq * 256 + st.ReadByte();
                originalppq.Text = Convert.ToString(ppq);
            }
            else
            {
                TCSTR.Text = OPPQSTR.Text;
                OPPQSTR.Text = NCSTR.Text = MLSTR.Text = "";
                for (int i = 0; i < 12; i++)
                {
                    st.ReadByte();
                }
                int ppq = st.ReadByte();
                ppq = ppq * 256 + st.ReadByte();
                trkcnt.Text = Convert.ToString(ppq);
                progress.IsEnabled = false;
                progress.SetResourceReference(ContentControl.ContentProperty, "UnableGroup");
                notecnt.Text = "";
                midilen.Text = "";
            }
            st.Seek(0, SeekOrigin.End);
            long en = st.Position;
            filesize.Text = (en - bg).ToString("N0");
            st.Close();
        }
        public void OKclick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
