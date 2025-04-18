﻿/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

/*
 * NET40 不支持设置压缩级，默认使用 Optimal。
 */

using System;
using System.IO;
using System.IO.Compression;

namespace Honoo.IO.Compression
{
    /// <summary>
    /// Deflate 压缩算法辅助。
    /// </summary>
    internal static class Deflate
    {
        /// <summary>
        /// 使用 DeflateStream 压缩字节数组。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        internal static byte[] Compress(byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return Compress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 使用 DeflateStream 压缩字节数组的指定区段。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <param name="offset">字节数组的偏移量。</param>
        /// <param name="count">要压缩的字节个数。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        internal static byte[] Compress(byte[] bytes, int offset, int count)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream zip = new DeflateStream(ms, CompressionMode.Compress))
                {
                    zip.Write(bytes, offset, count);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 使用 Deflate 解压缩字节数组。
        /// </summary>
        /// <param name="bytes">使用 Deflate 压缩过的字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        internal static byte[] Decompress(byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return Decompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 使用 Deflate 解压缩字节数组。
        /// </summary>
        /// <param name="bytes">使用 Deflate 压缩过的字节数组。</param>
        /// <param name="offset">字节数组的偏移量。</param>
        /// <param name="count">要解压的字节个数。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        internal static byte[] Decompress(byte[] bytes, int offset, int count)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream zip = new DeflateStream(new MemoryStream(bytes, offset, count), CompressionMode.Decompress))
                {
                    zip.CopyTo(ms);
                }
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// GZip 压缩算法辅助。
    /// </summary>
    internal static class GZip
    {
        /// <summary>
        /// 使用 GZip 压缩字节数组。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        internal static byte[] Compress(byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
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
        internal static byte[] Compress(byte[] bytes, int offset, int count)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress))
                {
                    zip.Write(bytes, offset, count);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 使用 GZip 解压缩字节数组。
        /// </summary>
        /// <param name="bytes">使用 GZip 压缩过的字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        internal static byte[] Decompress(byte[] bytes)
        {
            if (bytes is null)
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
        internal static byte[] Decompress(byte[] bytes, int offset, int count)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(new MemoryStream(bytes, offset, count), CompressionMode.Decompress))
                {
                    zip.CopyTo(ms);
                }
                return ms.ToArray();
            }
        }
    }
}