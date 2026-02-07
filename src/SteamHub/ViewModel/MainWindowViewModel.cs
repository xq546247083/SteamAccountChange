using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using SteamHub.Common;
using SteamHub.Entities;
using SteamHub.Manager;
using SteamHub.Model;
using SteamHub.Repositories;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace SteamHub.ViewModel
{
    /// <summary>
    /// 主界面的ViewModel
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// 当前的Steam账号
        /// </summary>
        private string currentSteamAccount;

        /// <summary>
        /// 构造方法
        /// </summary>
        public MainWindowViewModel()
        {
            Init();
        }

        #region 绑定属性

        /// <summary>
        /// 账号列表
        /// </summary>
        [ObservableProperty]
        private List<SteamAccount> steamAccounts;

        /// <summary>
        /// 选中账号
        /// </summary>
        [ObservableProperty]
        private SteamAccount selectedSteamAccount;

        /// <summary>
        /// Snackbar消息队列
        /// </summary>
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get;
        } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

        #endregion

        #region 绑定方法

        /// <summary>
        /// 按键
        /// </summary>
        /// <param name="commandParameterEx">参数</param>
        [RelayCommand]
        private void MainWindowKeyDown(CommandParameterEx commandParameterEx)
        {
            // 转换参数
            var keyEventArgs = commandParameterEx.EventArgs as System.Windows.Input.KeyEventArgs;
            if (keyEventArgs == null)
            {
                return;
            }

            switch (keyEventArgs.Key)
            {
                case Key.F1:
                    break;
            }
        }

        /// <summary>
        /// 正在关闭窗口
        /// </summary>
        /// <param name="commandParameterEx">参数</param>
        [RelayCommand]
        private void MainWindowClosing(CommandParameterEx commandParameterEx)
        {
            // 转换参数
            var cancelEventArgs = commandParameterEx.EventArgs as CancelEventArgs;
            if (cancelEventArgs == null)
            {
                return;
            }

            Lactor.MainWindow.Visibility = System.Windows.Visibility.Hidden;
            Lactor.MainWindow.ShowInTaskbar = false;
            cancelEventArgs.Cancel = true;
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

            ReLoad();
            Lactor.ShowToolTip("删除成功！");
        }
        
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

        /// <summary>
        /// 刷新 Steam 数据
        /// </summary>
        [RelayCommand]
        private void Refresh()
        {
            // 刷新账号数据
            var steamAccountSources = SteamAnalyzer.GetAllLoginUsers();
            var steamAccounts = steamAccountSources.Select(r => new SteamAccount(Guid.NewGuid(), r.SteamId, r.AccountName, r.PersonaName ?? r.AccountName, string.Empty, "0", r.Icon)).ToList();
            SteamAccountRepository.AddOrUpdateRange(steamAccounts);

            // 刷新游戏数据
            var steamGameSources = SteamAnalyzer.GetAllGames();
            var steamGames = steamGameSources.Select(r => new SteamGame(Guid.Empty, r.AppId, r.Name, r.Icon, r.LastOwnerSteamId)).ToList();
            SteamGameRepository.AddOrUpdateRange(steamGames);

            ReLoad();
            Lactor.ShowToolTip("刷新成功!");
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            LoadAllData();
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        private void ReLoad()
        {
            LoadAllData();
            NotifyIconManager.LoadMenu();
        }

        /// <summary>
        /// 加载进程列表
        /// </summary>
        private void LoadProcesses()
        {
            if (ProcessListMode == ProcessListMode.System)
            {
                var processes = Process.GetProcesses();
                DisplayProcessList = processes.Select(p => p.ProcessName).Distinct().OrderBy(n => n).ToList();
            }
            else
            {
                DisplayProcessList = SettingRepository.GetKillProcessList();
            }

            SelectedProcessName = DisplayProcessList != null && DisplayProcessList.Count > 0 ? DisplayProcessList.FirstOrDefault() : string.Empty;
        }

        private void LoadSteamAccounts() 
        {
            // 加载账号信息
            SteamAccounts = SteamAccountRepository.GetAll();

            // 选中第一个
            if (SelectedSteamAccount == null && SteamAccounts != null && SteamAccounts.Count > 0)
            {
                var currentSteamAccountInfo = string.IsNullOrEmpty(currentSteamAccount) ? null : SteamAccounts.FirstOrDefault(r => r.Account == currentSteamAccount);
                SelectedSteamAccount = currentSteamAccountInfo == null ? SteamAccounts.FirstOrDefault() : currentSteamAccountInfo;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 加载所有数据
        /// </summary>
        public void LoadAllData()
        {
            LoadSteamAccounts();
            LoadGames();
            LoadProcesses();
        }

        #endregion
    }
}