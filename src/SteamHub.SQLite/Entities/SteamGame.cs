namespace SteamHub.Entities;

/// <summary>
/// Steam 游戏实体
/// </summary>
public class SteamGame
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Steam 游戏 ID
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 游戏名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 游戏图标数据
    /// </summary>
    public byte[] Icon { get; set; }

    /// <summary>
    /// 所属账号
    /// </summary>
    public string SteamId { get; set; }
}