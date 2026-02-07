using SteamHub.Entities;

namespace SteamHub.Repositories
{
    /// <summary>
    /// Steam游戏仓储
    /// </summary>
    public static class SteamGameRepository
    {
        /// <summary>
        /// 获取所有游戏
        /// </summary>
        public static List<SteamGame> GetAll()
        {
            using var context = new SteamHubDbContext();
            return context.SteamGames
                .OrderBy(g => g.Name)
                .ToList();
        }

        /// <summary>
        /// 根据 SteamId 获取游戏列表
        /// </summary>
        public static List<SteamGame> GetBySteamId(string accountSteamId)
        {
            using var context = new SteamHubDbContext();
            return context.SteamGames
                .Where(g => g.AccountSteamId == accountSteamId)
                .OrderBy(g => g.Name)
                .ToList();
        }

        /// <summary>
        /// 根据 AppId 获取游戏
        /// </summary>
        public static SteamGame GetByAppId(string appId)
        {
            using var context = new SteamHubDbContext();
            return context.SteamGames.FirstOrDefault(g => g.AppId == appId);
        }

        /// <summary>
        /// 添加或更新游戏
        /// </summary>
        public static void AddOrUpdate(SteamGame game)
        {
            using var context = new SteamHubDbContext();
            var existing = context.SteamGames.FirstOrDefault(g => g.AppId == game.AppId);
            
            if (existing != null)
            {
                // 更新现有游戏
                existing.Name = game.Name;
                existing.Icon = game.Icon;
                existing.AccountSteamId = game.AccountSteamId;
            }
            else
            {
                // 添加新游戏
                if (game.Id == Guid.Empty)
                {
                    game.Id = Guid.NewGuid();
                }
                context.SteamGames.Add(game);
            }
            
            context.SaveChanges();
        }

        /// <summary>
        /// 批量添加或更新游戏
        /// </summary>
        public static void AddOrUpdateRange(List<SteamGame> games)
        {
            using var context = new SteamHubDbContext();
            
            foreach (var game in games)
            {
                var existing = context.SteamGames.FirstOrDefault(g => g.AppId == game.AppId);
                
                if (existing != null)
                {
                    existing.AccountSteamId = game.AccountSteamId;
                }
                else
                {
                    if (game.Id == Guid.Empty)
                    {
                        game.Id = Guid.NewGuid();
                    }
                    context.SteamGames.Add(game);
                }
            }
            
            context.SaveChanges();
        }

        /// <summary>
        /// 删除游戏
        /// </summary>
        public static void Delete(string appId)
        {
            using var context = new SteamHubDbContext();
            var entity = context.SteamGames.FirstOrDefault(g => g.AppId == appId);
            if (entity != null)
            {
                context.SteamGames.Remove(entity);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 检查游戏是否存在
        /// </summary>
        public static bool Exists(string appId)
        {
            using var context = new SteamHubDbContext();
            return context.SteamGames.Any(g => g.AppId == appId);
        }

        /// <summary>
        /// 清空所有游戏
        /// </summary>
        public static void Clear()
        {
            using var context = new SteamHubDbContext();
            context.SteamGames.RemoveRange(context.SteamGames);
            context.SaveChanges();
        }
    }
}