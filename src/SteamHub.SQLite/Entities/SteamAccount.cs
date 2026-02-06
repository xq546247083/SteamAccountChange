namespace SteamHub.Entities;

/// <summary>
/// Steam 账号实体
/// </summary>
public class SteamAccount
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 账号（唯一）
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Order { get; set; }
}