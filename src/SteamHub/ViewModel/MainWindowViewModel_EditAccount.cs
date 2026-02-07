using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Enums;
using SteamHub.Manager;
using SteamHub.Repositories;

namespace SteamHub.ViewModel
{
    public partial class MainWindowViewModel
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
            SteamAccountRepository.Update(SelectedSteamAccount);

            currentSteamAccount = SelectedSteamAccount.Account;
            EditType = EditType.None;

            ReLoad();
            Lactor.ShowToolTip("保存成功！");
        }

        #endregion
    }
}