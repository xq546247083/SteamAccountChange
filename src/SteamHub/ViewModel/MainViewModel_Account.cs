using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Entities;
using SteamHub.Repositories;
using System.Collections.ObjectModel;

namespace SteamHub.ViewModel
{
    public partial class MainViewModel
    {
        /// <summary>
        /// 当前的Steam账号
        /// </summary>
        private string currentSteamAccount;

        #region 绑定属性

        /// <summary>
        /// 账号列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SteamAccount> steamAccounts;

        /// <summary>
        /// 选中账号
        /// </summary>
        [ObservableProperty]
        private SteamAccount selectedSteamAccount;

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
    }
}