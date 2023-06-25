using System;
using System.Diagnostics;
using System.Windows;

namespace SteamAccountChange.Common
{
    /// <summary>
    /// steam帮助类
    /// </summary>
    public static class SteamHelper
    {
        /// <summary>
        /// 杀steam进程
        /// </summary>
        private static void KillProcess(string processName)
        {
            var processList = Process.GetProcesses();
            foreach (Process item in processList)
            {
                if (item.ProcessName.ToLower() == processName)
                {
                    item.Kill();
                }
            }
        }

        /// <summary>
        /// 打开steam
        /// </summary>
        /// <param name="account">账号</param>
        public static void OpenSteam(string account)
        {
            try
            {
                // 设置注册信息
                RegistryHelper.SetSteamRegistry("AutoLoginUser", account);

                // 杀掉游戏进程
                var saveInfo = ConfigHelper.GetConfig();
                foreach (var item in saveInfo.GameProcessList)
                {
                    KillProcess(item.Name);
                }

                // 杀掉steam进程
                KillProcess("steam");

                // 启动steam
                string steamExe = RegistryHelper.GetSteamExe();
                Process.Start(steamExe);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}