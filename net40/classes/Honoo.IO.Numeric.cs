﻿/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System;

namespace Honoo.IO
{
    /// <summary>
    /// I/O 数值对象辅助。
    /// </summary>
    public static class Numeric
    {
        #region 转换

        /// <summary>
        /// 字节类型的容量单位。单位进位是 1024 字节。
        /// </summary>
        public enum SizeKilo
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary>单位是 B。</summary>
            B,

            /// <summary>单位是 KiB。</summary>
            KiB,

            /// <summary>单位是 MiB。</summary>
            MiB,

            /// <summary>单位是 GiB。</summary>
            GiB,

            /// <summary>单位是 TiB。</summary>
            TiB,

            /// <summary>单位是 PiB。</summary>
            PiB,

            /// <summary>单位是 EiB。</summary>
            EiB,

            /// <summary>单位是 BiB。</summary>
            BiB,
        }

        /// <summary>
        /// 字节类型的容量单位。单位进位是 1000 字节。
        /// </summary>
        public enum SizeThousands
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary>单位是 B。</summary>
            B,

            /// <summary>单位是 KB。</summary>
            KB,

            /// <summary>单位是 MB。</summary>
            MB,

            /// <summary>单位是 GB。</summary>
            GB,

            /// <summary>单位是 TB。</summary>
            TB,

            /// <summary>单位是 PB。</summary>
            PB,

            /// <summary>单位是 EB。</summary>
            EB,

            /// <summary>单位是 BB。</summary>
            BB,
        }

        /// <summary>
        /// 位类型的每秒速度单位。单位进位是 1000 位。
        /// </summary>
        public enum SpeedBits
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary>单位是 bps。</summary>
            bps,

            /// <summary>单位是 Kbps。</summary>
            Kbps,

            /// <summary>单位是 Mbps。</summary>
            Mbps,

            /// <summary>单位是 Gbps。</summary>
            Gbps,

            /// <summary>单位是 Tbps。</summary>
            Tbps,

            /// <summary>单位是 Pbps。</summary>
            Pbps,

            /// <summary>单位是 Ebps。</summary>
            Ebps,

            /// <summary>单位是 Bbps。</summary>
            Bbps,
        }

        /// <summary>
        /// 字节类型的每秒速度单位。单位进位是 1024 字节。
        /// </summary>
        public enum SpeedKilo
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary>单位是 B/s。</summary>
            Bps,

            /// <summary>单位是 KiB/s。</summary>
            KiBps,

            /// <summary>单位是 MiB/s。</summary>
            MiBps,

            /// <summary>单位是 GiB/s。</summary>
            GiBps,

            /// <summary>单位是 TiB/s。</summary>
            TiBps,

            /// <summary>单位是 PiB/s。</summary>
            PiBps,

            /// <summary>单位是 EiB/s。</summary>
            EiBps,

            /// <summary>单位是 BiB/s。</summary>
            BiBps,
        }

        /// <summary>
        /// 字节类型的每秒速度单位。单位进位是 1000 字节。
        /// </summary>
        public enum SpeedThousands
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary>单位是 B/s。</summary>
            Bps,

            /// <summary>单位是 KB/s。</summary>
            KBps,

            /// <summary>单位是 MB/s。</summary>
            MBps,

            /// <summary>单位是 GB/s。</summary>
            GBps,

            /// <summary>单位是 TB/s。</summary>
            TBps,

            /// <summary>单位是 PB/s。</summary>
            PBps,

            /// <summary>单位是 EB/s。</summary>
            EBps,

            /// <summary>单位是 BB/s。</summary>
            BBps,
        }

        /// <summary>
        /// 将字节容量数值转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="radix">字节类型的容量单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        /// <returns></returns>
        public static double GetSize(long byteLength, SizeKilo radix, int places, out string unit)
        {
            double value = byteLength;
            int unitIndex = 0;
            if (radix == SizeKilo.Auto)
            {
                while (value >= 1024 && unitIndex < 7)
                {
                    value /= 1024;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1024;
                    unitIndex++;
                }
            }
            switch (unitIndex)
            {
                case 7: unit = "BiB"; break;
                case 6: unit = "EiB"; break;
                case 5: unit = "PiB"; break;
                case 4: unit = "TiB"; break;
                case 3: unit = "GiB"; break;
                case 2: unit = "MiB"; break;
                case 1: unit = "KiB"; break;
                case 0: default: unit = "B"; break;
            }
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节容量数值转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="radix">字节类型的容量单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        /// <returns></returns>
        public static double GetSize(long byteLength, SizeThousands radix, int places, out string unit)
        {
            double value = byteLength;
            int unitIndex = 0;
            if (radix == SizeThousands.Auto)
            {
                while (value >= 1000 && unitIndex < 7)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            switch (unitIndex)
            {
                case 7: unit = "BB"; break;
                case 6: unit = "EB"; break;
                case 5: unit = "PB"; break;
                case 4: unit = "TB"; break;
                case 3: unit = "GB"; break;
                case 2: unit = "MB"; break;
                case 1: unit = "KB"; break;
                case 0: default: unit = "B"; break;
            }
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节速度数值转换为指定单位。
        /// </summary>
        /// <param name="bytesPerSecond">每秒字节数值。</param>
        /// <param name="radix">位类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long bytesPerSecond, SpeedBits radix, int places, out string unit)
        {
            double value = bytesPerSecond * 8;
            int unitIndex = 0;
            if (radix == SpeedBits.Auto)
            {
                while (value >= 1000 && unitIndex < 7)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            switch (unitIndex)
            {
                case 7: unit = "Bbps"; break;
                case 6: unit = "Ebps"; break;
                case 5: unit = "Pbps"; break;
                case 4: unit = "Tbps"; break;
                case 3: unit = "Gbps"; break;
                case 2: unit = "Mbps"; break;
                case 1: unit = "Kbps"; break;
                case 0: default: unit = "bps"; break;
            }
            return Math.Round(value, places);
        }

        /// <summary>
        /// 根据字节数值和处理时间计算并转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="durationSeconds">以秒为单位的处理时间。</param>
        /// <param name="radix">位类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long byteLength, double durationSeconds, SpeedBits radix, int places, out string unit)
        {
            return GetSpeed((long)(byteLength / durationSeconds), radix, places, out unit);
        }

        /// <summary>
        /// 将字节速度数值转换为指定单位。
        /// </summary>
        /// <param name="bytesPerSecond">每秒字节数值。</param>
        /// <param name="radix">字节类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long bytesPerSecond, SpeedKilo radix, int places, out string unit)
        {
            double value = bytesPerSecond;
            int unitIndex = 0;
            if (radix == SpeedKilo.Auto)
            {
                while (value >= 1024 && unitIndex < 7)
                {
                    value /= 1024;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1024;
                    unitIndex++;
                }
            }
            switch (unitIndex)
            {
                case 7: unit = "BiB/s"; break;
                case 6: unit = "EiB/s"; break;
                case 5: unit = "PiB/s"; break;
                case 4: unit = "TiB/s"; break;
                case 3: unit = "GiB/s"; break;
                case 2: unit = "MiB/s"; break;
                case 1: unit = "KiB/s"; break;
                case 0: default: unit = "B/s"; break;
            }
            return Math.Round(value, places);
        }

        /// <summary>
        /// 根据字节数值和处理时间计算并转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="durationSeconds">以秒为单位的处理时间。</param>
        /// <param name="radix">位类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long byteLength, double durationSeconds, SpeedKilo radix, int places, out string unit)
        {
            return GetSpeed((long)(byteLength / durationSeconds), radix, places, out unit);
        }

        /// <summary>
        /// 将字节速度数值转换为指定单位。
        /// </summary>
        /// <param name="bytesPerSecond">每秒字节数值。</param>
        /// <param name="radix">字节类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long bytesPerSecond, SpeedThousands radix, int places, out string unit)
        {
            double value = bytesPerSecond;
            int unitIndex = 0;
            if (radix == SpeedThousands.Auto)
            {
                while (value >= 1000 && unitIndex < 7)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            switch (unitIndex)
            {
                case 7: unit = "BB/s"; break;
                case 6: unit = "EB/s"; break;
                case 5: unit = "PB/s"; break;
                case 4: unit = "TB/s"; break;
                case 3: unit = "GB/s"; break;
                case 2: unit = "MB/s"; break;
                case 1: unit = "KB/s"; break;
                case 0: default: unit = "B/s"; break;
            }
            return Math.Round(value, places);
        }

        /// <summary>
        /// 根据字节数值和处理时间计算并转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="durationSeconds">以秒为单位的处理时间。</param>
        /// <param name="radix">位类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long byteLength, double durationSeconds, SpeedThousands radix, int places, out string unit)
        {
            return GetSpeed((long)(byteLength / durationSeconds), radix, places, out unit);
        }

        #endregion 转换
    }
}