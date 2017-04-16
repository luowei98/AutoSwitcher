using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using AutoSwitcher.Input.Api;
using AutoSwitcher.Window.Api;
using log4net;


namespace AutoSwitcher
{
    public partial class MainForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainForm).Assembly, "AutoSwitcher");

        private IList<IWindowEntry> windows;
        private int currWindow;
        private bool run;
        private int stops;
        private KeyboardHook kbhook;
        private MouseHook mshook;

        private int stopTimes = 3;

        public MainForm()
        {
            InitializeComponent();
        }

        ~MainForm()
        {
            kbhook?.Dispose();
            mshook?.Dispose();
        }

        private void HookAction()
        {
            run = false;
            stops = 0;
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStop.Text) || string.IsNullOrEmpty(txtInterval.Text))
            {
                MessageBox.Show("请输入正确时间间隔");
                return;
            }

            timer.Interval = (int) (txtInterval.Text.ToNum()*1000*60);

            if (txtStop.Text.ToNum() > txtInterval.Text.ToNum())
            {
                stopTimes = (int) (txtStop.Text.ToNum()/txtInterval.Text.ToNum());
            }
            else
            {
                stopTimes = 1;
            }

            if (timer.Enabled)
            {
                button.Text = "开始";

                run = false;
                timer.Stop();

                kbhook?.Dispose();
                mshook?.Dispose();
            }
            else
            {
                button.Text = "停止";

                SetWindowsList();
                run = true;

                kbhook?.Dispose();
                kbhook = KeyboardHook.Hook(HookAction);

                mshook?.Dispose();
                mshook = MouseHook.Hook(HookAction);

                timer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            if (!run)
            {
                if (stops < stopTimes)
                {
                    stops++;
                    timer.Start();
                    return;
                }
                SetWindowsList();
                run = true;
            }

            currWindow = ++currWindow%windows.Count;
            windows[currWindow].Focus();

            timer.Start();
        }

        private void SetWindowsList()
        {
            var windowsList = WindowsListFactory.Load();
            windows = windowsList.Windows;

            foreach (var w in windows)
            {
                Log.Info(w.ProcessName);
            }
        }

        private void txtInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 46 && 
                !char.IsDigit(e.KeyChar) && e.KeyChar != 13)
            {
                e.Handled = true;
            }
        }

        private void txtInterval_Leave(object sender, EventArgs e)
        {
            txtInterval.Text = txtInterval.Text.ToNum().ToString(CultureInfo.CurrentCulture);
        }

        private void txtStop_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 46 &&
                !char.IsDigit(e.KeyChar) && e.KeyChar != 13)
            {
                e.Handled = true;
            }
        }

        private void txtStop_Leave(object sender, EventArgs e)
        {
            txtStop.Text = txtStop.Text.ToNum().ToString(CultureInfo.CurrentCulture);
        }
    }
}
