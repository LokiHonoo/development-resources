/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.IO;
using System.IO.Compression;

namespace Honoo.IO.Compression
{
    /// <summary>
    /// 压缩算法辅助。
    /// </summary>
    public static class CompressionHelper
    {
        /*
         * NET40 不支持设置压缩级，默认使用 Optimal。
         */

        /// <summary>
        /// 使用 DeflateStream 压缩字节数组。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] DeflateCompress(byte[] bytes)
        {
            return GZipCompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 使用 DeflateStream 压缩字节数组的指定区段。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <param name="offset">字节数组的偏移量。</param>
        /// <param name="count">要压缩的字节个数。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] DeflateCompress(byte[] bytes, int offset, int count)
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
        public static byte[] DeflateDecompress(byte[] bytes)
        {
            return GZipDecompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 使用 Deflate 解压缩字节数组。
        /// </summary>
        /// <param name="bytes">使用 Deflate 压缩过的字节数组。</param>
        /// <param name="offset">字节数组的偏移量。</param>
        /// <param name="count">要解压的字节个数。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] DeflateDecompress(byte[] bytes, int offset, int count)
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

        /// <summary>
        /// 使用 GZip 压缩字节数组。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GZipCompress(byte[] bytes)
        {
            return GZipCompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 使用 GZip 压缩字节数组的指定区段。
        /// </summary>
        /// <param name="bytes">要压缩的字节数组。</param>
        /// <param name="offset">字节数组的偏移量。</param>
        /// <param name="count">要压缩的字节个数。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GZipCompress(byte[] bytes, int offset, int count)
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
        public static byte[] GZipDecompress(byte[] bytes)
        {
            return GZipDecompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 使用 GZip 解压缩字节数组。
        /// </summary>
        /// <param name="bytes">使用 GZip 压缩过的字节数组。</param>
        /// <param name="offset">字节数组的偏移量。</param>
        /// <param name="count">要解压的字节个数。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GZipDecompress(byte[] bytes, int offset, int count)
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