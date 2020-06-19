using Microsoft.Win32;
using SteamAccountChange.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamAccountChange
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Init();
            RegisteEvent();
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisteEvent()
        {
            this.KeyDown += MainWindow_KeyDown;
        }

        /// <summary>
        /// 按键
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    MessageBox.Show("1、请在程序运行目录创建一个Steamaccount.txt \n\r2、把【Name ID】写在Steamaccount.txt里，一行一个。\n\rPS:各个账号都在电脑上点了记住密码登录过。", "提示");
                    break;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            try
            {
                cbAccount.Items.Clear();

                // 读取记事本
                var steamAccountFileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SteamAccount.txt"));
                StreamReader streamReader = steamAccountFileInfo.OpenText();

                // 读取账号
                string strLine = string.Empty;
                while (!string.IsNullOrEmpty(strLine = streamReader.ReadLine()))
                {
                    var strList = strLine.Split(' ');
                    var steamAccount = new SteamAccoutInfo()
                    {
                        Name = strList[0],
                        Account = strList.Length >= 2 ? strList[1] : strList[0],
                    };
                    cbAccount.Items.Add(steamAccount);
                }
                streamReader.Dispose();

                // 选中第一个
                if (cbAccount.Items.Count > 0) 
                {
                    cbAccount.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 点击确定
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string user = cbAccount.SelectedValue.ToString();
                SetSteamRegistry("AutoLoginUser", user);

                KillSteamProcess();
                string steamExe = GetSteamExe();
                Process.Start(steamExe);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取注册信息
        /// </summary>
        /// <returns></returns>
        private string GetSteamExe()
        {
            var currentUser = Registry.CurrentUser;
            if (currentUser == null)
            {
                return string.Empty;
            }

            var software = currentUser.OpenSubKey("SOFTWARE", true);
            if (software == null)
            {
                return string.Empty;
            }

            var vavle = software.OpenSubKey("Valve", true);
            if (vavle == null)
            {
                return string.Empty;
            }

            var steam = vavle.OpenSubKey("Steam", true);
            if (steam == null)
            {
                return string.Empty;
            }

            return steam.GetValue("SteamExe").ToString();
        }

        /// <summary>
        /// 设置Steam注册表
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        private void SetSteamRegistry(string key, string value)
        {
            var currentUser = Registry.CurrentUser;
            if (currentUser == null) 
            {
                return;
            }

            var software = currentUser.OpenSubKey("SOFTWARE", true);
            if (software == null)
            {
                return;
            }

            var valve = software.OpenSubKey("Valve", true);
            if (valve == null)
            {
                return;
            }

            var steam = valve.OpenSubKey("Steam", true);
            if (steam == null)
            {
                return;
            }

            steam.SetValue(key, value);
        }

        /// <summary>
        /// 杀steam进程
        /// </summary>
        private void KillSteamProcess()
        {
            var processList = Process.GetProcesses();
            foreach (Process item in processList)
            {
                if (item.ProcessName.ToLower() == "steam")
                {
                    item.Kill();
                }
            }
        }
    }
}
