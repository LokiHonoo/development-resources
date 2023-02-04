/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Text;

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

        /// <summary>
        /// 将字节数组转换为十六进制文本。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <param name="indent">指定每行缩进的占位字符。</param>
        /// <param name="split">指定每个字节之间的分隔符。</param>
        /// <param name="lineBreaks">达到指定的字符个数后换行。设置为 0 不换行。</param>
        /// <returns></returns>
        public static string GetHexString(byte[] bytes, string indent, string split, int lineBreaks)
        {
            StringBuilder result = new StringBuilder();
            int count = 0;
            if (bytes.Length > 0)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (count == 0)
                    {
                        result.Append(indent);
                    }
                    else if (lineBreaks > 0 && count >= lineBreaks)
                    {
                        result.Append(Environment.NewLine);
                        result.Append(indent);
                        count = 0;
                    }
                    result.Append(bytes[i].ToString("x2"));
                    result.Append(split);
                    count++;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 指定字节数组大小端，根据字节数组长度转换为对应的数值，并以 Int32 类型输出。
        /// </summary>
        /// <param name="bytes">字节数组。</param>
        /// <param name="bigEndian">指定大小端。</param>
        /// <returns></returns>
        public static int GetInteger(byte[] bytes, bool bigEndian)
        {
            return GetInteger(bytes, 0, bytes.Length, bigEndian);
        }

        /// <summary>
        /// 指定字节数组大小端，根据字节数组长度转换为对应的数值，并以 Int32 类型输出。
        /// </summary>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数量。</param>
        /// <param name="bigEndian">指定大小端。</param>
        /// <returns></returns>
        public static int GetInteger(byte[] buffer, int offset, int length, bool bigEndian)
        {
            if (length == 4)
            {
                int result;
                if (bigEndian)
                {
                    result = buffer[offset] & 0xFF;
                    result |= (buffer[offset + 1] & 0xFF) << 8;
                    result |= (buffer[offset + 2] & 0xFF) << 16;
                    result |= (buffer[offset + 3] & 0xFF) << 24;
                }
                else
                {
                    result = (buffer[offset] & 0xFF) << 24;
                    result |= (buffer[offset + 1] & 0xFF) << 16;
                    result |= (buffer[offset + 2] & 0xFF) << 8;
                    result |= buffer[offset + 3] & 0xFF;
                }
                return result;
            }
            else if (length == 3)
            {
                int result;
                if (bigEndian)
                {
                    result = buffer[offset] & 0xFF;
                    result |= (buffer[offset + 1] & 0xFF) << 8;
                    result |= (buffer[offset + 2] & 0xFF) << 16;
                }
                else
                {
                    result = (buffer[offset] & 0xFF) << 16;
                    result |= (buffer[offset + 1] & 0xFF) << 8;
                    result |= buffer[offset + 2] & 0xFF;
                }
                return result;
            }
            else if (length == 2)
            {
                int result;
                if (bigEndian)
                {
                    result = buffer[offset] & 0xFF;
                    result |= (buffer[offset + 1] & 0xFF) << 8;
                }
                else
                {
                    result = (buffer[offset] & 0xFF) << 8;
                    result |= buffer[offset + 1] & 0xFF;
                }
                return result;
            }
            else
            {
                return buffer[offset] & 0xFF;
            }
        }

        #endregion 转换
    }
}