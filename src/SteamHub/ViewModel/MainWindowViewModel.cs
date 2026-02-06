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
    public class MainWindowViewModel : ObservableObject
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
        private List<SteamAccount> steamAccoutInfoList;

        /// <summary>
        /// 账号列表
        /// </summary>
        public List<SteamAccount> SteamAccoutInfoList
        {
            get
            {
                return steamAccoutInfoList;
            }
            set
            {
                steamAccoutInfoList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 选中账号
        /// </summary>
        private SteamAccount selectedSteamAccoutInfo;

        /// <summary>
        /// 选中账号
        /// </summary>
        public SteamAccount SelectedSteamAccoutInfo
        {
            get
            {
                return selectedSteamAccoutInfo;
            }
            set
            {
                selectedSteamAccoutInfo = value;

                if (value != null)
                {
                    SteamAccountAccountText = selectedSteamAccoutInfo.Account;
                    SteamAccountNameText = selectedSteamAccoutInfo.Name;
                    SteamAccountPasswordText = selectedSteamAccoutInfo.Password;
                    SteamAccountOrderText = selectedSteamAccoutInfo.Order;
                }

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 进程列表模式
        /// </summary>
        private ProcessListMode processListMode;

        /// <summary>
        /// 进程列表模式
        /// </summary>
        public ProcessListMode ProcessListMode
        {
            get
            {
                return processListMode;
            }
            set
            {
                processListMode = value;
                OnPropertyChanged();
                LoadProcessList();
            }
        }

        /// <summary>
        /// 显示的进程列表
        /// </summary>
        private List<string> displayProcessList;

        /// <summary>
        /// 显示的进程列表
        /// </summary>
        public List<string> DisplayProcessList
        {
            get
            {
                return displayProcessList;
            }
            set
            {
                displayProcessList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 选中的进程名
        /// </summary>
        private string selectedProcessName;

        /// <summary>
        /// 选中的进程名
        /// </summary>
        public string SelectedProcessName
        {
            get
            {
                return selectedProcessName;
            }
            set
            {
                selectedProcessName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// steamAccount的Account文本内容
        /// </summary>
        private string steamAccountAccountText;

        /// <summary>
        /// steamAccount的Account文本内容
        /// </summary>
        public string SteamAccountAccountText
        {
            get
            {
                return steamAccountAccountText;
            }
            set
            {
                steamAccountAccountText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// steamAccount的Name文本内容
        /// </summary>
        private string steamAccountNameText;

        /// <summary>
        /// steamAccount的Name文本内容
        /// </summary>
        public string SteamAccountNameText
        {
            get
            {
                return steamAccountNameText;
            }
            set
            {
                steamAccountNameText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// steamAccount的Password文本内容
        /// </summary>
        private string steamAccountPasswordText;

        /// <summary>
        /// steamAccount的Password文本内容
        /// </summary>
        public string SteamAccountPasswordText
        {
            get
            {
                return steamAccountPasswordText;
            }
            set
            {
                steamAccountPasswordText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// steamAccount的Order文本内容
        /// </summary>
        private string steamAccountOrderText;

        /// <summary>
        /// steamAccount的Order文本内容
        /// </summary>
        public string SteamAccountOrderText
        {
            get
            {
                return steamAccountOrderText;
            }
            set
            {
                steamAccountOrderText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否打开右侧抽屉
        /// </summary>
        private bool isRightDrawerOpen;

        /// <summary>
        /// 是否打开右侧抽屉
        /// </summary>
        public bool IsRightDrawerOpen
        {
            get
            {
                return isRightDrawerOpen;
            }
            set
            {
                isRightDrawerOpen = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Snackbar消息队列
        /// </summary>
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get;
        } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

        /// <summary>
        /// 游戏列表
        /// </summary>
        private List<SteamGame> steamGameList;

        /// <summary>
        /// 游戏列表
        /// </summary>
        public List<SteamGame> SteamGameList
        {
            get
            {
                return steamGameList;
            }
            set
            {
                steamGameList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region 绑定方法

        /// <summary>
        /// 按键
        /// </summary>
        public RelayCommand<CommandParameterEx> MainWindowKeyDownCommand => new RelayCommand<CommandParameterEx>(DoMainWindowKeyDown);

        /// <summary>
        /// 按键
        /// </summary>
        /// <param name="commandParameterEx">参数</param>
        private void DoMainWindowKeyDown(CommandParameterEx commandParameterEx)
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
        public RelayCommand<CommandParameterEx> MainWindowClosingCommand => new RelayCommand<CommandParameterEx>(DoMainWindowClosing);

        /// <summary>
        /// 正在关闭窗口
        /// </summary>
        /// <param name="commandParameterEx">参数</param>
        private void DoMainWindowClosing(CommandParameterEx commandParameterEx)
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
        public RelayCommand NewBtnClickCommand => new RelayCommand(DoNewBtnClickCommand);

        /// <summary>
        /// 点击新建
        /// </summary>
        private void DoNewBtnClickCommand()
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
        public RelayCommand CopyUserAccountBtnClickCommand => new RelayCommand(DoCopyUserAccountBtnClick);

        /// <summary>
        /// 复制用户名
        /// </summary>
        private void DoCopyUserAccountBtnClick()
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
        public RelayCommand CopyPasswordBtnClickCommand => new RelayCommand(DoCopyPasswordBtnClick);

        /// <summary>
        /// 复制密码
        /// </summary>
        private void DoCopyPasswordBtnClick()
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
        public RelayCommand OpenEditDrawerCommand => new RelayCommand(DoOpenEditDrawer);

        /// <summary>
        /// 打开编辑抽屉
        /// </summary>
        private void DoOpenEditDrawer()
        {
            if (SelectedSteamAccoutInfo == null)
            {
                return;
            }
            IsRightDrawerOpen = true;
        }

        /// <summary>
        /// 关闭抽屉
        /// </summary>
        public RelayCommand CloseDrawerCommand => new RelayCommand(DoCloseDrawer);

        /// <summary>
        /// 关闭抽屉
        /// </summary>
        private void DoCloseDrawer()
        {
            IsRightDrawerOpen = false;
        }

        /// <summary>
        /// 保存steam账号信息
        /// </summary>
        public RelayCommand SaveSteamAccoutInfoBtnClickCommand => new RelayCommand(DoSaveSteamAccoutInfoBtnClick);

        /// <summary>
        /// 保存steam账号信息
        /// </summary>
        private void DoSaveSteamAccoutInfoBtnClick()
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
        public RelayCommand DelSteamAccoutInfoBtnClickCommand => new RelayCommand(DoDelSteamAccoutInfoBtnClick);

        /// <summary>
        /// 删除steam账号信息
        /// </summary>
        private void DoDelSteamAccoutInfoBtnClick()
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
        public RelayCommand SaveGameProcessInfoBtnClickCommand => new RelayCommand(DoSaveGameProcessInfoBtnClick);

        /// <summary>
        /// 添加游戏进程信息
        /// </summary>
        private void DoSaveGameProcessInfoBtnClick()
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
        public RelayCommand DelGameProcessInfoBtnClickCommand => new RelayCommand(DoDelGameProcessInfoBtnClick);

        /// <summary>
        /// 删除游戏进程信息
        /// </summary>
        private void DoDelGameProcessInfoBtnClick()
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
        public RelayCommand RefreshSteamDataCommand => new RelayCommand(DoRefreshSteamData);

        /// <summary>
        /// 刷新 Steam 数据
        /// </summary>
        private void DoRefreshSteamData()
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
            if (selectedSteamAccoutInfo == null && SteamAccoutInfoList != null && SteamAccoutInfoList.Count > 0)
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