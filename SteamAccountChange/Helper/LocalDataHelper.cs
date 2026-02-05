using Newtonsoft.Json;
using SteamAccountChange.Model;
using System;
using System.IO;
using System.Windows;

namespace SteamAccountChange.Helper
{
    /// <summary>
    /// 本地数据帮助类
    /// </summary>
    public static class LocalDataHelper
    {
        private const string DataFileName = "data.bin";

        /// <summary>
        /// 读取steam账号信息
        /// </summary>
        /// <returns></returns>
        public static LocalData GetLocalData()
        {
            try
            {
                var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataFileName);
                if (!File.Exists(infoFilePath))
                {
                    return new LocalData();
                }

                var fileBytes = File.ReadAllBytes(infoFilePath);
                var decryptedStr = EncryptionHelper.AesDecrypt(fileBytes);

                var saveInfo = JsonConvert.DeserializeObject<LocalData>(decryptedStr);
                return saveInfo ?? new LocalData();
            }
            catch (Exception)
            {
                return new LocalData();
            }
        }

        /// <summary>
        /// 保存保存信息
        /// </summary>
        /// <returns></returns>
        public static void Save(LocalData localData)
        {
            if (localData == null)
            {
                return;
            }

            try
            {
                var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataFileName);
                var jsonStr = JsonConvert.SerializeObject(localData);
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