using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace CJC_Advanced_Midi_Merger
{
    /// <summary>
    /// RenderWindow.xaml 的交互逻辑
    /// </summary>
    public class Node
    {
        public long TM;
        public List<byte> dt;
        public Node R;
        public Node()
        {
            TM = 0;
            dt = new List<byte>();
            R = null;
        }
    }
    public partial class RenderWindow : Window
    {
        public static Groups gr;
        public static string outfile;
        static BufferedStream ous;
        static int trks = 0, trkp = 0;
        static List<byte> tmp, tmp2, tmp3;
        static int alltrk = 0;
        public void trkcount(Groups grp, bool impbpm)
        {
            if (grp.file.EndsWith(".cjcamm"))
            {
                for (int i = 0; i < grp.ms.Count; i++)
                {
                    trkcount(grp.ms[i], impbpm || grp.ms[i].st.ImpBpm);
                }
            }
            else
            {
                Stream ins = File.Open(grp.file, FileMode.Open, FileAccess.Read, FileShare.Read);
                for (int i = 0; i < 10; i++)
                {
                    ins.ReadByte();
                }
                int trkcnt = ins.ReadByte();
                trkcnt = trkcnt * 256 + ins.ReadByte();
                if (!impbpm)
                {
                    alltrk += trkcnt;
                }
                else
                {
                    alltrk += trkcnt * 2;
                }
            }
        }
        public RenderWindow()
        {
            InitializeComponent();
        }
        public RenderWindow(Groups grp, string outfil)
        {
            trks = 0;
            trkp = 0;
            alltrk = 0;
            tmp = new List<byte>();
            tmp2 = new List<byte>();
            tmp3 = new List<byte>();
            gr = grp;
            outfile = outfil;
            ous = new BufferedStream(File.Open(outfile, FileMode.Create, FileAccess.Write, FileShare.Write), 67108864);
            InitializeComponent();
        }
        public List<byte> ImplaceTrks(List<byte> original, List<byte> newls)
        {
            if (newls.Count == 0)
            {
                return original;
            }
            if (original.Count > 2000000000)
            {
                original.Add(0);
                original.Add(0xff);
                original.Add(0x2f);
                original.Add(0);
                ous.WriteByte((byte)'M'); ous.WriteByte((byte)'T'); ous.WriteByte((byte)'r'); ous.WriteByte((byte)'k');
                long lens = original.Count();
                ous.WriteByte((byte)(lens / 256 / 256 / 256));
                ous.WriteByte((byte)(lens / 256 / 256 % 256));
                ous.WriteByte((byte)(lens / 256 % 256));
                ous.WriteByte((byte)(lens % 256));
                trks++;
                ous.Write(original.ToArray(), 0, original.Count);
                Dispatcher.Invoke(new Action(() =>
                {
                    Progress.Text = (string)Progress.DataContext;
                    Progress.Text = Progress.Text.Replace("{trks}", trkp.ToString() + "/" + alltrk.ToString());
                    Progress.Text = Progress.Text.Replace("{wrtt}", trks.ToString());
                }));
                return newls;
            }
            if (newls.Count > 2000000000)
            {
                newls.Add(0);
                newls.Add(0xff);
                newls.Add(0x2f);
                newls.Add(0);
                ous.WriteByte((byte)'M'); ous.WriteByte((byte)'T'); ous.WriteByte((byte)'r'); ous.WriteByte((byte)'k');
                long lens = newls.Count();
                ous.WriteByte((byte)(lens / 256 / 256 / 256));
                ous.WriteByte((byte)(lens / 256 / 256 % 256));
                ous.WriteByte((byte)(lens / 256 % 256));
                ous.WriteByte((byte)(lens % 256));
                trks++;
                ous.Write(newls.ToArray(), 0, newls.Count);
                Dispatcher.Invoke(new Action(() =>
                {
                    Progress.Text = (string)Progress.DataContext;
                    Progress.Text = Progress.Text.Replace("{trks}", trkp.ToString() + "/" + alltrk.ToString());
                    Progress.Text = Progress.Text.Replace("{wrtt}", trks.ToString());
                }));
                return original;
            }
            List<byte> ouf = new List<byte>();
            int ch1 = 0, ch2 = 0;
            long readtime1()
            {
                long tm = 0;
                while (true)
                {
                    int bt = original[ch1++];
                    tm = tm * 128 + (bt & 0b01111111);
                    if (bt < 128)
                    {
                        break;
                    }
                }
                return tm;
            }
            long readtime2()
            {
                long tm = 0;
                while (true)
                {
                    int bt = newls[ch2++];
                    tm = tm * 128 + (bt & 0b01111111);
                    if (bt < 128)
                    {
                        break;
                    }
                }
                return tm;
            }
            void WriteTime(long tm)
            {
                List<byte> ls = new List<byte>();
                ls.Add((byte)(tm % 128));
                tm /= 128;
                while (tm > 0)
                {
                    ls.Add((byte)(128 + tm % 128));
                    tm /= 128;
                }
                for (int i = ls.Count - 1; i >= 0; i--)
                {
                    ouf.Add(ls[i]);
                }
            }
            long TM1 = 0, TM2 = 0, TM = 0;
            int lstcmd = 256;
            while (ch2 < newls.Count)
            {
                TM2 += readtime2();
                int lstcmds = 256;
                while (ch1 < original.Count)
                {
                    int ch = ch1;
                    long tmj = readtime1();
                    if (tmj + TM1 > TM2)
                    {
                        ch1 = ch;
                        break;
                    }
                    TM1 += tmj;
                    WriteTime(TM1 - TM);
                    TM = TM1;
                    int cmds = original[ch1++];
                    if (cmds < 128)
                    {
                        ch1--;
                        cmds = lstcmds;
                    }
                    ouf.Add((byte)cmds);
                    int cms = cmds & 0b11110000;
                    if (cms == 0b10010000)
                    {
                        ouf.Add(original[ch1++]); ouf.Add(original[ch1++]);
                    }
                    else if (cms == 0b10000000)
                    {
                        ouf.Add(original[ch1++]); ouf.Add(original[ch1++]);
                    }
                    else if (cms == 0b11000000 || cms == 0b11010000 || cmds == 0b11110011)
                    {
                        ouf.Add(original[ch1++]);
                    }
                    else if (cms == 0b11100000 || cms == 0b10110000 || cmds == 0b11110010 || cms == 0b10100000)
                    {
                        ouf.Add(original[ch1++]); ouf.Add(original[ch1++]);
                    }
                    else if (cmds == 0b11110000)
                    {
                        int ffx = original[ch1++];
                        ouf.Add((byte)ffx);
                        do
                        {
                            ouf.Add((byte)(ffx = original[ch1++]));
                        } while (ffx != 0b11110111);
                    }
                    else if (cmds == 0b11110100 || cmds == 0b11110001 || cmds == 0b11110101 || cmds == 0b11111001 || cmds == 0b11111101 || cmds == 0b11110110 || cmds == 0b11110111 || cmds == 0b11111000 || cmds == 0b11111010 || cmds == 0b11111100 || cmds == 0b11111110)
                    {
                    }
                    else if (cmds == 0b11111111)
                    {
                        cmds = original[ch1++];
                        ouf.Add((byte)cmds);
                        if (cmds == 0)
                        {
                            ouf.Add(original[ch1++]); ouf.Add(original[ch1++]); ouf.Add(original[ch1++]);
                        }
                        else if (cmds >= 1 && cmds <= 10 || cmds == 0x7f)
                        {
                            long ff = readtime1();
                            WriteTime(ff);
                            while (ff-- > 0)
                            {
                                ouf.Add(original[ch1++]);
                            }
                        }
                        else if (cmds == 0x20 || cmds == 0x21)
                        {
                            ouf.Add(original[ch1++]); ouf.Add(original[ch1++]);
                        }
                        else if (cmds == 0x2f)
                        {
                            ouf.Add(original[ch1++]);
                            break;
                        }
                        else if (cmds == 0x51)
                        {
                            ouf.Add(original[ch1++]); ouf.Add(original[ch1++]); ouf.Add(original[ch1++]); ouf.Add(original[ch1++]);
                        }
                        else if (cmds == 0x54 || cmds == 0x58)
                        {
                            ouf.Add(original[ch1++]); ouf.Add(original[ch1++]); ouf.Add(original[ch1++]); ouf.Add(original[ch1++]); ouf.Add(original[ch1++]);
                        }
                        else if (cmds == 0x59)
                        {
                            ouf.Add(original[ch1++]); ouf.Add(original[ch1++]); ouf.Add(original[ch1++]);
                        }
                    }
                }
                WriteTime(TM2 - TM);
                TM = TM2;
                int cmd = newls[ch2++];
                if (cmd < 128)
                {
                    ch2--;
                    cmd = lstcmd;
                }
                ouf.Add((byte)cmd);
                int cm = cmd & 0b11110000;
                if (cm == 0b10010000)
                {
                    ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]);
                }
                else if (cm == 0b10000000)
                {
                    ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]);
                }
                else if (cm == 0b11000000 || cm == 0b11010000 || cmd == 0b11110011)
                {
                    ouf.Add(newls[ch2++]);
                }
                else if (cm == 0b11100000 || cm == 0b10110000 || cmd == 0b11110010 || cm == 0b10100000)
                {
                    ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]);
                }
                else if (cmd == 0b11110000)
                {
                    int ffx = newls[ch2++];
                    ouf.Add((byte)ffx);
                    do
                    {
                        ouf.Add((byte)(ffx = newls[ch2++]));
                    } while (ffx != 0b11110111);
                }
                else if (cmd == 0b11110100 || cmd == 0b11110001 || cmd == 0b11110101 || cmd == 0b11111001 || cmd == 0b11111101 || cmd == 0b11110110 || cmd == 0b11110111 || cmd == 0b11111000 || cmd == 0b11111010 || cmd == 0b11111100 || cmd == 0b11111110)
                {
                }
                else if (cmd == 0b11111111)
                {
                    cmd = newls[ch2++];
                    ouf.Add((byte)cmd);
                    if (cmd == 0)
                    {
                        ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]);
                    }
                    else if (cmd >= 1 && cmd <= 10 || cmd == 0x7f)
                    {
                        long ff = readtime2();
                        WriteTime(ff);
                        while (ff-- > 0)
                        {
                            ouf.Add(newls[ch2++]);
                        }
                    }
                    else if (cmd == 0x20 || cmd == 0x21)
                    {
                        ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]);
                    }
                    else if (cmd == 0x2f)
                    {
                        ouf.Add(newls[ch2++]);
                        break;
                    }
                    else if (cmd == 0x51)
                    {
                        ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]);
                    }
                    else if (cmd == 0x54 || cmd == 0x58)
                    {
                        ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]);
                    }
                    else if (cmd == 0x59)
                    {
                        ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]); ouf.Add(newls[ch2++]);
                    }
                }
            }
            while (ch1 < original.Count)
            {
                ouf.Add(original[ch1++]);
            }
            return ouf;
        }
        public long ImplaceMerge(Groups grp, bool impmrg, long offst, bool rembpm, int trppq, int ppq, int minvol, bool remb, bool remc)
        {
            if (grp.file.EndsWith(".cjcamm"))
            {
                if (grp.st.TrsPpq)
                {
                    ppq = grp.ppq;
                }
                long res = 0;
                for (int i = 0; i < grp.ms.Count; i++)
                {
                    long x = ImplaceMerge(grp.ms[i], impmrg, offst + grp.ms[i].st.offst, rembpm || grp.ms[i].st.RemoveBpm, (trppq == 0 && grp.ms[i].st.TrsPpq) ? grp.ppq : trppq, ppq, minvol < grp.ms[i].st.minvol ? grp.ms[i].st.minvol : minvol, remb || grp.ms[i].st.RemPB, remc || grp.ms[i].st.RemPC);
                    if (x > res)
                    {
                        res = x;
                    }
                }
                return res;
            }
            else
            {
                List<byte> oun = new List<byte>();
                long res = 0;
                BufferedStream buff = new BufferedStream(File.Open(grp.file, FileMode.Open, FileAccess.Read, FileShare.Read), 1048576);
                for (int i = 0; i < 10; i++)
                {
                    buff.ReadByte();
                }
                int trkcnt = buff.ReadByte();
                trkcnt = trkcnt * 256 + buff.ReadByte();
                if (grp.st.TrsPpq)
                {
                    ppq = grp.ppq;
                }
                buff.ReadByte(); buff.ReadByte();
                for (int trk = 0; trk < trkcnt; trk++)
                {
                    List<byte> ouf = new List<byte>();
                    for (int i = 0; i < 4; i++)
                    {
                        buff.ReadByte();
                    }
                    void WriteTime(long tm)
                    {
                        List<byte> ls = new List<byte>();
                        ls.Add((byte)(tm % 128));
                        tm /= 128;
                        while (tm > 0)
                        {
                            ls.Add((byte)(128 + tm % 128));
                            tm /= 128;
                        }
                        for (int i = ls.Count - 1; i >= 0; i--)
                        {
                            ouf.Add(ls[i]);
                        }
                    }
                    int lstcmd = 256, nxtcmd = 256;
                    long len = buff.ReadByte();
                    for (int i = 1; i < 4; i++)
                    {
                        len = len * 256 + buff.ReadByte();
                    }
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
                    int[] hold = new int[4096];
                    int[] rem = new int[4096];
                    for (int i = 0; i < 4096; i++)
                    {
                        hold[i] = 0;
                        rem[i] = 0;
                    }
                    long lsttm = -offst;
                    long TM = 0, TT = 0;
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
                        TT += readtime();
                        if (trppq == 0)
                        {
                            TM = TT;
                        }
                        else
                        {
                            TM = TT * trppq / ppq;
                        }
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
                            if (impmrg)
                            {
                                int nt = getbyte();
                                int ch = cmd & 0b00001111;
                                int v = getbyte();
                                if (v >= minvol)
                                {
                                    hold[ch * 256 + nt]++;
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)nt);
                                    ouf.Add((byte)v);
                                }
                                else
                                {
                                    rem[ch * 256 + nt]++;
                                }
                            }
                            else
                            {
                                getbyte(); getbyte();
                            }
                        }
                        else if (cm == 0b10000000)
                        {
                            if (impmrg)
                            {
                                int nt = getbyte();
                                int ch = cmd & 0b00001111;
                                if (rem[ch * 256 + nt] == 0)
                                {
                                    if (hold[ch * 256 + nt] > 0)
                                    {
                                        hold[ch * 256 + nt]--;
                                        WriteTime(TM - lsttm);
                                        lsttm = TM;
                                        ouf.Add((byte)cmd);
                                        ouf.Add((byte)nt);
                                        ouf.Add((byte)getbyte());
                                    }
                                    else
                                    {
                                        getbyte();
                                    }
                                }
                                else
                                {
                                    getbyte();
                                    rem[ch * 256 + nt]--;
                                }
                            }
                            else
                            {
                                getbyte(); getbyte();
                            }
                        }
                        else if (cm == 0b11000000)
                        {
                            if (impmrg && !remc)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                ouf.Add((byte)getbyte());
                            }
                            else
                            {
                                getbyte();
                            }
                        }
                        else if (cm == 0b11010000 || cmd == 0b11110011)
                        {
                            if (impmrg)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                ouf.Add((byte)getbyte());
                            }
                            else
                            {
                                getbyte();
                            }
                        }
                        else if (cm == 0b11100000)
                        {
                            if (impmrg && !remb)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                ouf.Add((byte)getbyte());
                                ouf.Add((byte)getbyte());
                            }
                            else
                            {
                                getbyte(); getbyte();
                            }
                        }
                        else if (cm == 0b10110000 || cmd == 0b11110010 || cm == 0b10100000)
                        {
                            if (impmrg)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                ouf.Add((byte)getbyte());
                                ouf.Add((byte)getbyte());
                            }
                            else
                            {
                                getbyte(); getbyte();
                            }
                        }
                        else if (cmd == 0b11110000)
                        {
                            if (impmrg)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                int ffx = getbyte();
                                ouf.Add((byte)ffx);
                                do
                                {
                                    ffx = getbyte();
                                    ouf.Add((byte)ffx);
                                } while (ffx != 0b11110111);
                            }
                            else
                            {
                                int ffx = getbyte();
                                do
                                {
                                    ffx = getbyte();
                                } while (ffx != 0b11110111);
                            }
                        }
                        else if (cmd == 0b11110100 || cmd == 0b11110001 || cmd == 0b11110101 || cmd == 0b11111001 || cmd == 0b11111101 || cmd == 0b11110110 || cmd == 0b11110111 || cmd == 0b11111000 || cmd == 0b11111010 || cmd == 0b11111100 || cmd == 0b11111110)
                        {
                            if (impmrg)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                            }
                        }
                        else if (cmd == 0b11111111)
                        {
                            cmd = getbyte();
                            if (cmd == 0)
                            {
                                if (impmrg)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                }
                                else
                                {
                                    getbyte(); getbyte(); getbyte();
                                }
                            }
                            else if (cmd >= 1 && cmd <= 10 || cmd == 0x7f)
                            {
                                if (impmrg)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    long ff = readtime();
                                    WriteTime(ff);
                                    while (ff-- > 0)
                                    {
                                        ouf.Add((byte)getbyte());
                                    }
                                }
                                else
                                {
                                    long ff = readtime();
                                    while (ff-- > 0)
                                    {
                                        getbyte();
                                    }
                                }
                            }
                            else if (cmd == 0x20 || cmd == 0x21)
                            {
                                if (impmrg)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                }
                                else
                                {
                                    getbyte(); getbyte();
                                }
                            }
                            else if (cmd == 0x2f)
                            {
                                getbyte();
                                break;
                            }
                            else if (cmd == 0x51)
                            {
                                if (!rembpm)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                }
                                else
                                {
                                    getbyte(); getbyte(); getbyte(); getbyte();
                                }
                            }
                            else if (cmd == 0x54 || cmd == 0x58)
                            {
                                if (impmrg)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                }
                                else
                                {
                                    getbyte(); getbyte(); getbyte(); getbyte(); getbyte();
                                }
                            }
                            else if (cmd == 0x59)
                            {
                                if (impmrg)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                }
                                else
                                {
                                    getbyte(); getbyte(); getbyte(); getbyte();
                                }
                            }
                        }
                    }
                    while (len > 0)
                    {
                        len--;
                        buff.ReadByte();
                    }
                    tmp3 = ImplaceTrks(tmp3, ouf);
                    if (tmp3.Count > tmp2.Count)
                    {
                        tmp2 = ImplaceTrks(tmp2, tmp3);
                        tmp3 = new List<byte>();
                    }
                    if (tmp2.Count > tmp.Count)
                    {
                        tmp = ImplaceTrks(tmp, tmp2);
                        tmp2 = new List<byte>();
                    }
                    trkp++;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Progress.Text = (string)Progress.DataContext;
                        Progress.Text = Progress.Text.Replace("{trks}", trkp.ToString() + "/" + alltrk.ToString());
                        Progress.Text = Progress.Text.Replace("{wrtt}", trks.ToString());
                    }));
                }
                buff.Close();
                return res;
            }
        }
        public void WriteMidis(Groups grp, bool rembpm, bool impmrg, bool remept, long offst, bool ImpBpm, int trppq, int ppq, int minvol, bool remb, bool remc)
        {
            if (!impmrg && !ImpBpm)
            {
                if (grp.file.EndsWith(".cjcamm"))
                {
                    if (grp.st.TrsPpq)
                    {
                        ppq = grp.ppq;
                    }
                    for (int i = 0; i < grp.ms.Count; i++)
                    {
                        WriteMidis(grp.ms[i], rembpm || grp.ms[i].st.RemoveBpm, grp.ms[i].st.ImpMrg, remept || grp.ms[i].st.RemEpt, offst + grp.ms[i].st.offst, grp.ms[i].st.ImpBpm, (trppq == 0 && grp.ms[i].st.TrsPpq) ? grp.ppq : trppq, ppq, minvol < grp.ms[i].st.minvol ? grp.ms[i].st.minvol : minvol, remb || grp.ms[i].st.RemPB, remc || grp.ms[i].st.RemPC);
                    }
                }
                else
                {
                    BufferedStream buff = new BufferedStream(File.Open(grp.file, FileMode.Open, FileAccess.Read, FileShare.Read), 1048576);
                    for (int i = 0; i < 10; i++)
                    {
                        buff.ReadByte();
                    }
                    int trkcnt = buff.ReadByte();
                    trkcnt = trkcnt * 256 + buff.ReadByte();
                    if (grp.st.TrsPpq)
                    {
                        ppq = grp.ppq;
                    }
                    buff.ReadByte(); buff.ReadByte();
                    for (int trk = 0; trk < trkcnt; trk++)
                    {
                        bool empt = true;
                        List<byte> ouf = new List<byte>();
                        for (int i = 0; i < 4; i++)
                        {
                            buff.ReadByte();
                        }
                        void WriteTime(long tm)
                        {
                            List<byte> ls = new List<byte>();
                            ls.Add((byte)(tm % 128));
                            tm /= 128;
                            while (tm > 0)
                            {
                                ls.Add((byte)(128 + tm % 128));
                                tm /= 128;
                            }
                            for (int i = ls.Count - 1; i >= 0; i--)
                            {
                                ouf.Add(ls[i]);
                            }
                        }
                        int lstcmd = 256, nxtcmd = 256;
                        long len = buff.ReadByte();
                        for (int i = 1; i < 4; i++)
                        {
                            len = len * 256 + buff.ReadByte();
                        }
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
                        int[] hold = new int[4096];
                        int[] rem = new int[4096];
                        for (int i = 0; i < 4096; i++)
                        {
                            hold[i] = 0;
                            rem[i] = 0;
                        }
                        long lsttm = -offst;
                        long TM = 0, TT = 0;
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
                            TT += readtime();
                            if (trppq != 0)
                            {
                                TM = TT * trppq / ppq;
                            }
                            else
                            {
                                TM = TT;
                            }
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
                                int nt = getbyte();
                                int ch = cmd & 0b00001111;
                                int v = getbyte();
                                if (v >= minvol)
                                {
                                    hold[ch * 256 + nt]++;
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)nt);
                                    ouf.Add((byte)v);
                                    empt = false;
                                }
                                else
                                {
                                    rem[ch * 256 + nt]++;
                                }
                            }
                            else if (cm == 0b10000000)
                            {
                                int nt = getbyte();
                                int ch = cmd & 0b00001111;
                                if (rem[ch * 256 + nt] == 0)
                                {
                                    if (hold[ch * 256 + nt] > 0)
                                    {
                                        hold[ch * 256 + nt]--;
                                        WriteTime(TM - lsttm);
                                        lsttm = TM;
                                        ouf.Add((byte)cmd);
                                        ouf.Add((byte)nt);
                                        ouf.Add((byte)getbyte());
                                        empt = false;
                                    }
                                    else
                                    {
                                        getbyte();
                                    }
                                }
                                else
                                {
                                    rem[ch * 256 + nt]--;
                                    getbyte();
                                }
                            }
                            else if (cm == 0b11000000)
                            {
                                if (!remc)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte());
                                }
                                else
                                {
                                    getbyte();
                                }
                            }
                            else if (cm == 0b11010000 || cmd == 0b11110011)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                ouf.Add((byte)getbyte());
                                empt = false;
                            }
                            else if (cm == 0b11100000)
                            {
                                if (!remb)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte());
                                    ouf.Add((byte)getbyte());
                                }
                                else
                                {
                                    getbyte(); getbyte();
                                }
                            }
                            else if (cm == 0b10110000 || cmd == 0b11110010 || cm == 0b10100000)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                ouf.Add((byte)getbyte());
                                ouf.Add((byte)getbyte());
                                empt = false;
                            }
                            else if (cmd == 0b11110000)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                int ffx = getbyte();
                                ouf.Add((byte)ffx);
                                do
                                {
                                    ffx = getbyte();
                                    ouf.Add((byte)ffx);
                                } while (ffx != 0b11110111);
                                empt = false;
                            }
                            else if (cmd == 0b11110100 || cmd == 0b11110001 || cmd == 0b11110101 || cmd == 0b11111001 || cmd == 0b11111101 || cmd == 0b11110110 || cmd == 0b11110111 || cmd == 0b11111000 || cmd == 0b11111010 || cmd == 0b11111100 || cmd == 0b11111110)
                            {
                                WriteTime(TM - lsttm);
                                lsttm = TM;
                                ouf.Add((byte)cmd);
                                empt = false;
                            }
                            else if (cmd == 0b11111111)
                            {
                                cmd = getbyte();
                                if (cmd == 0)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                    empt = false;
                                }
                                else if (cmd >= 1 && cmd <= 10 || cmd == 0x7f)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    long ff = readtime();
                                    WriteTime(ff);
                                    while (ff-- > 0)
                                    {
                                        ouf.Add((byte)getbyte());
                                    }
                                    empt = false;
                                }
                                else if (cmd == 0x20 || cmd == 0x21)
                                {
                                    empt = false;
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                }
                                else if (cmd == 0x2f)
                                {
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte());
                                    break;
                                }
                                else if (cmd == 0x51)
                                {
                                    if (!rembpm)
                                    {
                                        empt = false;
                                        WriteTime(TM - lsttm);
                                        lsttm = TM;
                                        ouf.Add((byte)0b11111111);
                                        ouf.Add((byte)cmd);
                                        ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                    }
                                    else
                                    {
                                        getbyte(); getbyte(); getbyte(); getbyte();
                                    }
                                }
                                else if (cmd == 0x54 || cmd == 0x58)
                                {
                                    empt = false;
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                }
                                else if (cmd == 0x59)
                                {
                                    empt = false;
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte()); ouf.Add((byte)getbyte());
                                }
                                else if (cmd == 0x0a)
                                {
                                    empt = false;
                                    WriteTime(TM - lsttm);
                                    lsttm = TM;
                                    ouf.Add((byte)0b11111111);
                                    ouf.Add((byte)cmd);
                                    int ffx = getbyte();
                                    ouf.Add((byte)ffx);
                                    while (ffx-- > 0)
                                    {
                                        ouf.Add((byte)getbyte());
                                    }
                                }
                            }
                        }
                        while (len > 0)
                        {
                            len--;
                            buff.ReadByte();
                        }
                        trkp++;
                        if (!empt || !remept)
                        {
                            ous.WriteByte((byte)'M'); ous.WriteByte((byte)'T'); ous.WriteByte((byte)'r'); ous.WriteByte((byte)'k');
                            long lens = ouf.Count();
                            ous.WriteByte((byte)(lens / 256 / 256 / 256));
                            ous.WriteByte((byte)(lens / 256 / 256 % 256));
                            ous.WriteByte((byte)(lens / 256 % 256));
                            ous.WriteByte((byte)(lens % 256));
                            trks++;
                            //ous.Write(ouf.ToArray(), 0, ouf.Count);
                            for (int i = 0; i < ouf.Count; i++)
                            {
                                ous.WriteByte(ouf[i]);
                            }
                        }
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Progress.Text = (string)Progress.DataContext;
                            Progress.Text = Progress.Text.Replace("{trks}", trkp.ToString() + "/" + alltrk.ToString());
                            Progress.Text = Progress.Text.Replace("{wrtt}", trks.ToString());
                        }));
                    }
                    buff.Close();
                }
            }
            else
            {
                long tms = ImplaceMerge(grp, impmrg, offst, rembpm, trppq, ppq, minvol, remb, remc);
                tmp2 = ImplaceTrks(tmp2, tmp3);
                tmp = ImplaceTrks(tmp, tmp2);
                if (tmp.Count > 0)
                {
                    tmp.Add(0);
                    tmp.Add(0xff);
                    tmp.Add(0x2f);
                    tmp.Add(0);
                    ous.WriteByte((byte)'M'); ous.WriteByte((byte)'T'); ous.WriteByte((byte)'r'); ous.WriteByte((byte)'k');
                    long lens = tmp.Count();
                    ous.WriteByte((byte)(lens / 256 / 256 / 256));
                    ous.WriteByte((byte)(lens / 256 / 256 % 256));
                    ous.WriteByte((byte)(lens / 256 % 256));
                    ous.WriteByte((byte)(lens % 256));
                    trks++;
                    ous.Write(tmp.ToArray(), 0, tmp.Count);
                    tmp.Clear();
                    tmp2 = new List<byte>();
                    tmp3 = new List<byte>();
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Progress.Text = (string)Progress.DataContext;
                        Progress.Text = Progress.Text.Replace("{trks}", trkp.ToString() + "/" + alltrk.ToString());
                        Progress.Text = Progress.Text.Replace("{wrtt}", trks.ToString());
                    }));
                }
                if (!impmrg)
                {
                    WriteMidis(grp, true, false, remept, offst, false, trppq, ppq, minvol, remb, remc);
                }
            }
        }
        public void StartMerge()
        {
            ous.WriteByte((byte)'M');
            ous.WriteByte((byte)'T');
            ous.WriteByte((byte)'h');
            ous.WriteByte((byte)'d');
            ous.WriteByte(0);
            ous.WriteByte(0);
            ous.WriteByte(0);
            ous.WriteByte(6);
            ous.WriteByte(0);
            ous.WriteByte(1);
            ous.WriteByte(0);
            ous.WriteByte(0);
            ous.WriteByte((byte)(gr.ppq / 256));
            ous.WriteByte((byte)(gr.ppq % 256));
            trkcount(gr, false);
            Dispatcher.Invoke(new Action(() =>
            {
                Progress.Text = (string)Progress.DataContext;
                Progress.Text = Progress.Text.Replace("{trks}", "0/" + alltrk.ToString());
                Progress.Text = Progress.Text.Replace("{wrtt}", "0");
            }));
            WriteMidis(gr, false, false, false, 0, false, 0, gr.ppq, 0, false, false);
            ous.Flush();
            ous.Seek(10, SeekOrigin.Begin);
            ous.WriteByte((byte)(trks / 256));
            ous.WriteByte((byte)(trks % 256));
            ous.Flush();
            ous.Close();
            Dispatcher.Invoke(new Action(() =>
            {
                Close();
            }));
        }
    }
}
