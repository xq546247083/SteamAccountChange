using Newtonsoft.Json;
using SteamHub.Entities;

namespace SteamHub.Repositories
{
    /// <summary>
    /// 设置仓储
    /// </summary>
    public static class SettingRepository
    {
        /// <summary>
        /// 获取配置值
        /// </summary>
        public static string? GetValue(string key)
        {
            using var context = new SteamHubDbContext();
            var config = context.Settings.FirstOrDefault(c => c.Key == key);
            return config?.Value;
        }

        /// <summary>
        /// 设置配置值
        /// </summary>
        public static void SetValue(string key, string value)
        {
            using var context = new SteamHubDbContext();
            var config = context.Settings.FirstOrDefault(c => c.Key == key);

            if (config != null)
            {
                config.Value = value;
            }
            else
            {
                config = new Setting
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Value = value
                };
                context.Settings.Add(config);
            }

            context.SaveChanges();
        }

        #region 设置进程

        /// <summary>
        /// 获取进程列表
        /// </summary>
        public static List<string> GetKillProcessList()
        {
            var json = GetValue("killprocess");
            if (string.IsNullOrEmpty(json))
            {
                return new List<string>();
            }

            try
            {
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 设置进程列表
        /// </summary>
        public static void SetKillProcessList(List<string> processes)
        {
            var json = JsonConvert.SerializeObject(processes);
            SetValue("killprocess", json);
        }

        /// <summary>
        /// 添加进程到列表
        /// </summary>
        public static void AddKillProcess(string processName)
        {
            var processes = GetKillProcessList();
            if (!processes.Contains(processName))
            {
                processes.Insert(0, processName);
                SetKillProcessList(processes);
            }
        }

        /// <summary>
        /// 从列表中删除进程
        /// </summary>
        public static void DeleteKillProcess(string processName)
        {
            var processes = GetKillProcessList();
            processes.Remove(processName);
            SetKillProcessList(processes);
        }

        #endregion
    }
}