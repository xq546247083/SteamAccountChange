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
    public static List<SteamAccountSource> GetAllLoginAccounts()
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

        var steamAccountPlayGames = LoadSteamAccountPlayGames();
        var manifestFiles = Directory.GetFiles(steamAppsPath, "appmanifest_*.acf");
        foreach (var manifestFile in manifestFiles)
        {
            var game = LoadSteamGameSource(manifestFile);
            if (game != null && !string.IsNullOrEmpty(game.AppId))
            {
                // 加载游戏用户
                game.AccountSteamId = steamAccountPlayGames.LastOrDefault(r => r.AppId == game.AppId)?.SteamId ?? string.Empty;
                games.Add(game);
            }
        }

        return games;
    }

    /// <summary>
    /// 获取所有的游戏图标
    /// </summary>
    public static HashSet<byte[]> GetAllSteamGameIcons()
    {
        var result = new HashSet<byte[]>();

        var steamPath = SteamTool.GetSteamPath();
        if (string.IsNullOrEmpty(steamPath))
        {
            return result;
        }

        // 获取图标路径列表
        var gameIconPaths = new List<string>();
        var steamGamesPath = Path.Combine(steamPath, "steam", "games");
        if (Directory.Exists(steamGamesPath))
        {
            gameIconPaths.AddRange(Directory.GetFiles(steamGamesPath, "*.ico"));
        }

        // 读取图标
        foreach (var iconPath in gameIconPaths)
        {
            try
            {
                var iconBytes = File.ReadAllBytes(iconPath);
                result.Add(iconBytes);
            }
            catch
            {
            }
        }

        return result;
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 加载用户玩的游戏汇总信息
    /// </summary>
    /// <returns></returns>
    private static List<SteamAccountPlayGameSource> LoadSteamAccountPlayGames()
    {
        var result = new List<SteamAccountPlayGameSource>();

        var steamPath = SteamTool.GetSteamPath();
        var userDataPath = Path.Combine(steamPath, "userdata");
        if (!Directory.Exists(userDataPath))
        {
            return result;
        }

        var userDirs = Directory.GetDirectories(userDataPath);
        foreach (var userDir in userDirs)
        {
            var steamId3Str = new DirectoryInfo(userDir).Name;
            if (long.TryParse(steamId3Str, out var steamId3))
            {
                var steamId64 = (steamId3 + 76561197960265728).ToString();
                var localConfigPath = Path.Combine(userDir, "config", "localconfig.vdf");

                if (File.Exists(localConfigPath))
                {
                    var singleSteamAccountPlayGames = LoadSingleSteamAccountPlayGames(steamId64, localConfigPath);
                    if (singleSteamAccountPlayGames != null && singleSteamAccountPlayGames.Count > 0)
                    {
                        result.AddRange(singleSteamAccountPlayGames);
                    }
                }
            }
        }

        return result.OrderBy(r => r.LastPlayed).ToList();
    }

    /// <summary>
    /// 解析 localconfig.vdf 获取用户应用列表
    /// </summary>
    private static List<SteamAccountPlayGameSource> LoadSingleSteamAccountPlayGames(string steamId, string localConfigPath)
    {
        var result = new List<SteamAccountPlayGameSource>();
        try
        {
            var content = File.ReadAllText(localConfigPath);
            // 匹配 "apps" 块 (使用平衡组处理嵌套的大括号)
            var appsBlockMatch = Regex.Match(content, @"""apps""\s*\{((?>[^{}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))\}", RegexOptions.IgnoreCase);
            if (appsBlockMatch.Success)
            {
                var appsBlock = appsBlockMatch.Groups[1].Value;
                // 匹配每个 AppId 块: "AppId" { ... }
                var appPattern = @"""(\d+)""\s*\{([\s\S]*?)\}";
                var matches = Regex.Matches(appsBlock, appPattern);

                foreach (Match match in matches)
                {
                    var appId = match.Groups[1].Value;
                    var appContent = match.Groups[2].Value;

                    var lastPlayedMatch = Regex.Match(appContent, @"""LastPlayed""\s+""(\d+)""", RegexOptions.IgnoreCase);
                    long lastPlayed = 0;
                    if (lastPlayedMatch.Success)
                    {
                        long.TryParse(lastPlayedMatch.Groups[1].Value, out lastPlayed);
                    }

                    if (!result.Any(x => x.AppId == appId))
                    {
                        result.Add(new SteamAccountPlayGameSource(steamId, appId, lastPlayed));
                    }
                }
            }
        }
        catch
        {
        }

        return result;
    }

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
            AccountSteamId = ExtractValue(vdfContent, "LastOwner")
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