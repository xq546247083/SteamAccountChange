using SteamHub.Manager;
using System.Windows;

namespace SteamHub
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Lactor.MainViewModel;
        }
    }
}