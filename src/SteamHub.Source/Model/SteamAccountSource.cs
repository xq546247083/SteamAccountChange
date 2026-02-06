namespace SteamHub.Model
{
    /// <summary>
    /// Steam 账号数据源模型
    /// </summary>
    public class SteamAccountSource
    {
        /// <summary>
        /// SteamId
        /// </summary>
        public string SteamId { get; set; }

        /// <summary>
        /// 账号名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string PersonaName { get; set; }

        /// <summary>
        /// 头像数据
        /// </summary>
        public byte[] Icon { get; set; }
    }
}