using SteamHub.Entities;

namespace SteamHub.Repositories
{
    /// <summary>
    /// Steam账号仓储
    /// </summary>
    public static class SteamAccountRepository
    {
        /// <summary>
        /// 获取所有账号
        /// </summary>
        public static List<SteamAccount> GetAll()
        {
            using var context = new SteamHubDbContext();
            return context.SteamAccounts
                .OrderBy(a => a.Order)
                .ThenBy(a => a.Account)
                .ToList();
        }

        /// <summary>
        /// 根据账号名获取账号
        /// </summary>
        public static SteamAccount? GetByAccount(string account)
        {
            using var context = new SteamHubDbContext();
            return context.SteamAccounts.FirstOrDefault(a => a.Account == account);
        }

        /// <summary>
        /// 添加账号
        /// </summary>
        public static void Add(SteamAccount account)
        {
            using var context = new SteamHubDbContext();
            if (account.Id == Guid.Empty)
            {
                account.Id = Guid.NewGuid();
            }
            context.SteamAccounts.Add(account);
            context.SaveChanges();
        }

        /// <summary>
        /// 更新账号
        /// </summary>
        public static void Update(SteamAccount account)
        {
            using var context = new SteamHubDbContext();
            var existing = context.SteamAccounts.FirstOrDefault(a => a.Account == account.Account);
            if (existing != null)
            {
                existing.Name = account.Name;
                existing.Password = account.Password;
                existing.Order = account.Order;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 删除账号
        /// </summary>
        public static void Delete(string account)
        {
            using var context = new SteamHubDbContext();
            var entity = context.SteamAccounts.FirstOrDefault(a => a.Account == account);
            if (entity != null)
            {
                context.SteamAccounts.Remove(entity);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 检查账号是否存在
        /// </summary>
        public static bool Exists(string account)
        {
            using var context = new SteamHubDbContext();
            return context.SteamAccounts.Any(a => a.Account == account);
        }
    }
}