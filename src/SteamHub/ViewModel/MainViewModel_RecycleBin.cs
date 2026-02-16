using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Entities;
using SteamHub.Enums;
using SteamHub.Manager;
using SteamHub.Repositories;
using System.Collections.ObjectModel;

namespace SteamHub.ViewModel
{
    /// <summary>
    /// 主界面的ViewModel
    /// </summary>
    public partial class MainViewModel
    {
        #region 绑定属性

        /// <summary>
        /// 已删除的账号列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SteamAccount> deletedSteamAccounts;

        /// <summary>
        /// 已删除的游戏列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SteamGame> deletedSteamGames;

        #endregion

        #region 绑定方法

        /// <summary>
        /// 打开回收站
        /// </summary>
        [RelayCommand]
        private void OpenRecycleBinDrawer(string recycleBinType)
        {
            EditType = string.Equals(recycleBinType, "Game", StringComparison.OrdinalIgnoreCase) ? EditType.SteamGameRecycleBin : EditType.SteamAccountRecycleBin;
        }

        /// <summary>
        /// 关闭回收站
        /// </summary>
        [RelayCommand]
        private void CloseRecycleBinDrawer()
        {
            EditType = EditType.None;
        }

        /// <summary>
        /// 恢复回收站
        /// </summary>
        [RelayCommand]
        private void RestoreRecycleBin()
        {
            if (EditType == EditType.SteamGameRecycleBin)
            {
                SteamGameRepository.RestoreRecycleBin();
            }
            else
            {
                SteamAccountRepository.RestoreRecycleBin();
            }

            EditType = EditType.None;
            Lactor.ReLoad();
            Lactor.ShowToolTip("恢复成功！");
        }

        /// <summary>
        /// 清空回收站
        /// </summary>
        [RelayCommand]
        private void EmptyRecycleBin()
        {
            if (EditType == EditType.SteamGameRecycleBin)
            {
                SteamGameRepository.EmptyRecycleBin();
            }
            else
            {
                SteamAccountRepository.EmptyRecycleBin();
            }

            EditType = EditType.None;
            Lactor.ReLoad();
            Lactor.ShowToolTip("清空成功！");
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 加载已删除数据
        /// </summary>
        private void LoadDeletedData()
        {
            DeletedSteamGames = new ObservableCollection<SteamGame>(SteamGameRepository.GetListOfDeleted());
            DeletedSteamAccounts = new ObservableCollection<SteamAccount>(SteamAccountRepository.GetListOfDeleted());
        }

        #endregion
    }
}