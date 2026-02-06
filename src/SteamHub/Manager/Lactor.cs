using SteamHub.ViewModel;

namespace SteamHub.Manager
{
    /// <summary>
    /// 加载器
    /// </summary>
    public static class Lactor
    {
        /// <summary>
        /// 对象锁
        /// </summary>
        private static object mainWindowLockObj = new object();

        /// <summary>
        /// 主窗口
        /// </summary>
        private static MainWindow mainWindow;

        /// <summary>
        /// 主窗口
        /// </summary>
        public static MainWindow MainWindow
        {
            get
            {
                if (mainWindow == null)
                {
                    lock (mainWindowLockObj)
                    {
                        if (mainWindow == null)
                        {
                            mainWindow = new MainWindow();
                        }
                    }
                }

                return mainWindow;
            }
        }

        /// <summary>
        /// 对象锁
        /// </summary>
        private static object mainWindowViewModelLockObj = new object();

        /// <summary>
        /// 主窗口ViewModel
        /// </summary>
        private static MainWindowViewModel mainWindowViewModel;

        /// <summary>
        /// 主窗口ViewModel
        /// </summary>
        public static MainWindowViewModel MainWindowViewModel
        {
            get
            {
                if (mainWindowViewModel == null)
                {
                    lock (mainWindowViewModelLockObj)
                    {
                        if (mainWindowViewModel == null)
                        {
                            mainWindowViewModel = new MainWindowViewModel();
                        }
                    }
                }

                return mainWindowViewModel;
            }
        }
    }
}
