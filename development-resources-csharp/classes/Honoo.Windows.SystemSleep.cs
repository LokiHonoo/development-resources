/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System.Runtime.InteropServices;

namespace Honoo.Windows
{
    /// <summary>
    /// 系统休眠。
    /// </summary>
    public static class SystemSleep
    {
        #region Native

        [DllImport("kernel32.dll")]
        private static extern uint SetThreadExecutionState(uint Flags);

        #endregion Native

        private const uint ES_CONTINUOUS = 0x80000000;

        private const uint ES_DISPLAY_REQUIRED = 0x00000002;

        private const uint ES_SYSTEM_REQUIRED = 0x00000001;

        /// <summary>
        ///阻止系统休眠，直到恢复休眠策略。
        /// </summary>
        /// <param name="includeDisplay">是否阻止关闭显示器。</param>
        /// <returns></returns>
        public static uint PreventSleep(bool includeDisplay)
        {
            if (includeDisplay)
            {
                return SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED | ES_CONTINUOUS);
            }
            else
            {
                return SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_CONTINUOUS);
            }
        }

        /// <summary>
        ///重置系统休眠计时器。
        /// </summary>
        /// <param name="includeDisplay">是否阻止关闭显示器。</param>
        /// <returns></returns>
        public static uint ResetSleepTimer(bool includeDisplay)
        {
            if (includeDisplay)
            {
                return SetThreadExecutionState(ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
            }
            else
            {
                return SetThreadExecutionState(ES_SYSTEM_REQUIRED);
            }
        }

        /// <summary>
        ///恢复系统休眠策略。
        /// </summary>
        /// <returns></returns>
        public static uint ResotreSleep()
        {
            return SetThreadExecutionState(ES_CONTINUOUS);
        }
    }
}