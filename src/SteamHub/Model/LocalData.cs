using System.Collections.Generic;

namespace SteamHub.Model
{
    /// <summary>
    /// 本地数据
    /// </summary>
    public class LocalData
    {
        /// <summary>
        /// 游戏账号信息列表
        /// </summary>
        public List<SteamAccoutInfo> SteamAccoutInfoList
        {
            get; set;
        }

        /// <summary>
        /// 要杀掉的进程列表
        /// </summary>
        public List<ProcessInfo> KillProcessList
        {
            get; set;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public LocalData()
        {
            SteamAccoutInfoList = new List<SteamAccoutInfo>();
            KillProcessList = new List<ProcessInfo>();
        }
    }
}