using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private void OpenEditSteamAccountDrawer()
        {
            if (SelectedSteamAccount == null)
            {
                return;
            }

            EditSteamAccountName = SelectedSteamAccount.Name;
            EditSteamAccountPassword = SelectedSteamAccount.Password;
            EditSteamAccountIcon = SelectedSteamAccount.Icon;

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
            EditType = EditType.SteamAccount;
        }

        /// <summary>
        /// 关闭抽屉
        /// </summary>
        [RelayCommand]
        private void CloseEditSteamAccountDrawer()
        {
            EditType = EditType.None;
        }

        /// <summary>
        /// 复制用户名
        /// </summary>
        [RelayCommand]
        private void CopySteamAccountAccount()
        {
            if (SelectedSteamAccount == null || SelectedSteamAccount.Account == null)
            {
                return;
            }

            System.Windows.Clipboard.SetDataObject(SelectedSteamAccount.Account);
            Lactor.ShowToolTip("复制成功！");
        }

        /// <summary>
        /// 复制密码
        /// </summary>
        [RelayCommand]
        private void CopySteamAccountPassword()
        {
            if (SelectedSteamAccount == null || SelectedSteamAccount.Password == null)
            {
                return;
            }

            System.Windows.Clipboard.SetDataObject(SelectedSteamAccount.Password);
            Lactor.ShowToolTip("复制成功！");
        }

        /// <summary>
        /// 删除steam账号信息
        /// </summary>
        [RelayCommand]
        private void DeleteSteamAccount()
        {
            if (SelectedSteamAccount == null)
            {
                return;
            }

            if (!SteamAccountRepository.Exists(SelectedSteamAccount.Account))
            {
                return;
            }

            SteamAccountRepository.Delete(SelectedSteamAccount.Account);

            EditType = EditType.None;
            ReLoad();
            Lactor.ShowToolTip("删除成功！");
        }

        /// <summary>
        /// 保存steam账号信息
        /// </summary>
        [RelayCommand]
        private void SaveSteamAccount()
        {
            if (SelectedSteamAccount == null)
            {
                EditType = EditType.None;
                return;
            }

            if (string.IsNullOrEmpty(EditSteamAccountName))
            {
                return;
            }

            if (!SteamAccountRepository.Exists(SelectedSteamAccount.Account))
            {
                return;
            }

            SelectedSteamAccount.Name = EditSteamAccountName;
            SelectedSteamAccount.Password = EditSteamAccountPassword;
            SelectedSteamAccount.Icon = EditSteamAccountIcon;
            SteamAccountRepository.Update(SelectedSteamAccount);

            currentSteamAccount = SelectedSteamAccount.Account;
            EditType = EditType.None;

            ReLoad();
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