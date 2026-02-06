using System.Diagnostics;

namespace SteamHub.Helper
{
    /// <summary>
    /// 进程帮助类
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// 杀steam进程
        /// </summary>
        public static void Kill(string processName)
        {
            var hendleProcessName = processName.ToLower().Split('.')[0];
            var processList = Process.GetProcesses();
            foreach (Process item in processList)
            {
                if (item.ProcessName.ToLower() == hendleProcessName)
                {
                    item.Kill();
                }
            }
        }
    }
}