using Microsoft.Win32;
using Newtonsoft.Json;
using SteamAccountChange.Model;
using System;
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
        /// 读取steam账号信息
        /// </summary>
        /// <returns></returns>
        public static SaveInfo GetSaveInfo()
        {
            try
            {
                // 创建文件
                var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SteamAccount.txt");
                if (!File.Exists(infoFilePath))
                {
                    return new SaveInfo();
                }

                // 读取记事本
                var steamAccountFileInfo = new FileInfo(infoFilePath);
                StreamReader streamReader = steamAccountFileInfo.OpenText();

                // 读取信息
                string strTotal = string.Empty;
                string strLine = string.Empty;
                while (!string.IsNullOrEmpty(strLine = streamReader.ReadLine()))
                {
                    strTotal = $"{strTotal}{strLine}";
                }
                streamReader.Dispose();

                // 序列化对象
                var saveInfo = JsonConvert.DeserializeObject<SaveInfo>(strTotal);
                if (saveInfo == null)
                {
                    return new SaveInfo();
                }

                return saveInfo;
            }
            catch (Exception)
            {
                return new SaveInfo();
            }
        }

        /// <summary>
        /// 保存保存信息
        /// </summary>
        /// <returns></returns>
        public static void SaveSaveInfo(SaveInfo saveInfo)
        {
            if (saveInfo == null)
            {
                return;
            }

            try
            {
                // 创建文件
                var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SteamAccount.txt");
                if (File.Exists(infoFilePath))
                {
                    File.Delete(infoFilePath);
                }

                // 创建新的文本
                var fileSteam = File.Create(infoFilePath);
                fileSteam.Dispose();

                // 追加信息
                var str = JsonConvert.SerializeObject(saveInfo);
                File.AppendAllText(infoFilePath, str);
            }
            catch (Exception)
            {
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
                SetSteamRegistry("AutoLoginUser", account);

                // 杀掉游戏进程

                // 杀掉steam进程
                KillProcess("stream");

                // 启动steam
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
