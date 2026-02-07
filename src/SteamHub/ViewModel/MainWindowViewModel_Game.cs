using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Entities;
using SteamHub.Repositories;
using System.Collections.ObjectModel;

namespace SteamHub.ViewModel
{
    public partial class MainWindowViewModel
    {
        #region 绑定属性

        /// <summary>
        /// 游戏列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SteamGame> steamGames;

        /// <summary>
        /// 搜索游戏名称
        /// </summary>
        [ObservableProperty]
        private string searchSteamGameName;

        /// <summary>
        /// 过滤Steam账号
        /// </summary>
        [ObservableProperty]
        private SteamAccount searchSteamAccount;

        #endregion

        #region 属性变化引发的事件

        /// <summary>
        /// 搜索游戏名称改变
        /// </summary>
        /// <param name="value"></param>
        partial void OnSearchSteamGameNameChanged(string value)
        {
            LoadGames();
        }

        /// <summary>
        /// 过滤Steam账号改变
        /// </summary>
        /// <param name="value"></param>
        partial void OnSearchSteamAccountChanged(SteamAccount value)
        {
            LoadGames();
        }

        #endregion

        #region 绑定方法

        /// <summary>
        /// 打开Steam游戏
        /// </summary>
        /// <param name="game">游戏</param>
        [RelayCommand]
        private void OpenSteamGame(SteamGame game)
        {
            if (game == null)
            {
                return;
            }

            SteamTool.OpenGame(game.AppId);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 应用游戏过滤
        /// </summary>
        private void LoadGames()
        {
            var allSteamGames = SteamGameRepository.GetAll();
            if (allSteamGames == null)
            {
                SteamGames = new ObservableCollection<SteamGame>();
                return;
            }

            var query = allSteamGames.AsEnumerable();

            // 过滤账号
            if (SearchSteamAccount != null)
            {
                query = query.Where(g => g.AccountSteamId == SearchSteamAccount.SteamId);
            }

            // 过滤名称
            if (!string.IsNullOrWhiteSpace(SearchSteamGameName))
            {
                query = query.Where(g => g.Name != null && g.Name.Contains(SearchSteamGameName, StringComparison.OrdinalIgnoreCase));
            }

            SteamGames = new ObservableCollection<SteamGame>(query);
        }

        #endregion
    }
}