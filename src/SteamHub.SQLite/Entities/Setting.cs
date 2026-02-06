namespace SteamHub.Entities;

/// <summary>
/// 配置实体
/// </summary>
public class Setting
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 配置键（唯一）
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 配置值（JSON 格式）
    /// </summary>
    public string Value { get; set; }
}