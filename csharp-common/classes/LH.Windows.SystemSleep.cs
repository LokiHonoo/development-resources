/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System.Runtime.InteropServices;

namespace LH.Windows
{
    /// <summary>
    /// 系统休眠。
    /// </summary>
    public static class SystemSleep
    {
        private const uint ES_CONTINUOUS = 0x80000000;

        private const uint ES_DISPLAY_REQUIRED = 0x00000002;

        private const uint ES_SYSTEM_REQUIRED = 0x00000001;

        /// <summary>
        ///阻止系统休眠，直到恢复休眠策略。
        /// </summary>
        /// <param name="includeDisplay">是否阻止关闭显示器。</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "<Pending>")]
        public static void PreventSleep(bool includeDisplay)
        {
            if (includeDisplay)
            {
                NativeMethods.SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED | ES_CONTINUOUS);
            }
            else
            {
                NativeMethods.SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_CONTINUOUS);
            }
        }

        /// <summary>
        ///重置系统休眠计时器。
        /// </summary>
        /// <param name="includeDisplay">是否阻止关闭显示器。</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "<Pending>")]
        public static void ResetSleepTimer(bool includeDisplay)
        {
            if (includeDisplay)
            {
                NativeMethods.SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
            }
            else
            {
                NativeMethods.SetThreadExecutionState(ES_SYSTEM_REQUIRED);
            }
        }

        /// <summary>
        ///恢复系统休眠策略。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "<Pending>")]
        public static void ResotreSleep()
        {
            NativeMethods.SetThreadExecutionState(ES_CONTINUOUS);
        }
    }

    internal static partial class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern uint SetThreadExecutionState(uint Flags);
    }
}