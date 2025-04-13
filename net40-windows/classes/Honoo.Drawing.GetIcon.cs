using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Honoo.Drawing
{
    /// <summary>
    /// SH file info.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHFILEINFO : IEquatable<SHFILEINFO>
    {
        /// <summary>
        ///
        /// </summary>
        internal IntPtr hIcon;

        /// <summary>
        ///
        /// </summary>
        internal int iIcon;

        /// <summary>
        ///
        /// </summary>
        internal uint dwAttributes;

        /// <summary>
        ///
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string szDisplayName;

        /// <summary>
        ///
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        internal string szTypeName;

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is SHFILEINFO s && this.hIcon.Equals(s.hIcon);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(SHFILEINFO left, SHFILEINFO right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(SHFILEINFO left, SHFILEINFO right)
        {
            return !(left == right);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SHFILEINFO other)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_READONLY = 0x00000001;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint FILE_ATTRIBUTE_VIRTUAL = 0x00010000;

        /// <summary>
        /// Get only specified attributes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_ATTR_SPECIFIED = 0x000020000;

        /// <summary>
        /// Get attributes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_ATTRIBUTES = 0x000000800;

        /// <summary>
        /// Get display name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_DISPLAYNAME = 0x000000200;

        /// <summary>
        /// Return exe type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_EXETYPE = 0x000002000;

        /// <summary>
        /// Get icon.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_ICON = 0x000000100;

        /// <summary>
        /// Get icon location.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_ICONLOCATION = 0x000001000;

        /// <summary>
        /// Get large icon.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_LARGEICON = 0x000000000;

        /// <summary>
        /// Put a link overlay on icon.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_LINKOVERLAY = 0x000008000;

        /// <summary>
        /// Get open icon.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_OPENICON = 0x000000002;

        /// <summary>
        /// pszPath is a pidl.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_PIDL = 0x000000008;

        /// <summary>
        /// Get icon in selected state.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_SELECTED = 0x000010000;

        /// <summary>
        /// Get shell size icon.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_SHELLICONSIZE = 0x000000004;

        /// <summary>
        /// Get small icon.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_SMALLICON = 0x000000001;

        /// <summary>
        /// Get system icon index.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_SYSICONINDEX = 0x000004000;

        /// <summary>
        /// Get type name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_TYPENAME = 0x000000400;

        /// <summary>
        /// Use passed dwFileAttribute. Takes the file name and attributes into account if it doesn't exist.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
        internal const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        /// <summary>
        /// Get icon by settings. Icon = Honoo.Drawing.GetIcon(path, Honoo.Drawing.SHGFI_ICON | Honoo.Drawing.SHGFI_USEFILEATTRIBUTES, Honoo.Drawing.FILE_ATTRIBUTE_NORMAL)
        /// <br/>https://learn.microsoft.com/zh-cn/windows/win32/api/shellapi/nf-shellapi-shgetfileinfoa
        /// </summary>
        /// <param name="pszPath"></param>
        /// <param name="dwFileAttributes"></param>
        /// <param name="uflags"></param>
        /// <returns></returns>
        public static SHFILEINFO? GetSHFILEINFO(string pszPath, uint dwFileAttributes, uint uflags)
        {
            if (SHGetFileInfo(pszPath, dwFileAttributes, out SHFILEINFO fi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), uflags) != 0)
            {
                return fi;
            }
            return null;
        }

        /// <summary>
        /// Get icon by settings. Icon = Honoo.Drawing.GetIcon(path, Honoo.Drawing.SHGFI_ICON | Honoo.Drawing.SHGFI_USEFILEATTRIBUTES, Honoo.Drawing.FILE_ATTRIBUTE_NORMAL)
        /// <br/>https://learn.microsoft.com/zh-cn/windows/win32/api/shellapi/nf-shellapi-shgetfileinfoa
        /// </summary>
        /// <param name="pszPath"></param>
        /// <returns></returns>
        internal static ImageSource GetIcon(string pszPath)
        {
            uint uflags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            uint dwFileAttributes = FILE_ATTRIBUTE_NORMAL;
            if (SHGetFileInfo(pszPath, dwFileAttributes, out SHFILEINFO fi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), uflags) != 0)
            {
                return Imaging.CreateBitmapSourceFromHIcon(fi.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            return null;
        }

        [DllImport("shell32")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA2101:指定对 P/Invoke 字符串参数进行封送处理", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5392:对 P/Invoke 使用 DefaultDllImportSearchPaths 属性", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "SYSLIB1054:使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码", Justification = "<挂起>")]
        private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uflags);
    }
}