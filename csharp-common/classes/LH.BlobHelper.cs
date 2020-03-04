/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.IO;
using System.IO.Compression;

namespace LH
{
    /// <summary>
    /// 二进制对象辅助。
    /// </summary>
    public static class BlobHelper
    {
        #region 压缩

        /// <summary>
        /// 使用 GZip 解压缩字节数组。
        /// </summary>
        /// <param name="bytes">使用 GZip 压缩过的字节数组。</param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return Decompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 使用 GZip 解压缩字节数组。
        /// </summary>
        /// <param name="bytes">使用 GZip 压缩过的字节数组。</param>
        /// <param name="offset">字节数组的偏移量。</param>
        /// <param name="count">要解压的字节个数。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static byte[] Decompress(byte[] bytes, int offset, int count)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(new MemoryStream(bytes, offset, count), CompressionMode.Decompress))
                {
                    gzip.CopyTo(ms);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 使用 GZip 压缩字节数组。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <returns></returns>
        internal static byte[] Compress(byte[] bytes)
        {
            return Compress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 使用 GZip 压缩字节数组的指定区段。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <param name="offset">字节数组的偏移量。</param>
        /// <param name="count">要压缩的字节个数。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static byte[] Compress(byte[] bytes, int offset, int count)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
                {
                    gzip.Write(bytes, offset, count);
                }
                return ms.ToArray();
            }
        }

        #endregion 压缩

        #region 转换

        /// <summary>
        /// 将十六进制字符串转换为字节数组。字符串必须是无分隔符的表示形式。
        /// </summary>
        /// <param name="hex">无分隔符的十六进制字符串。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GetHexBytes(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                throw new ArgumentNullException(nameof(hex));
            }
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return result;
        }

        /// <summary>
        /// 移除指定分隔符，将十六进制字符串转换为字节数组。
        /// </summary>
        /// <param name="hex">十六进制字符串。</param>
        /// <param name="remove">要移除的分隔符。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GetHexBytes(string hex, string remove)
        {
            if (string.IsNullOrEmpty(hex))
            {
                throw new ArgumentNullException(nameof(hex));
            }
            return GetHexBytes(hex.Replace(remove, string.Empty, StringComparison.InvariantCulture));
        }

        #endregion 转换
    }
}