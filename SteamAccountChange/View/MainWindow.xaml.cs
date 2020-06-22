﻿using SteamAccountChange.Common;
using System.Windows;
using System.Windows.Input;

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
            this.Closing += MainWindow_Closing;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            cbAccount.Items.Clear();

            // 加载账号信息
            var steamAccoutInfoList = SteamHelper.GetSteamAccoutInfoList();
            cbAccount.ItemsSource = steamAccoutInfoList;

            // 选中第一个
            if (cbAccount.Items.Count > 0)
            {
                cbAccount.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 点击确定
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbAccount.SelectedValue != null)
                SteamHelper.OpenSteam(cbAccount.SelectedValue.ToString());
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
                    MessageBox.Show("1、请在程序运行目录创建一个Steamaccount.txt \n\r2、把【Name ID】写在Steamaccount.txt里，一行一个。\n\rPS:各个账号都在电脑上点了记住密码登录过。", "帮助");
                    break;
            }
        }

        /// <summary>
        /// 正在关闭窗口
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Lactor.MainWindow.Visibility = System.Windows.Visibility.Hidden;
            Lactor.MainWindow.ShowInTaskbar = false;

            e.Cancel = true;
        }
    }
}
