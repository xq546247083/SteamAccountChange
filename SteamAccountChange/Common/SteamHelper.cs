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
            // 处理进程名
            var hendleProcessName = processName.ToLower();
            var strList = processName.Split('.');
            hendleProcessName = strList[0];

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
        public static void OpenSteam(string account)
        {
            try
            {
                // 设置注册信息
                RegistryHelper.Set(@"Software\Valve\Steam", "AutoLoginUser", account);

                // 杀掉游戏进程
                var saveInfo = ConfigHelper.GetConfig();
                foreach (var item in saveInfo.KillProcessList)
                {
                    KillProcess(item.Name);
                }

                // 杀掉steam进程
                KillProcess("steam");

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}