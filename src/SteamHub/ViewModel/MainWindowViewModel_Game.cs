using CommunityToolkit.Mvvm.ComponentModel;
using SteamHub.Entities;

namespace SteamHub.ViewModel
{
    /// <summary>
    /// 主界面的ViewModel
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        #region 绑定属性

        /// <summary>
        /// 游戏列表
        /// </summary>
        [ObservableProperty]
        private List<SteamGame> steamGames;

        #endregion
    }
}