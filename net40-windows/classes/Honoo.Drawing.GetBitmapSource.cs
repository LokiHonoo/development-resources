/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2023. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Honoo.Drawing
{
    /// <summary>
    /// Contains information about a file object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHFILEINFO : IEquatable<SHFILEINFO>
    {
        /// <summary>
        /// A handle to the icon that represents the file. You are responsible for destroying this handle with DestroyIcon when you no longer need it.
        /// </summary>
        internal IntPtr hIcon;

        /// <summary>
        /// The index of the icon image within the system image list.
        /// </summary>
        internal int iIcon;

        /// <summary>
        /// An array of values that indicates the attributes of the file object. For information about these values, see the IShellFolder::GetAttributesOf method.
        /// </summary>
        internal uint dwAttributes;

        /// <summary>
        /// A string that contains the name of the file as it appears in the Windows Shell, or the path and file name of the file that contains the icon representing the file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string szDisplayName;

        /// <summary>
        /// A string that describes the type of file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        internal string szTypeName;

        public override  bool Equals(object obj)
        {
            return obj is SHFILEINFO s && this.hIcon.Equals(s.hIcon);
        }

        public override  int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SHFILEINFO left, SHFILEINFO right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SHFILEINFO left, SHFILEINFO right)
        {
            return !(left == right);
        }

        public  bool Equals(SHFILEINFO other)
        {
            return other is SHFILEINFO s && this.GetHashCode().Equals(s.GetHashCode());
        }
    }

    /// <summary>
    /// Icon.
    /// </summary>
    internal static class Icon
    {
        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_READONLY = 0x00000001;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;

        /// <summary>
        ///
        /// </summary>
        internal const uint FILE_ATTRIBUTE_VIRTUAL = 0x00010000;

        /// <summary>
        /// Get only specified attributes.
        /// </summary>
        internal const uint SHGFI_ATTR_SPECIFIED = 0x000020000;

        /// <summary>
        /// Get attributes.
        /// </summary>
        internal const uint SHGFI_ATTRIBUTES = 0x000000800;

        /// <summary>
        /// Get display name.
        /// </summary>
        internal const uint SHGFI_DISPLAYNAME = 0x000000200;

        /// <summary>
        /// Return exe type.
        /// </summary>
        internal const uint SHGFI_EXETYPE = 0x000002000;

        /// <summary>
        /// Get icon.
        /// </summary>
        internal const uint SHGFI_ICON = 0x000000100;

        /// <summary>
        /// Get icon location.
        /// </summary>
        internal const uint SHGFI_ICONLOCATION = 0x000001000;

        /// <summary>
        /// Get large icon.
        /// </summary>
        internal const uint SHGFI_LARGEICON = 0x000000000;

        /// <summary>
        /// Put a link overlay on icon.
        /// </summary>
        internal const uint SHGFI_LINKOVERLAY = 0x000008000;

        /// <summary>
        /// Get open icon.
        /// </summary>
        internal const uint SHGFI_OPENICON = 0x000000002;

        /// <summary>
        /// pszPath is a pidl.
        /// </summary>
        internal const uint SHGFI_PIDL = 0x000000008;

        /// <summary>
        /// Get icon in selected state.
        /// </summary>
        internal const uint SHGFI_SELECTED = 0x000010000;

        /// <summary>
        /// Get shell size icon.
        /// </summary>
        internal const uint SHGFI_SHELLICONSIZE = 0x000000004;

        /// <summary>
        /// Get small icon.
        /// </summary>
        internal const uint SHGFI_SMALLICON = 0x000000001;

        /// <summary>
        /// Get system icon index.
        /// </summary>
        internal const uint SHGFI_SYSICONINDEX = 0x000004000;

        /// <summary>
        /// Get type name.
        /// </summary>
        internal const uint SHGFI_TYPENAME = 0x000000400;

        /// <summary>
        /// Use passed dwFileAttribute. Takes the file name and attributes into account if it doesn't exist.
        /// </summary>
        internal const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        /// <summary>
        /// Get icon by settings. <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/shellapi/nf-shellapi-shgetfileinfoa"/> .
        /// <br />Using dwFileAttributes = FILE_ATTRIBUTE_NORMAL; uflags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES; sourceRect = Int32Rect.Empty; options = BitmapSizeOptions.FromEmptyOptions().
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid.</param>
        /// <returns></returns>
        internal static BitmapSource GetFromHIcon(string pszPath)
        {
            uint uflags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            uint dwFileAttributes = FILE_ATTRIBUTE_NORMAL;
            if (SHGetFileInfo(pszPath, dwFileAttributes, out SHFILEINFO fi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), uflags) != 0)
            {
                return Imaging.CreateBitmapSourceFromHIcon(fi.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            return null;
        }

        /// <summary>
        /// Get icon by settings. <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/shellapi/nf-shellapi-shgetfileinfoa"/> .
        /// <br />Using sourceRect = Int32Rect.Empty; options = BitmapSizeOptions.FromEmptyOptions().
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid.</param>
        /// <param name="dwFileAttributes">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="uflags">The flags that specify the file information to retrieve. This parameter can be a combination of the following values.</param>
        /// <returns></returns>
        internal static BitmapSource GetFromHIcon(string pszPath, uint dwFileAttributes, uint uflags)
        {
            if (SHGetFileInfo(pszPath, dwFileAttributes, out SHFILEINFO fi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), uflags) != 0)
            {
                return Imaging.CreateBitmapSourceFromHIcon(fi.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            return null;
        }

        /// <summary>
        /// Get icon by settings. <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/shellapi/nf-shellapi-shgetfileinfoa"/> .
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid.</param>
        /// <param name="dwFileAttributes">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="uflags">The flags that specify the file information to retrieve. This parameter can be a combination of the following values.</param>
        /// <param name="sourceRect">The size of the source image. Can be Int32Rect.Empty,</param>
        /// <param name="options">A value of the enumeration that specifies how to handle conversions. Can be BitmapSizeOptions.FromEmptyOptions().</param>
        /// <returns></returns>
        internal static BitmapSource GetFromHIcon(string pszPath, uint dwFileAttributes, uint uflags, Int32Rect sourceRect, BitmapSizeOptions options)
        {
            if (SHGetFileInfo(pszPath, dwFileAttributes, out SHFILEINFO fi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), uflags) != 0)
            {
                return Imaging.CreateBitmapSourceFromHIcon(fi.hIcon, sourceRect, options);
            }
            return null;
        }

        /// <summary>
        /// Get image settings.
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid.</param>
        /// <param name="dwFileAttributes">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="uflags">The flags that specify the file information to retrieve. This parameter can be a combination of the following values.</param>
        /// <returns></returns>
        internal static SHFILEINFO? GetSHFILEINFO(string pszPath, uint dwFileAttributes, uint uflags)
        {
            if (SHGetFileInfo(pszPath, dwFileAttributes, out SHFILEINFO fi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), uflags) != 0)
            {
                return fi;
            }
            return null;
        }

        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uflags);
    }
}