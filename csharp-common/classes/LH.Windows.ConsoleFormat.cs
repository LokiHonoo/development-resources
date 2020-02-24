/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Runtime.InteropServices;

namespace LH.Windows
{
    /// <summary>
    /// Windows 控制台控制。
    /// </summary>
    public static class ConsoleFormat
    {
        #region API

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int hConsoleHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

        #endregion API

        private const uint ENABLE_INSERT_MODE = 0x0020;

        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

        private const int STD_INPUT_HANDLE = -10;

        /// <summary>
        /// 关闭控制台快速编辑模式，防止阻塞现象。
        /// </summary>
        public static void DisableQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(hStdin, out uint mode);
            mode &= ~ENABLE_INSERT_MODE;
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(hStdin, mode);
        }
    }
}