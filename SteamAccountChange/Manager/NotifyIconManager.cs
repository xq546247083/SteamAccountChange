using Microsoft.Win32;
using SteamAccountChange.Helper;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SteamAccountChange.Manager
{
    /// <summary>
    /// 通知管理
    /// </summary>
    public static class NotifyIconManager
    {
        #region 私有属性

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
            notifyIcon.Icon = Properties.Resources.steam;
            notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            LoadMenu();
        }

        /// <summary>
        /// 加载菜单
        /// </summary>
        public static void LoadMenu()
        {
            var contextMenuStrip = new ContextMenuStrip();

            // 添加账号到菜单
            var steamAccoutInfoList = LocalDataHelper.GetLocalData().SteamAccoutInfoList.OrderBy(r => r.Order).ThenBy(r => r.Account).ToList();
            foreach (var item in steamAccoutInfoList)
            {
                var steamAccountMenu = new ToolStripMenuItem(item.Name);
                steamAccountMenu.Tag = item.Account;
                steamAccountMenu.Click += SteamAccountMenu_Click;

                contextMenuStrip.Items.Add(steamAccountMenu);
            }

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            // 切换新账号
            var newAccountMenu = new ToolStripMenuItem("添加新账号");
            newAccountMenu.Click += NewAccountMenu_Click;
            contextMenuStrip.Items.Add(newAccountMenu);

            // 注册表
            var launchOnSysPowerOnByRegisterMenu = new ToolStripMenuItem("注册表");
            launchOnSysPowerOnByRegisterMenu.Checked = GetIsLaunchOnSysPowerOnByRegister();
            launchOnSysPowerOnByRegisterMenu.Click += LaunchOnSysPowerOnByRegister_Click;

            // 计划任务
            var launchOnSysPowerOnByTaskSchedulerMenu = new ToolStripMenuItem("计划任务(跳过UAC)");
            launchOnSysPowerOnByTaskSchedulerMenu.Checked = GetIsLaunchOnSysPowerOnByTaskScheduler();
            launchOnSysPowerOnByTaskSchedulerMenu.Click += LaunchOnSysPowerOnByTaskScheduler_Click;

            // 开机自启用菜单项
            var launchOnSysPowerOnMenu = new ToolStripMenuItem("开机自启动");
            launchOnSysPowerOnMenu.DropDownItems.Add(launchOnSysPowerOnByRegisterMenu);
            launchOnSysPowerOnMenu.DropDownItems.Add(launchOnSysPowerOnByTaskSchedulerMenu);
            contextMenuStrip.Items.Add(launchOnSysPowerOnMenu);

            // 退出菜单项
            var exitMenu = new ToolStripMenuItem("退出");
            exitMenu.Click += Exit_Click;
            contextMenuStrip.Items.Add(exitMenu);

            // 关联托盘控件
            notifyIcon.ContextMenuStrip = contextMenuStrip;
        }

        #endregion

        /// <summary>
        /// 获取是否注册表自启动
        /// </summary>
        /// <returns></returns>
        private static bool GetIsLaunchOnSysPowerOnByRegister()
        {
            var (getSuccess, appPathInfo) = RegistryHelper.Get(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", Local.AppName, Registry.LocalMachine);
            if (getSuccess == false || appPathInfo == null)
            {
                return false;
            }

            var appPathStr = appPathInfo.ToString();
            if (string.IsNullOrEmpty(appPathStr))
            {
                return false;
            }

            return appPathStr == Application.ExecutablePath;
        }

        /// <summary>
        /// 获取是否计划任务自启动
        /// </summary>
        /// <returns></returns>
        private static bool GetIsLaunchOnSysPowerOnByTaskScheduler()
        {
            var taslScheduler = TaskSchedulerHelper.Get(Local.AppName);
            return taslScheduler != null;
        }

        #region 事件

        /// <summary>
        /// 切换新账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void NewAccountMenu_Click(object sender, EventArgs e)
        {
            SteamHelper.OpenSteam(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// steam账号菜单点击
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void SteamAccountMenu_Click(object sender, EventArgs e)
        {
            var steamAccountMenu = sender as ToolStripMenuItem;
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
        /// 注册表，开机自启用
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void LaunchOnSysPowerOnByRegister_Click(object sender, EventArgs e)
        {
            var launchOnSysPowerOnByRegisterMenu = sender as ToolStripMenuItem;
            if (launchOnSysPowerOnByRegisterMenu == null)
            {
                return;
            }

            // 修改是否自启动
            var currentValue = !launchOnSysPowerOnByRegisterMenu.Checked;
            if (currentValue)
            {
                if (RegistryHelper.Set(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", Local.AppName, Application.ExecutablePath, Registry.LocalMachine))
                {
                    launchOnSysPowerOnByRegisterMenu.Checked = currentValue;
                }
            }
            else
            {
                if (RegistryHelper.Del(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", Local.AppName, Registry.LocalMachine))
                {
                    launchOnSysPowerOnByRegisterMenu.Checked = currentValue;
                }
            }
        }

        /// <summary>
        /// 计划任务，开机自启用
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void LaunchOnSysPowerOnByTaskScheduler_Click(object sender, EventArgs e)
        {
            var launchOnSysPowerOnByTaskSchedulerMenu = sender as ToolStripMenuItem;
            if (launchOnSysPowerOnByTaskSchedulerMenu == null)
            {
                return;
            }

            // 修改是否自启动
            var currentValue = !launchOnSysPowerOnByTaskSchedulerMenu.Checked;
            if (currentValue)
            {
                if (TaskSchedulerHelper.AddLuanchTask(Local.AppName, Application.ExecutablePath))
                {
                    launchOnSysPowerOnByTaskSchedulerMenu.Checked = currentValue;
                }
            }
            else
            {
                TaskSchedulerHelper.Del(Local.AppName);
                launchOnSysPowerOnByTaskSchedulerMenu.Checked = currentValue;
            }
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
