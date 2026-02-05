using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SteamAccountChange.Helper
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
                var localData = LocalDataHelper.GetLocalData();
                foreach (var item in localData.KillProcessList)
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
        /// <summary>
        /// 删除Steam账号相关的本地配置
        /// </summary>
        /// <param name="account">账号</param>
        public static void DeleteSteamAccount(string account)
        {
            try
            {
                // 1. 检查注册表中的 AutoLoginUser 是否为待删除账号
                var (success, autoLoginUserObj) = RegistryHelper.Get(@"Software\Valve\Steam", "AutoLoginUser");
                if (success && autoLoginUserObj != null)
                {
                    var currentAutoLoginUser = autoLoginUserObj.ToString();
                    if (string.Equals(currentAutoLoginUser, account, StringComparison.OrdinalIgnoreCase))
                    {
                        // 清空自动登录用户
                        RegistryHelper.Set(@"Software\Valve\Steam", "AutoLoginUser", "");
                        RegistryHelper.Set(@"Software\Valve\Steam", "RememberPassword", "0");

                        // 2. 删除 loginusers.vdf
                        var (pathSuccess, steamPathObj) = RegistryHelper.Get(@"Software\Valve\Steam", "SteamPath");
                        if (pathSuccess && steamPathObj != null)
                        {
                            var configPath = Path.Combine(steamPathObj.ToString(), "config", "loginusers.vdf");
                            if (File.Exists(configPath))
                            {
                                File.Delete(configPath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"清理Steam数据失败: {ex.Message}");
            }
        }
    }
}