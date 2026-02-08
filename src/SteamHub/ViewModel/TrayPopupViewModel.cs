using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Entities;
using SteamHub.Enums;
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
        /// 游戏列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SteamGame> steamGames;

        /// <summary>
        /// 显示类型
        /// </summary>
        [ObservableProperty]
        private ShowType showType = ShowType.SteamAccount;

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
            NotifyIconManager.Close();
        }

        /// <summary>
        /// 登录Steam账号
        /// </summary>
        [RelayCommand]
        private void LoginSteamAccount()
        {
            var processList = SettingRepository.GetKillProcessList();
            SteamTool.Open(string.Empty, processList);
            NotifyIconManager.Close();
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
            NotifyIconManager.Close();
        }

        /// <summary>
        /// 推出
        /// </summary>
        [RelayCommand]
        private void Exit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 切换显示方式
        /// </summary>
        [RelayCommand]
        private void ToggleShowType()
        {
            if (ShowType == ShowType.SteamAccount)
            {
                ShowType = ShowType.SteamGame;
            }
            else
            {
                ShowType = ShowType.SteamAccount;
            }
        }

        /// <summary>
        /// 打开Steam游戏
        /// </summary>
        [RelayCommand]
        private void OpenSteamGame(SteamGame game)
        {
            if (game == null)
            {
                return;
            }

            // 先切换账号
            var (success, currentLoginAccount) = RegistryHelper.Get(@"Software\Valve\Steam", "AutoLoginUser");
            var steamAccount = SteamAccountRepository.GetBySteamId(game.AccountSteamId);
            if (success && steamAccount != null && currentLoginAccount?.ToString() != steamAccount.Account)
            {
                var processList = SettingRepository.GetKillProcessList();
                SteamTool.Open(steamAccount.Account, processList);
            }

            // 再打开游戏
            SteamTool.OpenGame(game.AppId);
            NotifyIconManager.Close();
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

            // 加载游戏
            var games = SteamGameRepository.GetAll();
            if (games != null)
            {
                SteamGames = new ObservableCollection<SteamGame>(games);
            }

            // 加载自启动状态
            IsLaunchOnSysPowerOn = TaskSchedulerHelper.Get(AppGlobal.AppName) != null;
        }

        #endregion
    }
}