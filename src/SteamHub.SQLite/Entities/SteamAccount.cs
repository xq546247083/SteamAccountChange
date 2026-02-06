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
    /// SteamId
    /// </summary>
    public string SteamId { get; set; }

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
    public string Order { get; set; }

    /// <summary>
    /// 头像数据
    /// </summary>
    public byte[] Icon { get; set; }

    public SteamAccount()
    {
    }

    public SteamAccount(Guid id, string steamId, string account, string name, string password, string order, byte[] icon)
    {
        Id = id;
        SteamId = steamId;
        Account = account;
        Name = name;
        Password = password;
        Order = order;
        Icon = icon;
    }
}