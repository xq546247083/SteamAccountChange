using Newtonsoft.Json;
using SteamAccountChange.Model;
using System;
using System.IO;
using System.Windows;

namespace SteamAccountChange.Common
{
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public static class ConfigHelper
    {
        private const string ConfigFileName = "data.bin";

        /// <summary>
        /// 读取steam账号信息
        /// </summary>
        /// <returns></returns>
        public static SaveInfo GetConfig()
        {
            try
            {
                var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
                if (!File.Exists(infoFilePath))
                {
                    return new SaveInfo();
                }

                var fileBytes = File.ReadAllBytes(infoFilePath);
                var decryptedStr = EncryptionHelper.AesDecrypt(fileBytes);
                
                var saveInfo = JsonConvert.DeserializeObject<SaveInfo>(decryptedStr);
                return saveInfo ?? new SaveInfo();
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
                var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
                var jsonStr = JsonConvert.SerializeObject(saveInfo);
                var encryptedBytes = EncryptionHelper.AesEncrypt(jsonStr);
                
                File.WriteAllBytes(infoFilePath, encryptedBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}