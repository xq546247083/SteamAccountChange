﻿using SteamAccountChange.Common;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SteamAccountChange.View
{
    /// <summary>
    /// 通知
    /// </summary>
    public static class Notify
    {
        #region 私有方法

        /// <summary>
        /// 通知图标
        /// </summary>
        private static NotifyIcon notifyIcon;

        /// <summary>
        /// 加载菜单
        /// </summary>
        private static void LoadMenu()
        {
            var menuList = new List<MenuItem>();

            // 添加账号到菜单
            var saveInfo = SteamHelper.GetSaveInfo();
            foreach (var item in saveInfo.SteamAccoutInfoList)
            {
                var steamAccountMenu = new MenuItem(item.Name);
                steamAccountMenu.Tag = item.Account;
                steamAccountMenu.Click += SteamAccountMenu_Click;

                menuList.Add(steamAccountMenu);
            }

            // 重载信息
            var reloadSaveInfo = new MenuItem("重载信息");
            reloadSaveInfo.Click += ReloadSaveInfo_Click;
            menuList.Add(reloadSaveInfo);

            // 退出菜单项
            var exit = new MenuItem("退出");
            exit.Click += Exit_Click;
            menuList.Add(exit);

            // 关联托盘控件
            notifyIcon.ContextMenu = new ContextMenu(menuList.ToArray());
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = "Steam账号切换器";
            notifyIcon.Icon = SteamAccountChange.Properties.Resources.steam;
            notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            LoadMenu();
        }

        #endregion

        #region 事件

        /// <summary>
        /// steam账号菜单点击
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void SteamAccountMenu_Click(object sender, EventArgs e)
        {
            var steamAccountMenu = sender as MenuItem;
            if (steamAccountMenu == null)
            {
                return;
            }

            SteamHelper.OpenSteam(steamAccountMenu.Tag.ToString());
        }

        /// <summary>
        /// 重载信息
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void ReloadSaveInfo_Click(object sender, EventArgs e)
        {
            LoadMenu();
            Lactor.MainWindowViewModel.LoadSaveInfo();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            System.Environment.Exit(0);
        }

        /// <summary>
        /// 双击显示窗口
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Lactor.MainWindow.Visibility = System.Windows.Visibility.Visible;
            Lactor.MainWindow.ShowInTaskbar = true;
            Lactor.MainWindow.Activate();
        }

        #endregion
    }
}
