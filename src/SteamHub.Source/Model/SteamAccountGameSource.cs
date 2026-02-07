namespace SteamHub.Model
{
    /// <summary>
    /// 账号玩游戏信息
    /// </summary>
    public class SteamAccountPlayGameSource
    {
        /// <summary>
        /// SteamId
        /// </summary>
        public string SteamId { get; set; }

        /// <summary>
        /// SteamId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 上次游玩时间
        /// </summary>
        public long LastPlayed { get; set; }

        public SteamAccountPlayGameSource()
        {
        }

        public SteamAccountPlayGameSource(string steamId, string appId, long lastPlayed)
        {
            SteamId = steamId;
            AppId = appId;
            LastPlayed = lastPlayed;
        }
    }
}