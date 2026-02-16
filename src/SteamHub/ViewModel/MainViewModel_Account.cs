using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Entities;
using SteamHub.Repositories;
using System.Collections.ObjectModel;

namespace SteamHub.ViewModel
{
    public partial class MainViewModel
    {
        #region 绑定属性

        /// <summary>
        /// 账号列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SteamAccount> steamAccounts;

        #endregion

        #region 绑定方法

        /// <summary>
        /// 打开Steam登录账号
        /// </summary>
        [RelayCommand]
        private void OpenSteamAccount(SteamAccount account)
        {
            if (account == null)
            {
                return;
            }

            var processList = SettingRepository.GetKillProcessList();
            SteamTool.Open(account.Account, processList);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 加载steam账号
        /// </summary>
        private void LoadSteamAccounts()
        {
            SteamAccounts = new ObservableCollection<SteamAccount>(SteamAccountRepository.GetAll());
        }

        #endregion
    }
}