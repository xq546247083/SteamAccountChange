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
        #region 绑定属性

        /// <summary>
        /// 编辑steam账号的账号
        /// </summary>
        [ObservableProperty]
        private string editSteamAccountAccount;

        /// <summary>
        /// 编辑steam账号名称
        /// </summary>
        [ObservableProperty]
        private string editSteamAccountName;

        /// <summary>
        /// 编辑steam账号密码
        /// </summary>
        [ObservableProperty]
        private string editSteamAccountPassword;

        /// <summary>
        /// 编辑steam账号图标
        /// </summary>
        [ObservableProperty]
        private byte[] editSteamAccountIcon;

        /// <summary>
        /// 可用账号图标
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<byte[]> allAccountIcons;

        #endregion

        #region 绑定方法

        /// <summary>
        /// 打开编辑抽屉
        /// </summary>
        [RelayCommand]
        private void OpenEditSteamAccountDrawer(SteamAccount steamAccount)
        {
            if (steamAccount == null)
            {
                return;
            }

            EditSteamAccountAccount = steamAccount.Account;
            EditSteamAccountName = steamAccount.Name;
            EditSteamAccountPassword = steamAccount.Password;
            EditSteamAccountIcon = steamAccount.Icon;

            // 加载所有账号图标列表
            var allSteamAccountIcons = SteamAnalyzer.GetAllAccountIcons();
            allSteamAccountIcons.AddRange(SteamAnalyzer.GetAllGameIcons());
            if (EditSteamAccountIcon != null)
            {
                var editSteamAccountIconStr = BitConverter.ToString(EditSteamAccountIcon);
                for (var i = 0; i < allSteamAccountIcons.Count; i++)
                {
                    if (BitConverter.ToString(allSteamAccountIcons[i]) == editSteamAccountIconStr)
                    {
                        allSteamAccountIcons.Remove(allSteamAccountIcons[i]);
                    }
                }
                allSteamAccountIcons.Insert(0, EditSteamAccountIcon);
            }
            AllAccountIcons = new ObservableCollection<byte[]>(allSteamAccountIcons);
            DrawerType = DrawerType.SteamAccount;
        }

        /// <summary>
        /// 关闭抽屉
        /// </summary>
        [RelayCommand]
        private void CloseEditSteamAccountDrawer()
        {
            DrawerType = DrawerType.None;
        }

        /// <summary>
        /// 复制用户名
        /// </summary>
        [RelayCommand]
        private void CopySteamAccountAccount()
        {
            if (string.IsNullOrEmpty(EditSteamAccountAccount))
            {
                return;
            }

            System.Windows.Clipboard.SetDataObject(EditSteamAccountAccount);
            Lactor.ShowToolTip("复制成功！");
        }

        /// <summary>
        /// 复制密码
        /// </summary>
        [RelayCommand]
        private void CopySteamAccountPassword()
        {
            if (string.IsNullOrEmpty(EditSteamAccountPassword))
            {
                return;
            }

            System.Windows.Clipboard.SetDataObject(EditSteamAccountPassword);
            Lactor.ShowToolTip("复制成功！");
        }

        /// <summary>
        /// 删除steam账号信息
        /// </summary>
        [RelayCommand]
        private void DeleteSteamAccount()
        {
            if (string.IsNullOrEmpty(EditSteamAccountAccount))
            {
                return;
            }

            if (!SteamAccountRepository.Exists(EditSteamAccountAccount))
            {
                return;
            }

            SteamAccountRepository.Delete(EditSteamAccountAccount);

            DrawerType = DrawerType.None;
            Lactor.ReLoad();
            Lactor.ShowToolTip("删除成功！");
        }

        /// <summary>
        /// 保存steam账号信息
        /// </summary>
        [RelayCommand]
        private void SaveSteamAccount()
        {
            if (string.IsNullOrEmpty(EditSteamAccountAccount))
            {
                DrawerType = DrawerType.None;
                return;
            }

            if (string.IsNullOrEmpty(EditSteamAccountName))
            {
                return;
            }

            var steamAccount = SteamAccountRepository.GetByAccount(EditSteamAccountAccount);
            if (steamAccount == null)
            {
                DrawerType = DrawerType.None;
                return;
            }

            steamAccount.Name = EditSteamAccountName;
            steamAccount.Password = EditSteamAccountPassword;
            steamAccount.Icon = EditSteamAccountIcon;
            SteamAccountRepository.Update(steamAccount);

            DrawerType = DrawerType.None;
            Lactor.ReLoad();
            Lactor.ShowToolTip("保存成功！");
        }

        /// <summary>
        /// 选择账号图标
        /// </summary>
        [RelayCommand]
        private void SelectAccountIcon(byte[] icon)
        {
            if (icon != null)
            {
                EditSteamAccountIcon = icon;
            }
        }

        #endregion
    }
}