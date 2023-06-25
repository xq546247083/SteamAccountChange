using Microsoft.Win32;

namespace SteamAccountChange.Common
{
    /// <summary>
    /// 注册表帮助类
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 获取Steam进程信息
        /// </summary>
        /// <returns></returns>
        public static string GetSteamExe()
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
        /// 获取Steam注册表信息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public static string GetSteamRegistry(string key)
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

            var valve = software.OpenSubKey("Valve", true);
            if (valve == null)
            {
                return string.Empty;
            }

            var steam = valve.OpenSubKey("Steam", true);
            if (steam == null)
            {
                return string.Empty;
            }

            return steam.GetValue(key).ToString();
        }

        /// <summary>
        /// 设置Steam注册表信息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public static void SetSteamRegistry(string key, string value)
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
    }
}
