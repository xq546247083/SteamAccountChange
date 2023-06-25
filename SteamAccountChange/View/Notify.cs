using SteamAccountChange.Common;
using System;
using System.Linq;
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

        /// <summary>
        /// 加载菜单
        /// </summary>
        public static void LoadMenu()
        {
            var menuList = new List<MenuItem>();

            // 添加账号到菜单
            var steamAccoutInfoList = ConfigHelper.GetConfig().SteamAccoutInfoList.OrderBy(r => r.Order).ThenBy(r => r.Account).ToList();
            foreach (var item in steamAccoutInfoList)
            {
                var steamAccountMenu = new MenuItem(item.Name);
                steamAccountMenu.Tag = item.Account;
                steamAccountMenu.Click += SteamAccountMenu_Click;

                menuList.Add(steamAccountMenu);
            }

            // 开机自启用菜单项
            var launchOnSysPowerOnMenu = new MenuItem("开机自启用");
            launchOnSysPowerOnMenu.Click += LaunchOnSysPowerOn_Click;
            menuList.Add(launchOnSysPowerOnMenu);

            // 退出菜单项
            var exitMenu = new MenuItem("退出");
            exitMenu.Click += Exit_Click;
            menuList.Add(exitMenu);

            // 关联托盘控件
            notifyIcon.ContextMenu = new ContextMenu(menuList.ToArray());
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
        /// 开机自启用
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void LaunchOnSysPowerOn_Click(object sender, EventArgs e)
        {
            var launchOnSysPowerOnMenu = sender as MenuItem;
            if (launchOnSysPowerOnMenu == null)
            {
                return;
            }

            launchOnSysPowerOnMenu.Checked = !launchOnSysPowerOnMenu.Checked;
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
