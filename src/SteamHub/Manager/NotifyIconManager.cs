using SteamHub.Helper;
using SteamHub.Repositories;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SteamHub.Manager
{
    /// <summary>
    /// 通知管理
    /// </summary>
    public static class NotifyIconManager
    {
        #region Win32 API

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        #endregion

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
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SteamHub.Resources.app.ico"))
            {
                notifyIcon.Icon = new Icon(stream);
            }
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
            var steamAccounts = SteamAccountRepository.GetAll();
            foreach (var item in steamAccounts)
            {
                var steamAccountMenu = new ToolStripMenuItem(item.Name);
                steamAccountMenu.Tag = item.Account;
                steamAccountMenu.Click += SteamAccountMenu_Click;

                contextMenuStrip.Items.Add(steamAccountMenu);
            }

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            // 登录新账号
            var newAccountMenu = new ToolStripMenuItem("登录新账号");
            newAccountMenu.Click += NewAccountMenu_Click;
            contextMenuStrip.Items.Add(newAccountMenu);

            // 计划任务
            var launchOnSysPowerOnByTaskSchedulerMenu = new ToolStripMenuItem("计划任务");
            launchOnSysPowerOnByTaskSchedulerMenu.Checked = GetIsLaunchOnSysPowerOnByTaskScheduler();
            launchOnSysPowerOnByTaskSchedulerMenu.Click += LaunchOnSysPowerOnByTaskScheduler_Click;

            // 开机自启用菜单项
            var launchOnSysPowerOnMenu = new ToolStripMenuItem("开机自启动");
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
        /// 获取是否计划任务自启动
        /// </summary>
        /// <returns></returns>
        private static bool GetIsLaunchOnSysPowerOnByTaskScheduler()
        {
            var taslScheduler = TaskSchedulerHelper.Get(AppGlobal.AppName);
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
            var processList = SettingRepository.GetKillProcessList();
            SteamHelper.OpenSteam(Guid.NewGuid().ToString(), processList);
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

            var processList = SettingRepository.GetKillProcessList();
            SteamHelper.OpenSteam(steamAccountMenu.Tag.ToString(), processList);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private static void Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Environment.Exit(0);
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
                if (TaskSchedulerHelper.AddLuanchTask(AppGlobal.AppName, System.Windows.Forms.Application.ExecutablePath))
                {
                    launchOnSysPowerOnByTaskSchedulerMenu.Checked = currentValue;
                }
            }
            else
            {
                TaskSchedulerHelper.Del(AppGlobal.AppName);
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
            var window = Lactor.MainWindow;

            // 显示窗口
            window.Show();
            window.ShowInTaskbar = true;

            // 恢复窗口状态
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }

            // 使用 Win32 API 强制窗口显示在前面
            var helper = new WindowInteropHelper(window);
            var hwnd = helper.Handle;

            ShowWindow(hwnd, SW_RESTORE);
            SetForegroundWindow(hwnd);

            // WPF 层面的激活
            window.Activate();
            window.Topmost = true;
            window.Topmost = false;
        }

        #endregion
    }
}
