using Newtonsoft.Json;
using SteamAccountChange.Model;
using System;
using System.IO;
using System.Windows;

namespace SteamAccountChange.Common
{
    /// <summary>
    /// steam帮助类
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 读取steam账号信息
        /// </summary>
        /// <returns></returns>
        public static SaveInfo GetConfig()
        {
            try
            {
                // 创建文件
                var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SteamAccount.txt");
                if (!File.Exists(infoFilePath))
                {
                    return new SaveInfo();
                }

                // 序列化对象
                var strTotal = File.ReadAllText(infoFilePath);
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
        public static void Save(SaveInfo saveInfo)
        {
            if (saveInfo == null)
            {
                return;
            }

            try
            {
                // 创建新的文本
                var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SteamAccount.txt");
                var str = JsonConvert.SerializeObject(saveInfo);
                File.WriteAllText(infoFilePath, str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}