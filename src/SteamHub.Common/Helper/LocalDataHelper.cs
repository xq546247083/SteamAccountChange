using Newtonsoft.Json;
using SteamHub.Model;

namespace SteamHub.Helper
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

            var infoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataFileName);
            var jsonStr = JsonConvert.SerializeObject(localData);
            var encryptedBytes = EncryptionHelper.AesEncrypt(jsonStr);

            File.WriteAllBytes(infoFilePath, encryptedBytes);
        }
    }
}