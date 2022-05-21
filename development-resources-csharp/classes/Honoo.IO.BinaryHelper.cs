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

namespace Honoo.IO
{
    /// <summary>
    /// 二进制对象辅助。
    /// </summary>
    public static class BinaryHelper
    {
        #region 比较

        /// <summary>
        /// 比较字节数组。
        /// </summary>
        /// <param name="bytesA"></param>
        /// <param name="bytesB"></param>
        /// <returns></returns>
        public static bool Compare(byte[] bytesA, byte[] bytesB)
        {
            if (bytesA.Length == bytesB.Length)
            {
                for (int i = 0; i < bytesA.Length; i++)
                {
                    if (bytesA[i] != bytesB[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 比较字节数组。
        /// </summary>
        /// <param name="bytesA"></param>
        /// <param name="bytesB"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool Compare(byte[] bytesA, byte[] bytesB, int length)
        {
            if (bytesA.Length >= length && bytesB.Length >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    if (bytesA[i] != bytesB[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 比较字节数组。
        /// </summary>
        /// <param name="bytesA"></param>
        /// <param name="offsetA"></param>
        /// <param name="bytesB"></param>
        /// <param name="offsetB"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool Compare(byte[] bytesA, int offsetA, byte[] bytesB, int offsetB, int length)
        {
            if (bytesA.Length - offsetA >= length && bytesB.Length - offsetB >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    if (bytesA[offsetA + i] != bytesB[offsetB + i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion 比较

        #region 转换

        /// <summary>
        /// 将十六进制字符串转换为字节数组。字符串必须是无分隔符的表示形式。
        /// </summary>
        /// <param name="hex">无分隔符的十六进制字符串。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GetHexBytes(string hex)
        {
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
            return GetHexBytes(hex.Replace(remove, string.Empty));
        }

        #endregion 转换
    }
}