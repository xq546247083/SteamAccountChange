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
        private static void Kill(string processName)
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

        /// <summary>
        /// 打开steam
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="killProcessList">要杀掉的进程列表</param>
        public static void OpenSteam(string account, List<string> killProcessList = null)
        {
            // 设置注册信息
            RegistryHelper.Set(@"Software\Valve\Steam", "AutoLoginUser", account);

            // 杀掉游戏进程
            if (killProcessList != null)
            {
                foreach (var processName in killProcessList)
                {
                    Kill(processName);
                }
            }

            // 杀掉steam进程
            Kill("steam");

            // 启动steam
            var (getSuccess, steamExeObj) = RegistryHelper.Get(@"Software\Valve\Steam", "SteamExe");
            if (getSuccess == false || steamExeObj == null)
            {
                return;
            }

            var steamExe = steamExeObj.ToString();
            if (string.IsNullOrEmpty(steamExe))
            {
                return;
            }

            Process.Start(steamExe);
        }
    }
}