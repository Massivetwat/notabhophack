using System;
using System.Windows.Forms;
using System.Threading;
using Memory;
using System.Runtime.InteropServices;

namespace notabhophack
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey); // hotkey

        /* 
         * module = client.dll
         * localplayer = 0xD8C2CC
         * m_fFlags = 0x104
         * dwForceJump = 0x524DEDC
        */

        string localplayerAndmfFlag = "client.dll+0xD8C2BC,0x104"; // module + localplayer + fFlag
        string ForceJump = "client.dll+0x524DEDC"; // module + forcejump


        Mem m = new Mem(); // memory read and write class 
        int flag; 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int PID = m.GetProcIdFromName("csgo"); // get proc id from csgo.exe, check if running)
            if (PID > 0)
            {
                m.OpenProcess(PID);
                Thread bh = new Thread(HOP) { IsBackground = true }; // Create a separate thread in the background to run our bhop loop
                bh.Start();
            }
        }

        void HOP()
        {
            while (true)
            {
                if (GetAsyncKeyState(Keys.Space)<0) // If spacebar is down 
                {
                    flag = m.ReadInt(localplayerAndmfFlag); 
                    if (flag == 257 || flag == 263) // if we are grounded, standing or crouching
                    {
                        m.WriteMemory(ForceJump, "int", "5"); // +jump
                        Thread.Sleep(20);
                        m.WriteMemory(ForceJump, "int", "4"); // -jump
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
