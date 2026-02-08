using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Entities;
using SteamHub.Helper;
using SteamHub.Manager;
using SteamHub.Repositories;
using System.Collections.ObjectModel;

namespace SteamHub.ViewModel
{
    /// <summary>
    /// 托盘 ViewModel
    /// </summary>
    public partial class TrayPopupViewModel : ObservableObject
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public TrayPopupViewModel()
        {
            ReLoad();
        }

        #region 绑定属性

        /// <summary>
        /// 账号列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SteamAccount> steamAccounts;

        /// <summary>
        /// 是否开机自启动
        /// </summary>
        [ObservableProperty]
        private bool isLaunchOnSysPowerOn;

        #endregion

        #region 绑定方法

        /// <summary>
        /// 切换Steam账号
        /// </summary>
        /// <param name="account"></param>
        [RelayCommand]
        private void SwitchSteamAccount(SteamAccount account)
        {
            if (account == null)
            {
                return;
            }

            var processList = SettingRepository.GetKillProcessList();
            SteamTool.Open(account.Account, processList);
        }

        /// <summary>
        /// 登录Steam账号
        /// </summary>
        [RelayCommand]
        private void LoginSteamAccount()
        {
            var processList = SettingRepository.GetKillProcessList();
            SteamTool.Open(string.Empty, processList);
        }

        /// <summary>
        /// 切换自启动
        /// </summary>
        [RelayCommand]
        private void ToggleAutoStart()
        {
            var targetState = IsLaunchOnSysPowerOn;
            if (targetState)
            {
                IsLaunchOnSysPowerOn = TaskSchedulerHelper.AddLuanchTask(AppGlobal.AppName, Application.ExecutablePath);
            }
            else
            {
                TaskSchedulerHelper.Delete(AppGlobal.AppName);
            }
        }

        /// <summary>
        /// 打开主窗口
        /// </summary>
        [RelayCommand]
        private void OpenMainWindow()
        {
            Lactor.OpenMainWindow();
        }

        /// <summary>
        /// 推出
        /// </summary>
        [RelayCommand]
        private void Exit()
        {
            Environment.Exit(0);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 记载数据
        /// </summary>
        public void ReLoad()
        {
            // 加载账号
            var accounts = SteamAccountRepository.GetAll();
            if (accounts != null)
            {
                SteamAccounts = new ObservableCollection<SteamAccount>(accounts);
            }

            // 加载自启动状态
            IsLaunchOnSysPowerOn = TaskSchedulerHelper.Get(AppGlobal.AppName) != null;
        }

        #endregion
    }
}