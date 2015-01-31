﻿using System;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace PKHeX
{
    public partial class SplashScreen : Form
    {
        Form1 m_parent;
        public SplashScreen(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
            myTimer.Interval = 50; // milliseconds per trigger interval
            myTimer.Start();
        }
        public void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            if (!m_parent.init)
                if (L_Status.InvokeRequired) L_Status.Invoke((MethodInvoker)delegate { L_Status.Text = Form1.Status; });
                else { L_Status.Text = Form1.Status; }
            else
                if (InvokeRequired && IsHandleCreated)
                    try { Invoke((MethodInvoker)delegate() { Close(); }); } catch { Close(); }
                else Close();
        }
    }
}