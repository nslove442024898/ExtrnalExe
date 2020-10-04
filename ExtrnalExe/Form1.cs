using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ExtrnalExe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //var acad = FindWindow("AfxMDIFrame140u", null);
            var pros = Process.GetProcessesByName("acad");
            if (pros.Length == 0)
            {
                MessageBox.Show("没有找到acad进程，请检查后再运行本程序！");
                return;
            }
            SendCommandToAutoCAD("_circle 0,0,0 300 ", pros.First().MainWindowHandle);
            SetForegroundWindow(pros.First().MainWindowHandle);
        }
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        private void SendCommandToAutoCAD(string toSend, IntPtr hwnd)
        {
            const int WM_COPYDATA = 0x4A;
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = new IntPtr(1);
            string data = toSend + "\0";
            cds.cbData = data.Length * Marshal.SystemDefaultCharSize;
            cds.lpData = Marshal.StringToCoTaskMemAuto(data);
            SendMessageW(hwnd, WM_COPYDATA, IntPtr.Zero, ref cds);
            Marshal.FreeCoTaskMem(cds.lpData);
        }
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;

            public int cbData;

            public IntPtr lpData;

        }
    }
}
