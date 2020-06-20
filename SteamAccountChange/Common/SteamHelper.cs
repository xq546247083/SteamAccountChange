using Microsoft.Win32;
using SteamAccountChange.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SteamAccountChange.Common
{
    /// <summary>
    /// steam帮助类
    /// </summary>
    public static class SteamHelper
    {
        /// <summary>
        /// 获取注册信息
        /// </summary>
        /// <returns></returns>
        private static string GetSteamExe()
        {
            var currentUser = Registry.CurrentUser;
            if (currentUser == null)
            {
                return string.Empty;
            }

            var software = currentUser.OpenSubKey("SOFTWARE", true);
            if (software == null)
            {
                return string.Empty;
            }

            var vavle = software.OpenSubKey("Valve", true);
            if (vavle == null)
            {
                return string.Empty;
            }

            var steam = vavle.OpenSubKey("Steam", true);
            if (steam == null)
            {
                return string.Empty;
            }

            return steam.GetValue("SteamExe").ToString();
        }

        /// <summary>
        /// 设置Steam注册表
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        private static void SetSteamRegistry(string key, string value)
        {
            var currentUser = Registry.CurrentUser;
            if (currentUser == null)
            {
                return;
            }

            var software = currentUser.OpenSubKey("SOFTWARE", true);
            if (software == null)
            {
                return;
            }

            var valve = software.OpenSubKey("Valve", true);
            if (valve == null)
            {
                return;
            }

            var steam = valve.OpenSubKey("Steam", true);
            if (steam == null)
            {
                return;
            }

            steam.SetValue(key, value);
        }

        /// <summary>
        /// 杀steam进程
        /// </summary>
        private static void KillSteamProcess()
        {
            var processList = Process.GetProcesses();
            foreach (Process item in processList)
            {
                if (item.ProcessName.ToLower() == "steam")
                {
                    item.Kill();
                }
            }
        }

        /// <summary>
        /// 读取steam账号信息
        /// </summary>
        /// <returns></returns>
        public static List<SteamAccoutInfo> GetSteamAccoutInfoList()
        {
            var result = new List<SteamAccoutInfo>();

            try
            {
                // 读取记事本
                var steamAccountFileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SteamAccount.txt"));
                StreamReader streamReader = steamAccountFileInfo.OpenText();

                // 读取账号
                string strLine = string.Empty;
                while (!string.IsNullOrEmpty(strLine = streamReader.ReadLine()))
                {
                    var strList = strLine.Split(' ');
                    var steamAccount = new SteamAccoutInfo()
                    {
                        Name = strList[0],
                        Account = strList.Length >= 2 ? strList[1] : strList[0],
                    };

                    result.Add(steamAccount);
                }
                streamReader.Dispose();
            }
            catch (Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// 打开steam
        /// </summary>
        /// <param name="account">账号</param>
        public static void OpenSteam(string account)
        {
            try
            {
                SetSteamRegistry("AutoLoginUser", account);

                KillSteamProcess();
                string steamExe = GetSteamExe();
                Process.Start(steamExe);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
