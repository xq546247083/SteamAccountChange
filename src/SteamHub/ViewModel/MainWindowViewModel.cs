using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using SteamHub.Common;
using SteamHub.Entities;
using SteamHub.Helper;
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
        private List<SteamAccount> steamAccoutInfoList;

        /// <summary>
        /// 选中账号
        /// </summary>
        [ObservableProperty]
        private SteamAccount selectedSteamAccoutInfo;

        /// <summary>
        /// 进程列表模式
        /// </summary>
        [ObservableProperty]
        private ProcessListMode processListMode;

        /// <summary>
        /// 显示的进程列表
        /// </summary>
        [ObservableProperty]
        private List<string> displayProcessList;

        /// <summary>
        /// 选中的进程名
        /// </summary>
        [ObservableProperty]
        private string selectedProcessName;

        /// <summary>
        /// steamAccount的Account文本内容
        /// </summary>
        [ObservableProperty]
        private string steamAccountAccountText;

        /// <summary>
        /// steamAccount的Name文本内容
        /// </summary>
        [ObservableProperty]
        private string steamAccountNameText;

        /// <summary>
        /// steamAccount的Password文本内容
        /// </summary>
        [ObservableProperty]
        private string steamAccountPasswordText;

        /// <summary>
        /// steamAccount的Order文本内容
        /// </summary>
        [ObservableProperty]
        private string steamAccountOrderText;

        /// <summary>
        /// 是否打开右侧抽屉
        /// </summary>
        [ObservableProperty]
        private bool isRightDrawerOpen;

        /// <summary>
        /// 游戏列表
        /// </summary>
        [ObservableProperty]
        private List<SteamGame> steamGameList;

        /// <summary>
        /// Snackbar消息队列
        /// </summary>
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get;
        } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

        #endregion

        #region 属性变化引发的事件

        partial void OnProcessListModeChanged(ProcessListMode oldValue, ProcessListMode newValue)
        {
            LoadProcessList();
        }

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
        /// 点击新建
        /// </summary>
        [RelayCommand]
        private void NewBtnClick()
        {
            var (success, autoLoginUserObj) = RegistryHelper.Get(@"Software\Valve\Steam", "AutoLoginUser");
            if (success == false || autoLoginUserObj == null)
            {
                Lactor.ShowToolTip("请先登陆Steam！");
                return;
            }

            var autoLoginUser = autoLoginUserObj.ToString();
            if (string.IsNullOrEmpty(autoLoginUser))
            {
                Lactor.ShowToolTip("请先登陆Steam！");
                return;
            }

            if (SteamAccountRepository.Exists(autoLoginUser))
            {
                return;
            }

            // 构建账号信息
            var steamAccount = new SteamAccount();
            steamAccount.Account = autoLoginUser;
            steamAccount.Name = autoLoginUser;
            steamAccount.Password = "";
            steamAccount.Order = "0";
            SteamAccountRepository.Add(steamAccount);

            currentSteamAccount = steamAccount.Account;
            ReLoad();
            Lactor.ShowToolTip("添加成功！");
        }

        /// <summary>
        /// 复制用户名
        /// </summary>
        [RelayCommand]
        private void CopyUserAccountBtnClick()
        {
            if (SelectedSteamAccoutInfo == null || SelectedSteamAccoutInfo.Account == null)
            {
                return;
            }

            System.Windows.Clipboard.SetDataObject(SelectedSteamAccoutInfo.Account);
            Lactor.ShowToolTip("复制成功！");
        }

        /// <summary>
        /// 复制密码
        /// </summary>
        [RelayCommand]
        private void CopyPasswordBtnClick()
        {
            if (SelectedSteamAccoutInfo == null || SelectedSteamAccoutInfo.Password == null)
            {
                return;
            }

            System.Windows.Clipboard.SetDataObject(SelectedSteamAccoutInfo.Password);
            Lactor.ShowToolTip("复制成功！");
        }

        /// <summary>
        /// 打开编辑抽屉
        /// </summary>
        [RelayCommand]
        private void OpenEditDrawer()
        {
            if (SelectedSteamAccoutInfo == null)
            {
                return;
            }

            SteamAccountAccountText = SelectedSteamAccoutInfo.Account;
            SteamAccountNameText = SelectedSteamAccoutInfo.Name;
            SteamAccountPasswordText = SelectedSteamAccoutInfo.Password;
            SteamAccountOrderText = SelectedSteamAccoutInfo.Order;
            IsRightDrawerOpen = true;
        }

        /// <summary>
        /// 关闭抽屉
        /// </summary>
        [RelayCommand]
        private void CloseDrawer()
        {
            IsRightDrawerOpen = false;
        }

        /// <summary>
        /// 保存steam账号信息
        /// </summary>
        [RelayCommand]
        private void SaveSteamAccoutInfoBtnClick()
        {
            if (SelectedSteamAccoutInfo == null)
            {
                IsRightDrawerOpen = false;
                return;
            }

            if (!SteamAccountRepository.Exists(SelectedSteamAccoutInfo.Account))
            {
                return;
            }

            SelectedSteamAccoutInfo.Name = SteamAccountNameText;
            SelectedSteamAccoutInfo.Password = SteamAccountPasswordText;
            SelectedSteamAccoutInfo.Order = SteamAccountOrderText;
            SteamAccountRepository.Update(SelectedSteamAccoutInfo);

            currentSteamAccount = SelectedSteamAccoutInfo.Account;
            IsRightDrawerOpen = false;

            ReLoad();
            Lactor.ShowToolTip("保存成功！");
        }

        /// <summary>
        /// 删除steam账号信息
        /// </summary>
        [RelayCommand]
        private void DelSteamAccoutInfoBtnClick()
        {
            if (SelectedSteamAccoutInfo == null)
            {
                return;
            }

            if (!SteamAccountRepository.Exists(SelectedSteamAccoutInfo.Account))
            {
                return;
            }

            SteamAccountRepository.Delete(SelectedSteamAccoutInfo.Account);

            ReLoad();
            Lactor.ShowToolTip("删除成功！");
        }

        /// <summary>
        /// 添加游戏进程信息
        /// </summary>
        [RelayCommand]
        private void SaveGameProcessInfoBtnClick()
        {
            if (ProcessListMode != ProcessListMode.System)
            {
                return;
            }

            if (string.IsNullOrEmpty(SelectedProcessName))
            {
                return;
            }

            SettingRepository.AddKillProcess(SelectedProcessName);

            ReLoad();
            Lactor.ShowToolTip("添加成功！");
        }

        /// <summary>
        /// 删除游戏进程信息
        /// </summary>
        [RelayCommand]
        private void DelGameProcessInfoBtnClick()
        {
            if (ProcessListMode != ProcessListMode.Saved)
            {
                return;
            }

            if (string.IsNullOrEmpty(SelectedProcessName))
            {
                return;
            }

            SettingRepository.DeleteKillProcess(SelectedProcessName);

            ReLoad();
            Lactor.ShowToolTip("删除成功!");
        }

        /// <summary>
        /// 刷新 Steam 数据
        /// </summary>
        [RelayCommand]
        private void RefreshSteamData()
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
            LoadSaveInfo();
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        private void ReLoad()
        {
            NotifyIconManager.LoadMenu();
            LoadSaveInfo();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 加载保存的信息
        /// </summary>
        public void LoadSaveInfo()
        {
            // 加载账号信息
            SteamAccoutInfoList = SteamAccountRepository.GetAll();

            // 选中第一个
            if (SelectedSteamAccoutInfo == null && SteamAccoutInfoList != null && SteamAccoutInfoList.Count > 0)
            {
                // 选回之前的账号
                SteamAccount currentSteamAccountInfo = null;
                if (!string.IsNullOrEmpty(currentSteamAccount))
                {
                    currentSteamAccountInfo = SteamAccoutInfoList.FirstOrDefault(r => r.Account == currentSteamAccount);
                }

                // 选中第一个
                if (currentSteamAccountInfo == null)
                {
                    currentSteamAccountInfo = SteamAccoutInfoList.FirstOrDefault();
                }

                SelectedSteamAccoutInfo = currentSteamAccountInfo;
            }

            // 加载游戏信息
            SteamGameList = SteamGameRepository.GetAll();

            LoadProcessList();
        }

        /// <summary>
        /// 加载进程列表
        /// </summary>
        private void LoadProcessList()
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

            if (DisplayProcessList != null && DisplayProcessList.Count > 0)
            {
                SelectedProcessName = DisplayProcessList.FirstOrDefault();
            }
            else
            {
                SelectedProcessName = string.Empty;
            }
        }

        #endregion
    }
}