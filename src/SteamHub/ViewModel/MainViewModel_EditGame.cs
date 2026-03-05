using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Entities;
using SteamHub.Enums;
using SteamHub.Manager;
using SteamHub.Repositories;
using System.Collections.ObjectModel;

namespace SteamHub.ViewModel
{
    public partial class MainViewModel
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

            // 加载所有游戏图标列表
            var allSteamGameIcons = SteamAnalyzer.GetAllGameIcons();
            if (EditSteamGameIcon != null)
            {
                var editSteamGameIconStr = BitConverter.ToString(EditSteamGameIcon);
                for (var i = 0; i < allSteamGameIcons.Count; i++)
                {
                    if (BitConverter.ToString(allSteamGameIcons[i]) == editSteamGameIconStr)
                    {
                        allSteamGameIcons.Remove(allSteamGameIcons[i]);
                    }
                }
                allSteamGameIcons.Insert(0, EditSteamGameIcon);
            }
            AllGameIcons = new ObservableCollection<byte[]>(allSteamGameIcons);
            DrawerType = DrawerType.SteamGame;
        }

        /// <summary>
        /// 关闭编辑游戏抽屉
        /// </summary>
        [RelayCommand]
        private void CloseEditSteamGameDrawer()
        {
            DrawerType = DrawerType.None;
        }

        /// <summary>
        /// 删除steam游戏信息
        /// </summary>
        [RelayCommand]
        private void DeleteSteamGame()
        {
            if (string.IsNullOrEmpty(editSteamGameAppId))
            {
                DrawerType = DrawerType.None;
                return;
            }

            SteamGameRepository.Delete(editSteamGameAppId);

            DrawerType = DrawerType.None;
            Lactor.ReLoad();
            Lactor.ShowToolTip("保存成功！");
        }

        /// <summary>
        /// 保存steam游戏信息
        /// </summary>
        [RelayCommand]
        private void SaveSteamGame()
        {
            if (string.IsNullOrEmpty(editSteamGameAppId))
            {
                DrawerType = DrawerType.None;
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

            DrawerType = DrawerType.None;
            Lactor.ReLoad();
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