using SteamHub.Model;
using System.Text.RegularExpressions;

namespace SteamHub;

/// <summary>
/// Steam数据分析器,用于从本地文件解析 Steam 账号和游戏数据
/// </summary>
public static class SteamAnalyzer
{
    #region 公共方法

    /// <summary>
    /// 获取登录用户列表
    /// </summary>
    /// <returns></returns>
    public static List<SteamAccountSource> GetAllLoginUsers()
    {
        var accounts = new List<SteamAccountSource>();

        var steamPath = SteamTool.GetSteamPath();
        var loginUsersPath = Path.Combine(steamPath, "config", "loginusers.vdf");
        if (!File.Exists(loginUsersPath))
        {
            return accounts;
        }

        var vdfContent = File.ReadAllText(loginUsersPath);

        // 使用正则表达式提取每个用户块
        var userPattern = @"""(\d{17})""[\s\S]*?{([\s\S]*?)}";
        var matches = Regex.Matches(vdfContent, userPattern);

        foreach (Match match in matches)
        {
            var steamId = match.Groups[1].Value;
            var userBlock = match.Groups[2].Value;

            var account = new SteamAccountSource
            {
                SteamId = steamId,
                AccountName = ExtractValue(userBlock, "AccountName"),
                PersonaName = ExtractValue(userBlock, "PersonaName"),
                Icon = LoadAccountAvatar(steamPath, steamId)
            };

            if (!string.IsNullOrEmpty(account.AccountName))
            {
                accounts.Add(account);
            }
        }

        return accounts;
    }

    /// <summary>
    /// 解析所有游戏清单文件,获取游戏信息
    /// </summary>
    /// <param name="steamPath">Steam 安装路径</param>
    /// <returns>游戏列表</returns>
    public static List<SteamGameSource> GetAllGames()
    {
        var games = new List<SteamGameSource>();

        var steamPath = SteamTool.GetSteamPath();
        var steamAppsPath = Path.Combine(steamPath, "steamapps");
        if (!Directory.Exists(steamAppsPath))
        {
            return games;
        }

        var manifestFiles = Directory.GetFiles(steamAppsPath, "appmanifest_*.acf");
        foreach (var manifestFile in manifestFiles)
        {
            var game = LoadSteamGameSource(manifestFile);
            if (game != null && !string.IsNullOrEmpty(game.AppId))
            {
                // 加载游戏图标
                game.Icon = LoadGameIcon(steamPath, game.AppId);
                games.Add(game);
            }
        }

        return games;
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 解析单个 appmanifest_*.acf 文件
    /// </summary>
    /// <param name="manifestPath">清单文件路径</param>
    /// <returns>游戏信息</returns>
    private static SteamGameSource LoadSteamGameSource(string manifestPath)
    {
        if (!File.Exists(manifestPath))
        {
            return null;
        }

        var vdfContent = File.ReadAllText(manifestPath);
        return new SteamGameSource
        {
            AppId = ExtractValue(vdfContent, "appid"),
            Name = ExtractValue(vdfContent, "name"),
            LastOwnerSteamId = ExtractValue(vdfContent, "LastOwner")
        };
    }

    /// <summary>
    /// 加载用户头像
    /// </summary>
    /// <param name="steamPath">Steam 安装路径</param>
    /// <param name="steamId">Steam64 ID</param>
    /// <returns>头像数据</returns>
    private static byte[] LoadAccountAvatar(string steamPath, string steamId)
    {
        var avatarPath = Path.Combine(steamPath, "config", "avatarcache", $"{steamId}.png");
        if (File.Exists(avatarPath))
        {
            return File.ReadAllBytes(avatarPath);
        }

        return null;
    }

    /// <summary>
    /// 加载游戏图标
    /// </summary>
    /// <param name="steamPath">Steam 安装路径</param>
    /// <param name="appId">游戏 ID</param>
    /// <returns>图标数据</returns>
    private static byte[] LoadGameIcon(string steamPath, string appId)
    {
        var iconPath = Path.Combine(steamPath, "steam", "games", $"{appId}.ico");
        if (File.Exists(iconPath))
        {
            return File.ReadAllBytes(iconPath);
        }

        return null;
    }

    /// <summary>
    /// 从 VDF 内容中提取指定键的值
    /// </summary>
    private static string ExtractValue(string vdfContent, string key)
    {
        var pattern = $@"""{key}""\s+""([^""]*)""";
        var match = Regex.Match(vdfContent, pattern, RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    #endregion
}