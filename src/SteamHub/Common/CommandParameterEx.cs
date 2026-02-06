using System;
using System.Windows;

namespace SteamAccountChange.Common
{
    /// <summary>
	/// 扩展CommandParameter，使CommandParameter可以带事件参数
	/// </summary>
    public class CommandParameterEx
    {
        /// <summary>
		/// 事件触发源
		/// </summary>
		public DependencyObject Sender
        {
            get; set;
        }

        /// <summary>
        /// 事件参数
        /// </summary>
        public EventArgs EventArgs
        {
            get; set;
        }

        /// <summary>
        /// 额外参数
        /// </summary>
        public object Parameter
        {
            get; set;
        }
    }
}
