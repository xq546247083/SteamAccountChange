namespace SteamAccountChange.Common
{
    /// <summary>
    /// 加载器
    /// </summary>
    public static class Lactor
    {
        /// <summary>
        /// 主窗口
        /// </summary>
        private static MainWindow mainWindow;

        /// <summary>
        /// 对象锁
        /// </summary>
        private static object mainWindowLockObj = new object();

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
    }
}
