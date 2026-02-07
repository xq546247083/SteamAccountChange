using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        /// <summary>
        /// 是否编辑
        /// </summary>
        [ObservableProperty]
        private bool isEdit;

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
            IsEdit = true;
        }

        /// <summary>
        /// 关闭抽屉
        /// </summary>
        [RelayCommand]
        private void CloseEditSteamAccountDrawer()
        {
            IsEdit = false;
        }

        /// <summary>
        /// 保存steam账号信息
        /// </summary>
        [RelayCommand]
        private void SaveSteamAccount()
        {
            if (SelectedSteamAccount == null)
            {
                IsEdit = false;
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
            IsEdit = false;

            ReLoad();
            Lactor.ShowToolTip("保存成功！");
        }

        #endregion
    }
}