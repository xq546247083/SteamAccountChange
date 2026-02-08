using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using MaterialDesignThemes.Wpf;
using SteamHub.Common;
using SteamHub.Entities;
using SteamHub.Enums;
using SteamHub.Manager;
using SteamHub.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace SteamHub.ViewModel
{
    /// <summary>
    /// 主界面的ViewModel
    /// </summary>
    public partial class MainViewModel : ObservableObject, GongSolutions.Wpf.DragDrop.IDropTarget
    {
        /// <summary>
        /// 当前的Steam账号
        /// </summary>
        private string currentSteamAccount;

        /// <summary>
        /// 构造方法
        /// </summary>
        public MainViewModel()
        {
            Init();
        }

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

        /// <summary>
        /// 编辑类型
        /// </summary>
        [ObservableProperty]
        private EditType editType;

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
            var steamAccountSources = SteamAnalyzer.GetAllLoginAccounts();
            var steamAccounts = steamAccountSources.Select(r => new SteamAccount(Guid.NewGuid(), r.SteamId, r.AccountName, r.PersonaName ?? r.AccountName, string.Empty, 0, r.Icon)).ToList();
            SteamAccountRepository.AddList(steamAccounts);

            // 刷新游戏数据
            var steamGameSources = SteamAnalyzer.GetAllGames();
            var steamGames = steamGameSources.Select(r => new SteamGame(Guid.Empty, r.AppId, r.Name, r.Icon, r.AccountSteamId, 0)).ToList();
            SteamGameRepository.AddList(steamGames);

            ReLoad();
            Lactor.ShowToolTip("刷新成功!");
        }

        /// <summary>
        /// 打开 GitHub 仓库
        /// </summary>
        [RelayCommand]
        private void OpenGitHub()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/xq546247083/SteamHub",
                UseShellExecute = true
            });
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
            Lactor.TrayPopupViewModel.ReLoad();
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

        /// <summary>
        /// 加载steam账号
        /// </summary>
        private void LoadSteamAccounts()
        {
            // 加载账号信息
            SteamAccounts = new ObservableCollection<SteamAccount>(SteamAccountRepository.GetAll());

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
            LoadSteamGames();
            LoadProcesses();
        }

        #endregion

        #region 拖拽

        /// <summary>
        /// 拖拽
        /// </summary>
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data == dropInfo.TargetItem) return;

            if (dropInfo.Data is SteamAccount && dropInfo.TargetItem is SteamAccount)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
            else if (dropInfo.Data is SteamGame && dropInfo.TargetItem is SteamGame)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
        }

        /// <summary>
        /// 拖拽放下
        /// </summary>
        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is SteamAccount sourceAccount && dropInfo.TargetItem is SteamAccount targetAccount)
            {
                int oldIndex = SteamAccounts.IndexOf(sourceAccount);
                int newIndex = SteamAccounts.IndexOf(targetAccount);
                if (oldIndex != -1 && newIndex != -1)
                {
                    SteamAccounts.Move(oldIndex, newIndex);
                    for (int i = 0; i < SteamAccounts.Count; i++)
                    {
                        SteamAccounts[i].Order = i;
                    }

                    SteamAccountRepository.UpdateList(SteamAccounts.ToList());
                    Lactor.TrayPopupViewModel.ReLoad();
                }
            }
            else if (dropInfo.Data is SteamGame sourceGame && dropInfo.TargetItem is SteamGame targetGame)
            {
                int oldIndex = SteamGames.IndexOf(sourceGame);
                int newIndex = SteamGames.IndexOf(targetGame);
                if (oldIndex != -1 && newIndex != -1)
                {
                    SteamGames.Move(oldIndex, newIndex);
                    for (int i = 0; i < SteamGames.Count; i++)
                    {
                        SteamGames[i].Order = i;
                    }

                    SteamGameRepository.UpdateList(SteamGames.ToList());
                    Lactor.TrayPopupViewModel.ReLoad();
                }
            }
        }

        #endregion
    }
}