using SteamAccountChange.Manager;
using System.Diagnostics;
using System.Windows;

namespace SteamAccountChange
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 应用启动
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 如果存在，直接返回
            if (ExistsCurrentProcess())
            {
                System.Environment.Exit(0);
            }

            NotifyIconManager.Init();
        }

        /// <summary>
        /// 是否存在当前进程
        /// </summary>
        private static bool ExistsCurrentProcess()
        {
            var currentProcess = Process.GetCurrentProcess();

            var processList = Process.GetProcesses();
            foreach (Process item in processList)
            {
                if (item.ProcessName.ToLower() == currentProcess.ProcessName.ToLower() && item.Id != currentProcess.Id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}