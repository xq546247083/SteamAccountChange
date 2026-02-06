namespace SteamHub;

/// <summary>
/// 数据库初始化器
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// 初始化数据库（自动创建和迁移）
    /// </summary>
    public static void Initialize()
    {
        using var context = new SteamHubDbContext();

        // 确保数据库已创建
        context.Database.EnsureCreated();
    }
}