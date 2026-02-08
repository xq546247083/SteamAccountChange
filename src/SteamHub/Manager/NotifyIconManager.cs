using Hardcodet.Wpf.TaskbarNotification;
using System.Reflection;

namespace SteamHub.Manager
{
    /// <summary>
    /// 通知管理
    /// </summary>
    public static class NotifyIconManager
    {
        #region 私有属性

        /// <summary>
        /// 系统托盘
        /// </summary>
        private static TaskbarIcon taskbarIcon;

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            taskbarIcon = new TaskbarIcon();
            taskbarIcon.ToolTipText = "Steam账号切换器";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SteamHub.Resources.app.ico"))
            {
                taskbarIcon.Icon = new Icon(stream);
            }
            taskbarIcon.TrayPopup = Lactor.TrayPopupControl;
            taskbarIcon.TrayMouseDoubleClick += TaskbarIcon_TrayMouseDoubleClick;
            taskbarIcon.TrayRightMouseUp += TaskbarIcon_TrayRightMouseUp;
        }

        /// <summary>
        /// 双击系统推盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TaskbarIcon_TrayMouseDoubleClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Lactor.OpenMainWindow();
        }

        /// <summary>
        /// 右键点击系统托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TaskbarIcon_TrayRightMouseUp(object sender, System.Windows.RoutedEventArgs e)
        {
            taskbarIcon.ShowTrayPopup();
        }
    }
}