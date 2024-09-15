/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System;
using System.Globalization;
using System.Text;

namespace Honoo
{
    /// <summary>
    /// 二进制对象辅助。
    /// </summary>
    public static class Binaries
    {
        #region 比较

        /// <summary>
        /// 比较字节数组。
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static bool SequenceEqual(byte[] first, byte[] second)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            if (first.Length == second.Length)
            {
                for (int i = 0; i < first.Length; i++)
                {
                    if (first[i] != second[i])
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
        /// <param name="first"></param>
        /// <param name="firstOffset"></param>
        /// <param name="second"></param>
        /// <param name="secondOffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static bool SequenceEqual(byte[] first, int firstOffset, byte[] second, int secondOffset, int length)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second is null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            if (first.Length - firstOffset >= length && second.Length - secondOffset >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    if (first[firstOffset + i] != second[secondOffset + i])
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
        /// 指定输出字节数组大小端，将十六进制字符串转换为字节数组。字符串必须是无分隔符的表示形式。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="hex">无分隔符的十六进制字符串。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GetBytes(bool littleEndian, string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException($"“{nameof(hex)}”can't be null or empty.", nameof(hex));
            }

            if (hex.Length % 2 > 0)
            {
                throw new ArgumentException("Hex string length must be multiple of 2.");
            }
            byte[] result = new byte[hex.Length / 2];
            if (littleEndian)
            {
                for (int i = result.Length - 1; i >= 0; i--)
                {
                    result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
            }
            return result;
        }

        /// <summary>
        /// 指定输出字节数组大小端，移除指定的字符串，将十六进制字符串转换为字节数组。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="hex">十六进制字符串。</param>
        /// <param name="replace">要移除的字符串。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GetBytes(bool littleEndian, string hex, string replace)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException($"“{nameof(hex)}”can't be null or empty.", nameof(hex));
            }

            return GetBytes(littleEndian, hex.Replace(replace, string.Empty));
        }

        /// <summary>
        /// 指定输出字节数组大小端，移除多个指定的字符串，将十六进制字符串转换为字节数组。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="hex">十六进制字符串。</param>
        /// <param name="replaces">要移除的字符串集合。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GetBytes(bool littleEndian, string hex, string[] replaces)
        {
            if (string.IsNullOrEmpty(hex))
            {
                throw new ArgumentException($"“{nameof(hex)}”can't be null or empty.", nameof(hex));
            }
            if (replaces is null)
            {
                throw new ArgumentNullException(nameof(replaces));
            }

            foreach (var replace in replaces)
            {
                hex = hex.Replace(replace, string.Empty);
            }
            return GetBytes(littleEndian, hex);
        }

        /// <summary>
        /// 指定输出字节数组大小端，将 Int16 类型转换为字节数组。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="value">要转换的值。</param>
        /// <returns></returns>
        public static byte[] GetBytes(bool littleEndian, short value)
        {
            byte[] result = new byte[2];
            if (littleEndian)
            {
                result[0] = (byte)value;
                result[1] = (byte)(value >> 8);
            }
            else
            {
                result[0] = (byte)(value >> 8);
                result[1] = (byte)value;
            }
            return result;
        }

        /// <summary>
        /// 指定输出字节数组大小端，将 UInt16 类型转换为字节数组。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="value">要转换的值。</param>
        /// <returns></returns>
        public static byte[] GetBytes(bool littleEndian, ushort value)
        {
            byte[] result = new byte[2];
            if (littleEndian)
            {
                result[0] = (byte)value;
                result[1] = (byte)(value >> 8);
            }
            else
            {
                result[0] = (byte)(value >> 8);
                result[1] = (byte)value;
            }
            return result;
        }

        /// <summary>
        /// 指定输出字节数组大小端，将 Int32 类型转换为字节数组。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="value">要转换的值。</param>
        /// <returns></returns>
        public static byte[] GetBytes(bool littleEndian, int value)
        {
            byte[] result = new byte[4];
            if (littleEndian)
            {
                result[0] = (byte)value;
                result[1] = (byte)(value >> 8);
                result[2] = (byte)(value >> 16);
                result[3] = (byte)(value >> 24);
            }
            else
            {
                result[0] = (byte)(value >> 24);
                result[1] = (byte)(value >> 16);
                result[2] = (byte)(value >> 8);
                result[3] = (byte)value;
            }
            return result;
        }

        /// <summary>
        /// 指定输出字节数组大小端，将 UInt32 类型转换为字节数组。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="value">要转换的值。</param>
        /// <returns></returns>
        public static byte[] GetBytes(bool littleEndian, uint value)
        {
            byte[] result = new byte[4];
            if (littleEndian)
            {
                result[0] = (byte)value;
                result[1] = (byte)(value >> 8);
                result[2] = (byte)(value >> 16);
                result[3] = (byte)(value >> 24);
            }
            else
            {
                result[0] = (byte)(value >> 24);
                result[1] = (byte)(value >> 16);
                result[2] = (byte)(value >> 8);
                result[3] = (byte)value;
            }
            return result;
        }

        /// <summary>
        /// 指定输出字节数组大小端，将 Int64 类型转换为字节数组。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="value">要转换的值。</param>
        /// <returns></returns>
        public static byte[] GetBytes(bool littleEndian, long value)
        {
            byte[] result = new byte[8];
            if (littleEndian)
            {
                result[0] = (byte)value;
                result[1] = (byte)(value >> 8);
                result[2] = (byte)(value >> 16);
                result[3] = (byte)(value >> 24);
                result[4] = (byte)(value >> 32);
                result[5] = (byte)(value >> 40);
                result[6] = (byte)(value >> 48);
                result[7] = (byte)(value >> 56);
            }
            else
            {
                result[0] = (byte)(value >> 56);
                result[1] = (byte)(value >> 48);
                result[2] = (byte)(value >> 40);
                result[3] = (byte)(value >> 32);
                result[4] = (byte)(value >> 24);
                result[5] = (byte)(value >> 16);
                result[6] = (byte)(value >> 8);
                result[7] = (byte)value;
            }
            return result;
        }

        /// <summary>
        /// 指定输出字节数组大小端，将 UInt64 类型转换为字节数组。
        /// </summary>
        /// <param name="littleEndian">指示输出字节数组的大小端模式。</param>
        /// <param name="value">要转换的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static byte[] GetBytes(bool littleEndian, ulong value)
        {
            byte[] result = new byte[8];
            if (littleEndian)
            {
                result[0] = (byte)value;
                result[1] = (byte)(value >> 8);
                result[2] = (byte)(value >> 16);
                result[3] = (byte)(value >> 24);
                result[4] = (byte)(value >> 32);
                result[5] = (byte)(value >> 40);
                result[6] = (byte)(value >> 48);
                result[7] = (byte)(value >> 56);
            }
            else
            {
                result[0] = (byte)(value >> 56);
                result[1] = (byte)(value >> 48);
                result[2] = (byte)(value >> 40);
                result[3] = (byte)(value >> 32);
                result[4] = (byte)(value >> 24);
                result[5] = (byte)(value >> 16);
                result[6] = (byte)(value >> 8);
                result[7] = (byte)value;
            }
            return result;
        }

        /// <summary>
        /// 指定输入字节数组大小端，将字节数组转换为十六进制文本。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="bytes"/> 的大小端模式。</param>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static string ToHex(bool littleEndian, byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return ToHex(littleEndian, bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 指定输入字节数组大小端，将字节数组转换为十六进制文本。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="buffer"/> 的大小端模式。</param>
        /// <param name="buffer">要转换的字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数组长度。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static string ToHex(bool littleEndian, byte[] buffer, int offset, int length)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            StringBuilder result = new StringBuilder();
            int end = offset + length;
            if (littleEndian)
            {
                for (int i = end - 1; i >= offset; i--)
                {
                    result.Append(buffer[i].ToString("x2", CultureInfo.InvariantCulture));
                }
            }
            else
            {
                for (int i = offset; i < end; i++)
                {
                    result.Append(buffer[i].ToString("x2", CultureInfo.InvariantCulture));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 指定输入字节数组大小端，根据可读取的字节数组长度（最大 2 字节）转换为对应的数值，并以 Int16 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="bytes"/> 的大小端模式。</param>
        /// <param name="bytes">字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static short ToInt16(bool littleEndian, byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return ToInt16(littleEndian, bytes, 0, Math.Min(bytes.Length, 2));
        }

        /// <summary>
        /// 指定输入字节数组大小端，指定要读取的字节数组长度（最大 2 字节）转换为对应的数值，并以 Int16 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="buffer"/> 的大小端模式。</param>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数量。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static short ToInt16(bool littleEndian, byte[] buffer, int offset, int length)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (length <= 0 || length > 2)
            {
                throw new ArgumentException("Input length must be between 1 - 2.");
            }
            int result;
            if (length == 2)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFF;
                    result |= (buffer[offset + 1] & 0xFF) << 8;
                }
                else
                {
                    result = (buffer[offset] & 0xFF) << 8;
                    result |= buffer[offset + 1] & 0xFF;
                }
            }
            else
            {
                result = buffer[offset] & 0xFF;
            }
            return (short)result;
        }

        /// <summary>
        /// 指定输入字节数组大小端，根据可读取的字节数组长度（最大 4 字节）转换为对应的数值，并以 Int32 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="bytes"/> 的大小端模式。</param>
        /// <param name="bytes">字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static int ToInt32(bool littleEndian, byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return ToInt32(littleEndian, bytes, 0, Math.Min(bytes.Length, 4));
        }

        /// <summary>
        /// 指定输入字节数组大小端，指定要读取的字节数组长度（最大 4 字节）转换为对应的数值，并以 Int32 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="buffer"/> 的大小端模式。</param>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数量。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static int ToInt32(bool littleEndian, byte[] buffer, int offset, int length)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (length <= 0 || length > 4)
            {
                throw new ArgumentException("Input length must be between 1 - 4.");
            }
            int result;
            if (length == 4)
            {
                if (littleEndian)
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
            }
            else if (length == 3)
            {
                if (littleEndian)
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
            }
            else if (length == 2)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFF;
                    result |= (buffer[offset + 1] & 0xFF) << 8;
                }
                else
                {
                    result = (buffer[offset] & 0xFF) << 8;
                    result |= buffer[offset + 1] & 0xFF;
                }
            }
            else
            {
                result = buffer[offset] & 0xFF;
            }
            return result;
        }

        /// <summary>
        /// 指定输入字节数组大小端，根据可读取的字节数组长度（最大 8 字节）转换为对应的数值，并以 Int64 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="bytes"/> 的大小端模式。</param>
        /// <param name="bytes">字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static long ToInt64(bool littleEndian, byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return ToInt64(littleEndian, bytes, 0, Math.Min(bytes.Length, 8));
        }

        /// <summary>
        /// 指定输入字节数组大小端，指定要读取的字节数组长度（最大 8 字节）转换为对应的数值，并以 Int64 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="buffer"/> 的大小端模式。</param>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数量。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static long ToInt64(bool littleEndian, byte[] buffer, int offset, int length)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (length <= 0 || length > 8)
            {
                throw new ArgumentException("Input length must be between 1 - 8.");
            }
            long result;
            if (length == 8)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFL;
                    result |= (buffer[offset + 1] & 0xFFL) << 8;
                    result |= (buffer[offset + 2] & 0xFFL) << 16;
                    result |= (buffer[offset + 3] & 0xFFL) << 24;
                    result |= (buffer[offset + 4] & 0xFFL) << 32;
                    result |= (buffer[offset + 5] & 0xFFL) << 40;
                    result |= (buffer[offset + 6] & 0xFFL) << 48;
                    result |= (buffer[offset + 7] & 0xFFL) << 56;
                }
                else
                {
                    result = (buffer[offset] & 0xFFL) << 56;
                    result |= (buffer[offset + 1] & 0xFFL) << 48;
                    result |= (buffer[offset + 2] & 0xFFL) << 40;
                    result |= (buffer[offset + 3] & 0xFFL) << 32;
                    result |= (buffer[offset + 4] & 0xFFL) << 24;
                    result |= (buffer[offset + 5] & 0xFFL) << 16;
                    result |= (buffer[offset + 6] & 0xFFL) << 8;
                    result |= buffer[offset + 7] & 0xFFL;
                }
            }
            else if (length == 7)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFL;
                    result |= (buffer[offset + 1] & 0xFFL) << 8;
                    result |= (buffer[offset + 2] & 0xFFL) << 16;
                    result |= (buffer[offset + 3] & 0xFFL) << 24;
                    result |= (buffer[offset + 4] & 0xFFL) << 32;
                    result |= (buffer[offset + 5] & 0xFFL) << 40;
                    result |= (buffer[offset + 6] & 0xFFL) << 48;
                }
                else
                {
                    result = (buffer[offset] & 0xFFL) << 48;
                    result |= (buffer[offset + 1] & 0xFFL) << 40;
                    result |= (buffer[offset + 2] & 0xFFL) << 32;
                    result |= (buffer[offset + 3] & 0xFFL) << 24;
                    result |= (buffer[offset + 4] & 0xFFL) << 16;
                    result |= (buffer[offset + 5] & 0xFFL) << 8;
                    result |= buffer[offset + 6] & 0xFFL;
                }
            }
            else if (length == 6)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFL;
                    result |= (buffer[offset + 1] & 0xFFL) << 8;
                    result |= (buffer[offset + 2] & 0xFFL) << 16;
                    result |= (buffer[offset + 3] & 0xFFL) << 24;
                    result |= (buffer[offset + 4] & 0xFFL) << 32;
                    result |= (buffer[offset + 5] & 0xFFL) << 40;
                }
                else
                {
                    result = (buffer[offset] & 0xFFL) << 40;
                    result |= (buffer[offset + 1] & 0xFFL) << 32;
                    result |= (buffer[offset + 2] & 0xFFL) << 24;
                    result |= (buffer[offset + 3] & 0xFFL) << 16;
                    result |= (buffer[offset + 4] & 0xFFL) << 8;
                    result |= buffer[offset + 5] & 0xFFL;
                }
            }
            else if (length == 5)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFL;
                    result |= (buffer[offset + 1] & 0xFFL) << 8;
                    result |= (buffer[offset + 2] & 0xFFL) << 16;
                    result |= (buffer[offset + 3] & 0xFFL) << 24;
                    result |= (buffer[offset + 4] & 0xFFL) << 32;
                }
                else
                {
                    result = (buffer[offset] & 0xFFL) << 32;
                    result |= (buffer[offset + 1] & 0xFFL) << 24;
                    result |= (buffer[offset + 2] & 0xFFL) << 16;
                    result |= (buffer[offset + 3] & 0xFFL) << 8;
                    result |= buffer[offset + 4] & 0xFFL;
                }
            }
            else if (length == 4)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFL;
                    result |= (buffer[offset + 1] & 0xFFL) << 8;
                    result |= (buffer[offset + 2] & 0xFFL) << 16;
                    result |= (buffer[offset + 3] & 0xFFL) << 24;
                }
                else
                {
                    result = (buffer[offset] & 0xFFL) << 24;
                    result |= (buffer[offset + 1] & 0xFFL) << 16;
                    result |= (buffer[offset + 2] & 0xFFL) << 8;
                    result |= buffer[offset + 3] & 0xFFL;
                }
            }
            else if (length == 3)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFL;
                    result |= (buffer[offset + 1] & 0xFFL) << 8;
                    result |= (buffer[offset + 2] & 0xFFL) << 16;
                }
                else
                {
                    result = (buffer[offset] & 0xFFL) << 16;
                    result |= (buffer[offset + 1] & 0xFFL) << 8;
                    result |= buffer[offset + 2] & 0xFFL;
                }
            }
            else if (length == 2)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFL;
                    result |= (buffer[offset + 1] & 0xFFL) << 8;
                }
                else
                {
                    result = (buffer[offset] & 0xFFL) << 8;
                    result |= buffer[offset + 1] & 0xFFL;
                }
            }
            else
            {
                result = buffer[offset] & 0xFFL;
            }
            return result;
        }

        /// <summary>
        /// 将字节数组转换为十六进制文本。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static string ToString(byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return ToString(bytes, 0, bytes.Length, string.Empty, 0, string.Empty);
        }

        /// <summary>
        /// 将字节数组转换为十六进制文本。
        /// </summary>
        /// <param name="buffer">要转换的字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数组长度。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static string ToString(byte[] buffer, int offset, int length)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            return ToString(buffer, offset, length, string.Empty, 0, string.Empty);
        }

        /// <summary>
        /// 将字节数组转换为十六进制文本。
        /// </summary>
        /// <param name="buffer">要转换的字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数组长度。</param>
        /// <param name="split">指定每个字节之间的分隔符。</param>
        /// <param name="lineBreaks">转换指定的字节个数后换行。设置为 0 不换行。</param>
        /// <param name="indents">指定每行缩进的占位字符。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static string ToString(byte[] buffer, int offset, int length, string split, int lineBreaks, string indents)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            split = split ?? string.Empty;
            indents = indents ?? string.Empty;
            StringBuilder result = new StringBuilder();
            bool newLine = true;
            int count = 0;
            while (offset < length)
            {
                if (newLine)
                {
                    if (indents.Length > 0)
                    {
                        result.Append(indents);
                    }
                    newLine = false;
                }
                else if (lineBreaks > 0 && count >= lineBreaks)
                {
                    result.Append(Environment.NewLine);
                    newLine = true;
                    count = 0;
                }
                else
                {
                    result.Append(buffer[offset].ToString("x2", CultureInfo.InvariantCulture));
                    count++;
                    offset++;
                    if (split.Length > 0 && offset < length)
                    {
                        result.Append(split);
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 指定输入字节数组大小端，根据可读取的字节数组长度（最大 2 字节）转换为对应的数值，并以 Int16 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="bytes"/> 的大小端模式。</param>
        /// <param name="bytes">字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static ushort ToUInt16(bool littleEndian, byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return ToUInt16(littleEndian, bytes, 0, Math.Min(bytes.Length, 2));
        }

        /// <summary>
        /// 指定输入字节数组大小端，指定要读取的字节数组长度（最大 2 字节）转换为对应的数值，并以 UInt16 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="buffer"/> 的大小端模式。</param>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数量。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static ushort ToUInt16(bool littleEndian, byte[] buffer, int offset, int length)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (length <= 0 || length > 2)
            {
                throw new ArgumentException("Input length must be between 1 - 2.");
            }
            int result;
            if (length == 2)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFF;
                    result |= (buffer[offset + 1] & 0xFF) << 8;
                }
                else
                {
                    result = (buffer[offset] & 0xFF) << 8;
                    result |= buffer[offset + 1] & 0xFF;
                }
            }
            else
            {
                result = buffer[offset] & 0xFF;
            }
            return (ushort)result;
        }

        /// <summary>
        /// 指定输入字节数组大小端，根据可读取的字节数组长度（最大 4 字节）转换为对应的数值，并以 UInt32 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="bytes"/> 的大小端模式。</param>
        /// <param name="bytes">字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static uint ToUInt32(bool littleEndian, byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return ToUInt32(littleEndian, bytes, 0, Math.Min(bytes.Length, 4));
        }

        /// <summary>
        /// 指定输入字节数组大小端，指定要读取的字节数组长度（最大 4 字节）转换为对应的数值，并以 UInt32 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="buffer"/> 的大小端模式。</param>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数量。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static uint ToUInt32(bool littleEndian, byte[] buffer, int offset, int length)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (length <= 0 || length > 4)
            {
                throw new ArgumentException("Input length must be between 1 - 4.");
            }
            uint result;
            if (length == 4)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFU;
                    result |= (buffer[offset + 1] & 0xFFU) << 8;
                    result |= (buffer[offset + 2] & 0xFFU) << 16;
                    result |= (buffer[offset + 3] & 0xFFU) << 24;
                }
                else
                {
                    result = (buffer[offset] & 0xFFU) << 24;
                    result |= (buffer[offset + 1] & 0xFFU) << 16;
                    result |= (buffer[offset + 2] & 0xFFU) << 8;
                    result |= buffer[offset + 3] & 0xFFU;
                }
            }
            else if (length == 3)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFU;
                    result |= (buffer[offset + 1] & 0xFFU) << 8;
                    result |= (buffer[offset + 2] & 0xFFU) << 16;
                }
                else
                {
                    result = (buffer[offset] & 0xFFU) << 16;
                    result |= (buffer[offset + 1] & 0xFFU) << 8;
                    result |= buffer[offset + 2] & 0xFFU;
                }
            }
            else if (length == 2)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFU;
                    result |= (buffer[offset + 1] & 0xFFU) << 8;
                }
                else
                {
                    result = (buffer[offset] & 0xFFU) << 8;
                    result |= buffer[offset + 1] & 0xFFU;
                }
            }
            else
            {
                result = buffer[offset] & 0xFFU;
            }
            return result;
        }

        /// <summary>
        /// 指定字节数组大小端，根据可读取的字节数组长度（最大 8 字节）转换为对应的数值，并以 UInt64 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="bytes"/> 的大小端模式。</param>
        /// <param name="bytes">字节数组。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static ulong ToUInt64(bool littleEndian, byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return ToUInt64(littleEndian, bytes, 0, Math.Min(bytes.Length, 8));
        }

        /// <summary>
        /// 指定字节数组大小端，指定要读取的字节数组长度（最大 8 字节）转换为对应的数值，并以 UInt64 类型输出。
        /// </summary>
        /// <param name="littleEndian">指示 <paramref name="buffer"/> 的大小端模式。</param>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">读取的字节数组偏移量。</param>
        /// <param name="length">读取的字节数量。</param>
        /// <returns></returns>
        /// <exception cref="Exception" />
        public static ulong ToUInt64(bool littleEndian, byte[] buffer, int offset, int length)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (length <= 0 || length > 8)
            {
                throw new ArgumentException("Input length must be between 1 - 8.");
            }
            ulong result;
            if (length == 8)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFUL;
                    result |= (buffer[offset + 1] & 0xFFUL) << 8;
                    result |= (buffer[offset + 2] & 0xFFUL) << 16;
                    result |= (buffer[offset + 3] & 0xFFUL) << 24;
                    result |= (buffer[offset + 4] & 0xFFUL) << 32;
                    result |= (buffer[offset + 5] & 0xFFUL) << 40;
                    result |= (buffer[offset + 6] & 0xFFUL) << 48;
                    result |= (buffer[offset + 7] & 0xFFUL) << 56;
                }
                else
                {
                    result = (buffer[offset] & 0xFFUL) << 56;
                    result |= (buffer[offset + 1] & 0xFFUL) << 48;
                    result |= (buffer[offset + 2] & 0xFFUL) << 40;
                    result |= (buffer[offset + 3] & 0xFFUL) << 32;
                    result |= (buffer[offset + 4] & 0xFFUL) << 24;
                    result |= (buffer[offset + 5] & 0xFFUL) << 16;
                    result |= (buffer[offset + 6] & 0xFFUL) << 8;
                    result |= buffer[offset + 7] & 0xFFUL;
                }
            }
            else if (length == 7)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFUL;
                    result |= (buffer[offset + 1] & 0xFFUL) << 8;
                    result |= (buffer[offset + 2] & 0xFFUL) << 16;
                    result |= (buffer[offset + 3] & 0xFFUL) << 24;
                    result |= (buffer[offset + 4] & 0xFFUL) << 32;
                    result |= (buffer[offset + 5] & 0xFFUL) << 40;
                    result |= (buffer[offset + 6] & 0xFFUL) << 48;
                }
                else
                {
                    result = (buffer[offset] & 0xFFUL) << 48;
                    result |= (buffer[offset + 1] & 0xFFUL) << 40;
                    result |= (buffer[offset + 2] & 0xFFUL) << 32;
                    result |= (buffer[offset + 3] & 0xFFUL) << 24;
                    result |= (buffer[offset + 4] & 0xFFUL) << 16;
                    result |= (buffer[offset + 5] & 0xFFUL) << 8;
                    result |= buffer[offset + 6] & 0xFFUL;
                }
            }
            else if (length == 6)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFUL;
                    result |= (buffer[offset + 1] & 0xFFUL) << 8;
                    result |= (buffer[offset + 2] & 0xFFUL) << 16;
                    result |= (buffer[offset + 3] & 0xFFUL) << 24;
                    result |= (buffer[offset + 4] & 0xFFUL) << 32;
                    result |= (buffer[offset + 5] & 0xFFUL) << 40;
                }
                else
                {
                    result = (buffer[offset] & 0xFFUL) << 40;
                    result |= (buffer[offset + 1] & 0xFFUL) << 32;
                    result |= (buffer[offset + 2] & 0xFFUL) << 24;
                    result |= (buffer[offset + 3] & 0xFFUL) << 16;
                    result |= (buffer[offset + 4] & 0xFFUL) << 8;
                    result |= buffer[offset + 5] & 0xFFUL;
                }
            }
            else if (length == 5)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFUL;
                    result |= (buffer[offset + 1] & 0xFFUL) << 8;
                    result |= (buffer[offset + 2] & 0xFFUL) << 16;
                    result |= (buffer[offset + 3] & 0xFFUL) << 24;
                    result |= (buffer[offset + 4] & 0xFFUL) << 32;
                }
                else
                {
                    result = (buffer[offset] & 0xFFUL) << 32;
                    result |= (buffer[offset + 1] & 0xFFUL) << 24;
                    result |= (buffer[offset + 2] & 0xFFUL) << 16;
                    result |= (buffer[offset + 3] & 0xFFUL) << 8;
                    result |= buffer[offset + 4] & 0xFFUL;
                }
            }
            else if (length == 4)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFUL;
                    result |= (buffer[offset + 1] & 0xFFUL) << 8;
                    result |= (buffer[offset + 2] & 0xFFUL) << 16;
                    result |= (buffer[offset + 3] & 0xFFUL) << 24;
                }
                else
                {
                    result = (buffer[offset] & 0xFFUL) << 24;
                    result |= (buffer[offset + 1] & 0xFFUL) << 16;
                    result |= (buffer[offset + 2] & 0xFFUL) << 8;
                    result |= buffer[offset + 3] & 0xFFUL;
                }
            }
            else if (length == 3)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFUL;
                    result |= (buffer[offset + 1] & 0xFFUL) << 8;
                    result |= (buffer[offset + 2] & 0xFFUL) << 16;
                }
                else
                {
                    result = (buffer[offset] & 0xFFUL) << 16;
                    result |= (buffer[offset + 1] & 0xFFUL) << 8;
                    result |= buffer[offset + 2] & 0xFFUL;
                }
            }
            else if (length == 2)
            {
                if (littleEndian)
                {
                    result = buffer[offset] & 0xFFUL;
                    result |= (buffer[offset + 1] & 0xFFUL) << 8;
                }
                else
                {
                    result = (buffer[offset] & 0xFFUL) << 8;
                    result |= buffer[offset + 1] & 0xFFUL;
                }
            }
            else
            {
                result = buffer[offset] & 0xFFUL;
            }
            return result;
        }

        #endregion 转换
    }
}