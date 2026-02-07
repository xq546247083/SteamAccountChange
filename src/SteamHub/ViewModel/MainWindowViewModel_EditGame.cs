using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Entities;
using SteamHub.Enums;
using SteamHub.Manager;
using SteamHub.Repositories;
using System.Collections.ObjectModel;
using System.IO;

namespace SteamHub.ViewModel
{
    public partial class MainWindowViewModel
    {
        private string editSteamGameAppId;

        #region 绑定属性

        /// <summary>
        /// 编辑steam游戏名称
        /// </summary>
        [ObservableProperty]
        private string editSteamGameName;

        /// <summary>
        /// 编辑steam游戏图标
        /// </summary>
        [ObservableProperty]
        private byte[] editSteamGameIcon;

        /// <summary>
        /// 可用游戏图标
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<byte[]> allGameIcons;

        #endregion

        #region 绑定方法

        /// <summary>
        /// 打开编辑游戏抽屉
        /// </summary>
        [RelayCommand]
        private void OpenEditSteamGameDrawer(SteamGame game)
        {
            if (game == null)
            {
                return;
            }

            EditSteamGameName = game.Name;
            EditSteamGameIcon = game.Icon;
            editSteamGameAppId = game.AppId;

            // 加载所有游戏列表
            var allSteamGameIcons = SteamAnalyzer.GetAllSteamGameIcons();
            if (EditSteamGameIcon != null)
            {
                allSteamGameIcons.Add(EditSteamGameIcon);
            }
            AllGameIcons = new ObservableCollection<byte[]>(allSteamGameIcons);
            EditType = EditType.SteamGame;
        }

        /// <summary>
        /// 关闭编辑游戏抽屉
        /// </summary>
        [RelayCommand]
        private void CloseEditSteamGameDrawer()
        {
            EditType = EditType.None;
        }

        /// <summary>
        /// 保存steam游戏信息
        /// </summary>
        [RelayCommand]
        private void SaveSteamGame()
        {
            if (string.IsNullOrEmpty(editSteamGameAppId))
            {
                EditType = EditType.None;
                return;
            }

            if (string.IsNullOrEmpty(EditSteamGameName))
            {
                return;
            }

            var game = SteamGameRepository.GetByAppId(editSteamGameAppId);
            if (game == null)
            {
                return;
            }

            game.Name = EditSteamGameName;
            game.Icon = EditSteamGameIcon;
            SteamGameRepository.UpdateList(new List<SteamGame>() { game });

            EditType = EditType.None;
            ReLoad();
            Lactor.ShowToolTip("保存成功！");
        }

        /// <summary>
        /// 选择游戏图标
        /// </summary>
        [RelayCommand]
        private void SelectGameIcon(byte[] icon)
        {
            if (icon != null)
            {
                EditSteamGameIcon = icon;
            }
        }

        #endregion
    }
}