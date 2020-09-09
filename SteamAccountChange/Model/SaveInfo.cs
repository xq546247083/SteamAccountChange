using System.Collections.Generic;

namespace SteamAccountChange.Model
{
    /// <summary>
    /// 保存的消息
    /// </summary>
    public class SaveInfo
    {
        /// <summary>
        /// 游戏账号信息列表
        /// </summary>
        public List<SteamAccoutInfo> SteamAccoutInfoList { get; set; }

        /// <summary>
        /// 游戏进程列表
        /// </summary>
        public List<GameProcessInfo> GameProcessList { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public SaveInfo()
        {
            SteamAccoutInfoList = new List<SteamAccoutInfo>();
            GameProcessList = new List<GameProcessInfo>();
        }
    }
}
