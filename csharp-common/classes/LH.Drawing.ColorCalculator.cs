/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

/*
 * Observer.CIE1964 转换参照常量没有找到权威的标准值，可能存在错误
 */

using System;
using System.Diagnostics.CodeAnalysis;

namespace LH.Drawing
{
    /// <summary>
    /// RGB/XYZ 调整器。
    /// </summary>
    internal enum Adaptation
    {
        None,
        Bradford,
        VonKries,
        XYZScaling
    }

    /// <summary>
    /// 光源。
    /// </summary>
    internal enum Illuminant
    {
        A,
        B,
        C,
        D50,
        D55,
        D65,
        D75,
        E,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12
    }

    /// <summary>
    /// 观测角度。
    /// </summary>
    internal enum Observer
    {
        /// <summary>CIE1931 标准。角度 2°。</summary>
        CIE1931,

        /// <summary>CIE1964 标准。角度 10°。</summary>
        CIE1964
    }

    /// <summary>
    /// RGB 模型。
    /// </summary>
    [SuppressMessage("Naming", "CA1707:标识符不应包含下划线", Justification = "<挂起>")]
    internal enum RGBModel
    {
        Adobe1998,
        Apple,
        Best,
        Beta,
        Bruce,
        CIE,
        ColorMatch,
        Don4,
        ECIv2,
        EktaSpacePS5,
        NTSC,
        PAL_SECAM,
        ProPhoto,
        SMPTE_C,
        Standard,
        WideGamut
    }

    /// <summary>
    /// 颜色计算器。
    /// </summary>
    internal static class ColorCalculator
    {
        #region 成员

        /// <summary>
        /// 转换常量。
        /// </summary>
        private const double KE = 216d / 24389d;

        /// <summary>
        /// 转换常量。
        /// </summary>
        private const double KK = 24389d / 27d;

        /// <summary>
        /// 转换常量。
        /// </summary>
        private const double KKE = 8d;

        #endregion 成员

        /// <summary>
        /// CCT 转换到 XYZ。
        /// </summary>
        /// <param name="CCT">CCT 值。</param>
        /// <param name="reference">转换参照值。</param>
        /// <returns></returns>
        internal static double[] CCT2XYZ(double CCT, Reference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            double Temp = CCT;
            double C1 = 2d * Math.PI * 6.626176d * 2.99792458d * 2.99792458d;    // * 1.0e-18
            double C2 = 6.626176d * 2.99792458d / 1.380662d;    // * 1.0e-3
            double nm;

            double X = 0d;
            double Y = 0d;
            double Z = 0d;

            int i = 0;
            for (nm = 360; nm <= 830; nm += 5)
            {
                double dWavelengthM = nm * 1.0e-3; // * 1.0e-6
                double dWavelengthM5 = dWavelengthM * dWavelengthM * dWavelengthM * dWavelengthM * dWavelengthM;   // * 1.0e-30
                double blackbody = C1 / (dWavelengthM5 * 1.0e-12 * (Math.Exp(C2 / (Temp * dWavelengthM * 1.0e-3)) - 1d)); // -12 = -30 - (-18)

                double[,] so = reference.Observer == Observer.CIE1964 ? reference.CIE1964StandardObserver : reference.CIE1931StandardObserver;

                X += (blackbody * so[0, i]);
                Y += (blackbody * so[1, i]);
                Z += (blackbody * so[2, i]);
                i++;
            }
            X /= Y;
            Z /= Y;
            Y = 1d;
            return new double[3] { X, Y, Z };
        }

        /// <summary>
        /// 从 xyY 获取波长。如果无法转换，返回 0。
        /// </summary>
        /// <param name="xyY">xyY 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <param name="exception">错误信息。无法转换时输出错误原因。</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        internal static double DominantWavelength(double[] xyY, Reference reference, out Exception exception)
        {
            if (xyY == null)
            {
                throw new ArgumentNullException(nameof(xyY));
            }

            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            exception = null;

            double x = xyY[0];
            double y = xyY[1];
            double xr = reference.White[0] / (reference.White[0] + reference.White[1] + reference.White[2]);
            double yr = reference.White[1] / (reference.White[0] + reference.White[1] + reference.White[2]);

            int count = 0;
            double[] ts = new double[] { 0d, 0d };    // t
            double[] ws = new double[] { 0d, 0d };    // wavelength
            int[] cs = new int[] { 0, 0 };        // cycle

            double a = x - xr;
            double b = y - yr;

            if ((a >= -0.000001d) && (a <= 0.000001d) && (b >= -0.000001d) && (b <= 0.000001d))
            {
                exception = new Exception("(DominantWavelength01)x, y 值和参照值相同。");
                return 0d;   // cannot compute the dominant wavelength, because (x, y) is the same as (xr, yr)
            }

            int nm;

            for (nm = 360; nm <= 830; nm += 5)
            {
                int i1 = (nm - 360) / 5;
                int i2 = (nm == 830) ? 0 : i1 + 1;
                int nm2 = 5 * i2 + 360;

                double[,] so = reference.Observer == Observer.CIE1964 ? reference.CIE1964StandardObserver : reference.CIE1931StandardObserver;

                double x1 = so[0, i1] / (so[0, i1] + so[1, i1] + so[2, i1]);
                double y1 = so[1, i1] / (so[0, i1] + so[1, i1] + so[2, i1]);
                double x2 = so[0, i2] / (so[0, i2] + so[1, i2] + so[2, i2]);
                double y2 = so[1, i2] / (so[0, i2] + so[1, i2] + so[2, i2]);

                double c = x1 - xr;
                double d = y1 - yr;
                double e = x2 - x1;
                double f = y2 - y1;

                double s = (a * d - b * c) / (b * e - a * f);
                if ((s < 0d) || (s >= 1d))
                {
                    continue;
                }

                double t = (Math.Abs(a) >= Math.Abs(b)) ? ((e * s + c) / a) : ((f * s + d) / b);
                ts[count] = t;
                cs[count] = nm;
                ws[count] = (nm2 - nm) * s + nm;
                count += 1;
            }

            if ((cs[1] == 830) && (ts[1] > 0d))
            {
                return -ws[0];
            }
            else
            {
                return ts[0] >= 0d ? ws[0] : ws[1];
            }
        }

        /// <summary>
        /// Lab 转换到 LCHab。
        /// </summary>
        /// <param name="Lab">Lab 值的数组。</param>
        /// <returns></returns>
        internal static double[] Lab2LCHab(double[] Lab)
        {
            if (Lab == null)
            {
                throw new ArgumentNullException(nameof(Lab));
            }

            double L = Lab[0];
            double a = Lab[1];
            double b = Lab[2];
            double C = Math.Sqrt(a * a + b * b);
            double H = 180d * Math.Atan2(b, a) / Math.PI;
            if (H < 0d)
            {
                H += 360d;
            }
            return new double[3] { L, C, H };
        }

        /// <summary>
        /// Lab 转换到 XYZ。
        /// </summary>
        /// <param name="Lab">Lab 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <returns></returns>
        internal static double[] Lab2XYZ(double[] Lab, Reference reference)
        {
            if (Lab == null)
            {
                throw new ArgumentNullException(nameof(Lab));
            }

            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            double fY = (Lab[0] + 16d) / 116d;
            double fX = 0.002d * Lab[1] + fY;
            double fZ = fY - 0.005d * Lab[2];

            double fX3 = fX * fX * fX;
            double fZ3 = fZ * fZ * fZ;

            double X = fX3 > KE ? fX3 : ((116d * fX - 16d) / KK);
            double Y = Lab[0] > KKE ? Math.Pow((Lab[0] + 16d) / 116d, 3d) : Lab[0] / KK;
            double Z = fZ3 > KE ? fZ3 : (116d * fZ - 16d) / KK;

            return new double[3] { X * reference.White[0], Y * reference.White[1], Z * reference.White[2] };
        }

        /// <summary>
        /// LCHab 转换到 Lab。
        /// </summary>
        /// <param name="LCHab">LCHab 值的数组。</param>
        /// <returns></returns>
        internal static double[] LCHab2Lab(double[] LCHab)
        {
            if (LCHab == null)
            {
                throw new ArgumentNullException(nameof(LCHab));
            }

            return new double[3] { LCHab[0], LCHab[1] * Math.Cos(LCHab[2] * Math.PI / 180d), LCHab[1] * Math.Sin(LCHab[2] * Math.PI / 180d) };
        }

        /// <summary>
        /// LCHuv 转换到 Luv。
        /// </summary>
        /// <param name="LCHuv">LCHab 值的数组。</param>
        /// <returns></returns>
        internal static double[] LCHuv2Luv(double[] LCHuv)
        {
            if (LCHuv == null)
            {
                throw new ArgumentNullException(nameof(LCHuv));
            }

            return new double[3] { LCHuv[0], LCHuv[1] * Math.Cos(LCHuv[2] * Math.PI / 180d), LCHuv[1] * Math.Sin(LCHuv[2] * Math.PI / 180d) };
        }

        /// <summary>
        /// Luv 转换到 LCHuv。
        /// </summary>
        /// <param name="Luv">Luv 值的数组。</param>
        /// <returns></returns>
        internal static double[] Luv2LCHuv(double[] Luv)
        {
            if (Luv == null)
            {
                throw new ArgumentNullException(nameof(Luv));
            }

            double L = Luv[0];
            double u = Luv[1];
            double v = Luv[2];
            double C = Math.Sqrt(u * u + v * v);
            double H = 180d * Math.Atan2(v, u) / Math.PI;
            if (H < 0d)
            {
                H += 360d;
            }
            return new double[3] { L, C, H };
        }

        /// <summary>
        /// Luv 转换到 XYZ。
        /// </summary>
        /// <param name="Luv">Luv 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <returns></returns>
        internal static double[] Luv2XYZ(double[] Luv, Reference reference)
        {
            if (Luv == null)
            {
                throw new ArgumentNullException(nameof(Luv));
            }

            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            double L = Luv[0];
            double u = Luv[1];
            double v = Luv[2];

            double Y = L > KKE ? Math.Pow((L + 16d) / 116d, 3d) : L / KK;

            double rDen = reference.White[0] + 15d * reference.White[1] + 3d * reference.White[2];
            double urp = 4d * reference.White[0] / rDen;
            double vrp = 9d * reference.White[1] / rDen;

            double a = (((52d * L) / (u + 13d * L * urp)) - 1d) / 3d;
            double b = -5d * Y;
            double c = -1d / 3d;
            double d = Y * (((39d * L) / (v + 13d * L * vrp)) - 5d);

            double X = (d - b) / (a - c);
            double Z = X * a + b;

            return new double[3] { X, Y, Z };
        }

        /// <summary>
        /// RGB(0-255) 转换到 RGB(0-1.0)。
        /// </summary>
        /// <param name="RGB">RGB(0-255) 值的数组。</param>
        /// <returns></returns>
        internal static double[] RGB2RGB(byte[] RGB)
        {
            if (RGB == null)
            {
                throw new ArgumentNullException(nameof(RGB));
            }

            return new double[3] { RGB[0] / 255d, RGB[1] / 255d, RGB[2] / 255d };
        }

        /// <summary>
        /// RGB(0-1.0) 转换到 RGB(0-255)。
        /// </summary>
        /// <param name="RGB">RGB(0-1.0) 值的数组。</param>
        /// <returns></returns>
        internal static byte[] RGB2RGB(double[] RGB)
        {
            if (RGB == null)
            {
                throw new ArgumentNullException(nameof(RGB));
            }

            return new byte[3] {
                (byte)Math.Min(Math.Round(RGB[0] * 255d, 0), 255d),
                (byte)Math.Min(Math.Round(RGB[1] * 255d, 0), 255d),
                (byte)Math.Min(Math.Round(RGB[2] * 255d, 0), 255d)
            };
        }

        /// <summary>
        /// RGB(0-1.0) 转换到 XYZ。
        /// </summary>
        /// <param name="RGB">RGB(0-1.0) 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <returns></returns>
        internal static double[] RGB2XYZ(double[] RGB, Reference reference)
        {
            if (RGB == null)
            {
                throw new ArgumentNullException(nameof(RGB));
            }

            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            double R = GammaLinear(RGB[0], reference.Gamma);
            double G = GammaLinear(RGB[1], reference.Gamma);
            double B = GammaLinear(RGB[2], reference.Gamma);
            //
            double X = R * reference.MtxRGB2XYZ[0, 0] + G * reference.MtxRGB2XYZ[1, 0] + B * reference.MtxRGB2XYZ[2, 0];
            double Y = R * reference.MtxRGB2XYZ[0, 1] + G * reference.MtxRGB2XYZ[1, 1] + B * reference.MtxRGB2XYZ[2, 1];
            double Z = R * reference.MtxRGB2XYZ[0, 2] + G * reference.MtxRGB2XYZ[1, 2] + B * reference.MtxRGB2XYZ[2, 2];
            //
            if (reference.Adaptation != Adaptation.None)
            {
                double dX = reference.White[0] * reference.MtxAdaptation[0, 0] +
                            reference.White[1] * reference.MtxAdaptation[1, 0] +
                            reference.White[2] * reference.MtxAdaptation[2, 0];
                double dY = reference.White[0] * reference.MtxAdaptation[0, 1] +
                            reference.White[1] * reference.MtxAdaptation[1, 1] +
                            reference.White[2] * reference.MtxAdaptation[2, 1];
                double dZ = reference.White[0] * reference.MtxAdaptation[0, 2] +
                            reference.White[1] * reference.MtxAdaptation[1, 2] +
                            reference.White[2] * reference.MtxAdaptation[2, 2];

                double sX = reference.WhiteRGB[0] * reference.MtxAdaptation[0, 0] +
                            reference.WhiteRGB[1] * reference.MtxAdaptation[1, 0] +
                            reference.WhiteRGB[2] * reference.MtxAdaptation[2, 0];
                double sY = reference.WhiteRGB[0] * reference.MtxAdaptation[0, 1] +
                            reference.WhiteRGB[1] * reference.MtxAdaptation[1, 1] +
                            reference.WhiteRGB[2] * reference.MtxAdaptation[2, 1];
                double sZ = reference.WhiteRGB[0] * reference.MtxAdaptation[0, 2] +
                            reference.WhiteRGB[1] * reference.MtxAdaptation[1, 2] +
                            reference.WhiteRGB[2] * reference.MtxAdaptation[2, 2];

                double fX = X * reference.MtxAdaptation[0, 0] + Y * reference.MtxAdaptation[1, 0] + Z * reference.MtxAdaptation[2, 0];
                double fY = X * reference.MtxAdaptation[0, 1] + Y * reference.MtxAdaptation[1, 1] + Z * reference.MtxAdaptation[2, 1];
                double fZ = X * reference.MtxAdaptation[0, 2] + Y * reference.MtxAdaptation[1, 2] + Z * reference.MtxAdaptation[2, 2];

                fX *= dX / sX;
                fY *= dY / sY;
                fZ *= dZ / sZ;

                X = fX * reference.MtxAdaptationI[0, 0] + fY * reference.MtxAdaptationI[1, 0] + fZ * reference.MtxAdaptationI[2, 0];
                Y = fX * reference.MtxAdaptationI[0, 1] + fY * reference.MtxAdaptationI[1, 1] + fZ * reference.MtxAdaptationI[2, 1];
                Z = fX * reference.MtxAdaptationI[0, 2] + fY * reference.MtxAdaptationI[1, 2] + fZ * reference.MtxAdaptationI[2, 2];
            }
            //
            return new double[3] { X, Y, Z };
        }

        /// <summary>
        /// xyY 转换到 XYZ。
        /// </summary>
        /// <param name="xyY">xyY 值的数组。</param>
        /// <returns></returns>
        [SuppressMessage("样式", "IDE1006:命名样式", Justification = "<挂起>")]
        internal static double[] xyY2XYZ(double[] xyY)
        {
            if (xyY == null)
            {
                throw new ArgumentNullException(nameof(xyY));
            }

            if (xyY[1] < 0.000001d)
            {
                return new double[3] { 0d, 0d, 0d };
            }
            else
            {
                return new double[3] { xyY[0] * xyY[2] / xyY[1], xyY[2], (1d - xyY[0] - xyY[1]) * xyY[2] / xyY[1] };
            }
        }

        /// <summary>
        /// XYZ 转换到 CCT。如果无法转换，返回 0。
        /// </summary>
        /// <param name="XYZ">XYZ 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <param name="exception">错误信息。无法转换时输出错误原因。</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        internal static double XYZ2CCT(double[] XYZ, Reference reference, out Exception exception)
        {
            if (XYZ == null)
            {
                throw new ArgumentNullException(nameof(XYZ));
            }
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            exception = null;

            double us = 4d * XYZ[0] / (XYZ[0] + 15d * XYZ[1] + 3d * XYZ[2]);
            double vs = 6d * XYZ[1] / (XYZ[0] + 15d * XYZ[1] + 3d * XYZ[2]);
            double prevVertDist = 0d;
            double thisVertDist = 0d;

            int i;
            for (i = 0; i < 31; i++)
            {
                thisVertDist = (vs - reference.V[i]) - reference.T[i] * (us - reference.U[i]);
                if ((i == 0) && (thisVertDist <= 0d))
                {
                    exception = new Exception("(XYZ2CCT02)蓝色值过高。");
                    return 0d;
                }
                if ((i > 0) && (thisVertDist <= 0d))
                    break;  /* found lines bounding (us, vs) : i-1 and i */
                prevVertDist = thisVertDist;
            }
            if (i == 31)
            {
                exception = new Exception("(XYZ2CCT01)红色值过高。");
                return 0d;
            }
            else
            {
                double thisPerpDist = thisVertDist / Math.Sqrt(1d + reference.T[i] * reference.T[i]);
                double prevPerpDist = prevVertDist / Math.Sqrt(1d + reference.T[i - 1] * reference.T[i - 1]);
                double w = prevPerpDist / (prevPerpDist - thisPerpDist);       /* w = lerping parameter, 0 : i-1, 1 : i */
                return 1d / ((reference.RT[i] - reference.RT[i - 1]) * w + reference.RT[i - 1]);      /* 1.0 / (LERP(rt[i-1], rt[i], w)) */
            }
        }

        /// <summary>
        /// XYZ 转换到 Lab。
        /// </summary>
        /// <param name="XYZ">XYZ 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <returns></returns>
        internal static double[] XYZ2Lab(double[] XYZ, Reference reference)
        {
            if (XYZ == null)
            {
                throw new ArgumentNullException(nameof(XYZ));
            }
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            double X = XYZ[0] / reference.White[0];
            double Y = XYZ[1] / reference.White[1];
            double Z = XYZ[2] / reference.White[2];

            double fX = X > KE ? Math.Pow(X, 1d / 3d) : (KK * X + 16d) / 116d;
            double fY = Y > KE ? Math.Pow(Y, 1d / 3d) : (KK * Y + 16d) / 116d;
            double fZ = Z > KE ? Math.Pow(Z, 1d / 3d) : (KK * Z + 16d) / 116d;

            return new double[3] { 116d * fY - 16d, 500d * (fX - fY), 200d * (fY - fZ) };
        }

        /// <summary>
        /// XYZ 转换到 Luv。
        /// </summary>
        /// <param name="XYZ">XYZ 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <returns></returns>
        internal static double[] XYZ2Luv(double[] XYZ, Reference reference)
        {
            if (XYZ == null)
            {
                throw new ArgumentNullException(nameof(XYZ));
            }
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            double Den = XYZ[0] + 15d * XYZ[1] + 3d * XYZ[2];
            double up = Den > 0d ? 4d * XYZ[0] / Den : 0d;
            double vp = Den > 0d ? 9d * XYZ[1] / Den : 0d;

            double rDen = reference.White[0] + 15d * reference.White[1] + 3d * reference.White[2];
            double urp = 4d * reference.White[0] / rDen;
            double vrp = 9d * reference.White[1] / rDen;

            double yr = XYZ[1] / reference.White[1];

            double L = (yr > KE) ? (116d * Math.Pow(yr, 1d / 3d) - 16d) : (KK * yr);
            double u = 13d * L * (up - urp);
            double v = 13d * L * (vp - vrp);

            return new double[3] { L, u, v };
        }

        /// <summary>
        /// XYZ 转换到 RGB(0-1.0)。
        /// </summary>
        /// <param name="XYZ">XYZ 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <returns></returns>
        internal static double[] XYZ2RGB(double[] XYZ, Reference reference)
        {
            if (XYZ == null)
            {
                throw new ArgumentNullException(nameof(XYZ));
            }
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            double X = XYZ[0];
            double Y = XYZ[1];
            double Z = XYZ[2];
            if (reference.Adaptation != Adaptation.None)
            {
                double dX = reference.WhiteRGB[0] * reference.MtxAdaptation[0, 0] +
                            reference.WhiteRGB[1] * reference.MtxAdaptation[1, 0] +
                            reference.WhiteRGB[2] * reference.MtxAdaptation[2, 0];
                double dY = reference.WhiteRGB[0] * reference.MtxAdaptation[0, 1] +
                            reference.WhiteRGB[1] * reference.MtxAdaptation[1, 1] +
                            reference.WhiteRGB[2] * reference.MtxAdaptation[2, 1];
                double dZ = reference.WhiteRGB[0] * reference.MtxAdaptation[0, 2] +
                            reference.WhiteRGB[1] * reference.MtxAdaptation[1, 2] +
                            reference.WhiteRGB[2] * reference.MtxAdaptation[2, 2];

                double sX = reference.White[0] * reference.MtxAdaptation[0, 0] +
                            reference.White[1] * reference.MtxAdaptation[1, 0] +
                            reference.White[2] * reference.MtxAdaptation[2, 0];
                double sY = reference.White[0] * reference.MtxAdaptation[0, 1] +
                            reference.White[1] * reference.MtxAdaptation[1, 1] +
                            reference.White[2] * reference.MtxAdaptation[2, 1];
                double sZ = reference.White[0] * reference.MtxAdaptation[0, 2] +
                            reference.White[1] * reference.MtxAdaptation[1, 2] +
                            reference.White[2] * reference.MtxAdaptation[2, 2];

                double fX = X * reference.MtxAdaptation[0, 0] + Y * reference.MtxAdaptation[1, 0] + Z * reference.MtxAdaptation[2, 0];
                double fY = X * reference.MtxAdaptation[0, 1] + Y * reference.MtxAdaptation[1, 1] + Z * reference.MtxAdaptation[2, 1];
                double fZ = X * reference.MtxAdaptation[0, 2] + Y * reference.MtxAdaptation[1, 2] + Z * reference.MtxAdaptation[2, 2];

                fX *= dX / sX;
                fY *= dY / sY;
                fZ *= dZ / sZ;

                X = fX * reference.MtxAdaptationI[0, 0] + fY * reference.MtxAdaptationI[1, 0] + fZ * reference.MtxAdaptationI[2, 0];
                Y = fX * reference.MtxAdaptationI[0, 1] + fY * reference.MtxAdaptationI[1, 1] + fZ * reference.MtxAdaptationI[2, 1];
                Z = fX * reference.MtxAdaptationI[0, 2] + fY * reference.MtxAdaptationI[1, 2] + fZ * reference.MtxAdaptationI[2, 2];
            }
            double R = GammaCompand(X * reference.MtxXYZ2RGB[0, 0] + Y * reference.MtxXYZ2RGB[1, 0] + Z * reference.MtxXYZ2RGB[2, 0], reference.Gamma);
            double G = GammaCompand(X * reference.MtxXYZ2RGB[0, 1] + Y * reference.MtxXYZ2RGB[1, 1] + Z * reference.MtxXYZ2RGB[2, 1], reference.Gamma);
            double B = GammaCompand(X * reference.MtxXYZ2RGB[0, 2] + Y * reference.MtxXYZ2RGB[1, 2] + Z * reference.MtxXYZ2RGB[2, 2], reference.Gamma);
            //
            return new double[3] { R, G, B };
        }

        /// <summary>
        /// XYZ 转换到 xyY。
        /// </summary>
        /// <param name="XYZ">XYZ 值的数组。</param>
        /// <param name="reference">转换参照值。</param>
        /// <returns></returns>
        internal static double[] XYZ2xyY(double[] XYZ, Reference reference)
        {
            if (XYZ == null)
            {
                throw new ArgumentNullException(nameof(XYZ));
            }
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }
            double Den = XYZ[0] + XYZ[1] + XYZ[2];
            double x;
            double y;
            if (Den > 0d)
            {
                x = XYZ[0] / Den;
                y = XYZ[1] / Den;
            }
            else
            {
                x = reference.White[0] / (reference.White[0] + reference.White[1] + reference.White[2]);
                y = reference.White[1] / (reference.White[0] + reference.White[1] + reference.White[2]);
            }
            return new double[3] { x, y, XYZ[1] };
        }

        /// <summary>
        /// XYZ 转 RGB 复合 Gamma 修正。
        /// </summary>
        /// <param name="linear">已通过调整器转换和矩阵转换后的 XYZ 值的其中一个数值。</param>
        /// <param name="gamma">Gamma 值。可选 1.0，1.8，2.2，-2.2=sRGB，0=L*。</param>
        /// <returns></returns>
        private static double GammaCompand(double linear, double gamma)
        {
            if (gamma > 0d)
            {
                return linear >= 0d ? Math.Pow(linear, 1d / gamma) : -Math.Pow(-linear, 1d / gamma);
            }
            else if (gamma < 0d)
            {
                /* sRGB */
                double sign = 1d;
                if (linear < 0d)
                {
                    sign = -1d;
                    linear = -linear;
                }
                double companded = linear > 0.0031308d ? (1.055d * Math.Pow(linear, 1d / 2.4d) - 0.055d) : linear * 12.92d;
                companded *= sign;
                return companded;
            }
            else
            {
                /* L* */
                double sign = 1d;
                if (linear < 0d)
                {
                    sign = -1d;
                    linear = -linear;
                }
                double companded = linear > 216d / 24389d ? 1.16d * Math.Pow(linear, 1d / 3d) - 0.16d : linear * 24389d / 2700d;
                companded *= sign;
                return companded;
            }
        }

        /// <summary>
        /// RGB 转 XYZ 线性 Gamma 修正。
        /// </summary>
        /// <param name="companded">RGB(0-1.0) 值的其中一个数值。</param>
        /// <param name="gamma">Gamma 值。可选 1.0，1.8，2.2，-2.2=sRGB，0=L*。</param>
        /// <returns></returns>
        private static double GammaLinear(double companded, double gamma)
        {
            if (gamma > 0d)
            {
                return companded >= 0d ? Math.Pow(companded, gamma) : -Math.Pow(-companded, gamma);
            }
            else if (gamma < 0d)
            {
                /* sRGB */
                double sign = 1d;
                if (companded < 0d)
                {
                    sign = -1d;
                    companded = -companded;
                }
                double linear = companded > 0.04045d ? Math.Pow((companded + 0.055d) / 1.055d, 2.4d) : (companded / 12.92d);
                linear *= sign;
                return linear;
            }
            else
            {
                /* L* */
                double sign = 1d;
                if (companded < 0d)
                {
                    sign = -1d;
                    companded = -companded;
                }
                double linear = companded > 0.08d ?
                    (((1000000d * companded + 480000d) * companded + 76800d) * companded + 4096d) / 1560896d
                    :
                    2700d * companded / 24389d;
                linear *= sign;
                return linear;
            }
        }
    }

    /// <summary>
    /// 转换参照值。
    /// </summary>
    [SuppressMessage("Performance", "CA1812:避免未实例化的内部类", Justification = "<挂起>")]
    internal sealed class Reference
    {
        #region 成员

        /// <summary>
        /// 调整器。
        /// </summary>
        internal Adaptation Adaptation { get; }

        /// <summary>
        /// 波长转换常量。360nm 至 830nm，5nm 间隔。
        /// </summary>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[,] CIE1931StandardObserver { get; } = new double[3, 95] {
            {
                0.000129900000d, 0.000232100000d, 0.000414900000d, 0.000741600000d, 0.001368000000d, 0.002236000000d,
                0.004243000000d, 0.007650000000d, 0.014310000000d, 0.023190000000d, 0.043510000000d, 0.077630000000d,
                0.134380000000d, 0.214770000000d, 0.283900000000d, 0.328500000000d, 0.348280000000d, 0.348060000000d,
                0.336200000000d, 0.318700000000d, 0.290800000000d, 0.251100000000d, 0.195360000000d, 0.142100000000d,
                0.095640000000d, 0.057950010000d, 0.032010000000d, 0.014700000000d, 0.004900000000d, 0.002400000000d,
                0.009300000000d, 0.029100000000d, 0.063270000000d, 0.109600000000d, 0.165500000000d, 0.225749900000d,
                0.290400000000d, 0.359700000000d, 0.433449900000d, 0.512050100000d, 0.594500000000d, 0.678400000000d,
                0.762100000000d, 0.842500000000d, 0.916300000000d, 0.978600000000d, 1.026300000000d, 1.056700000000d,
                1.062200000000d, 1.045600000000d, 1.002600000000d, 0.938400000000d, 0.854449900000d, 0.751400000000d,
                0.642400000000d, 0.541900000000d, 0.447900000000d, 0.360800000000d, 0.283500000000d, 0.218700000000d,
                0.164900000000d, 0.121200000000d, 0.087400000000d, 0.063600000000d, 0.046770000000d, 0.032900000000d,
                0.022700000000d, 0.015840000000d, 0.011359160000d, 0.008110916000d, 0.005790346000d, 0.004109457000d,
                0.002899327000d, 0.002049190000d, 0.001439971000d, 0.000999949300d, 0.000690078600d, 0.000476021300d,
                0.000332301100d, 0.000234826100d, 0.000166150500d, 0.000117413000d, 0.000083075270d, 0.000058706520d,
                0.000041509940d, 0.000029353260d, 0.000020673830d, 0.000014559770d, 0.000010253980d, 0.000007221456d,
                0.000005085868d, 0.000003581652d, 0.000002522525d, 0.000001776509d, 0.000001251141d
            },
            {
                0.000003917000d, 0.000006965000d, 0.000012390000d, 0.000022020000d, 0.000039000000d, 0.000064000000d,
                0.000120000000d, 0.000217000000d, 0.000396000000d, 0.000640000000d, 0.001210000000d, 0.002180000000d,
                0.004000000000d, 0.007300000000d, 0.011600000000d, 0.016840000000d, 0.023000000000d, 0.029800000000d,
                0.038000000000d, 0.048000000000d, 0.060000000000d, 0.073900000000d, 0.090980000000d, 0.112600000000d,
                0.139020000000d, 0.169300000000d, 0.208020000000d, 0.258600000000d, 0.323000000000d, 0.407300000000d,
                0.503000000000d, 0.608200000000d, 0.710000000000d, 0.793200000000d, 0.862000000000d, 0.914850100000d,
                0.954000000000d, 0.980300000000d, 0.994950100000d, 1.000000000000d, 0.995000000000d, 0.978600000000d,
                0.952000000000d, 0.915400000000d, 0.870000000000d, 0.816300000000d, 0.757000000000d, 0.694900000000d,
                0.631000000000d, 0.566800000000d, 0.503000000000d, 0.441200000000d, 0.381000000000d, 0.321000000000d,
                0.265000000000d, 0.217000000000d, 0.175000000000d, 0.138200000000d, 0.107000000000d, 0.081600000000d,
                0.061000000000d, 0.044580000000d, 0.032000000000d, 0.023200000000d, 0.017000000000d, 0.011920000000d,
                0.008210000000d, 0.005723000000d, 0.004102000000d, 0.002929000000d, 0.002091000000d, 0.001484000000d,
                0.001047000000d, 0.000740000000d, 0.000520000000d, 0.000361100000d, 0.000249200000d, 0.000171900000d,
                0.000120000000d, 0.000084800000d, 0.000060000000d, 0.000042400000d, 0.000030000000d, 0.000021200000d,
                0.000014990000d, 0.000010600000d, 0.000007465700d, 0.000005257800d, 0.000003702900d, 0.000002607800d,
                0.000001836600d, 0.000001293400d, 0.000000910930d, 0.000000641530d, 0.000000451810d
            },
            {
                0.000606100000d, 0.001086000000d, 0.001946000000d, 0.003486000000d, 0.006450001000d, 0.010549990000d,
                0.020050010000d, 0.036210000000d, 0.067850010000d, 0.110200000000d, 0.207400000000d, 0.371300000000d,
                0.645600000000d, 1.039050100000d, 1.385600000000d, 1.622960000000d, 1.747060000000d, 1.782600000000d,
                1.772110000000d, 1.744100000000d, 1.669200000000d, 1.528100000000d, 1.287640000000d, 1.041900000000d,
                0.812950100000d, 0.616200000000d, 0.465180000000d, 0.353300000000d, 0.272000000000d, 0.212300000000d,
                0.158200000000d, 0.111700000000d, 0.078249990000d, 0.057250010000d, 0.042160000000d, 0.029840000000d,
                0.020300000000d, 0.013400000000d, 0.008749999000d, 0.005749999000d, 0.003900000000d, 0.002749999000d,
                0.002100000000d, 0.001800000000d, 0.001650001000d, 0.001400000000d, 0.001100000000d, 0.001000000000d,
                0.000800000000d, 0.000600000000d, 0.000340000000d, 0.000240000000d, 0.000190000000d, 0.000100000000d,
                0.000049999990d, 0.000030000000d, 0.000020000000d, 0.000010000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d
            }
        };

        /// <summary>
        /// 波长转换常量。360nm 至 830nm，5nm 间隔。
        /// </summary>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[,] CIE1964StandardObserver { get; } = new double[3, 95] {
            {
                0.000000122200d, 0.000000919270d, 0.000005958600d, 0.000033266000d, 0.000159952000d, 0.000662440000d,
                0.002361600000d, 0.007242300000d, 0.019109700000d, 0.043400000000d, 0.084736000000d, 0.140638000000d,
                0.204492000000d, 0.264737000000d, 0.314679000000d, 0.357719000000d, 0.383734000000d, 0.386726000000d,
                0.370702000000d, 0.342957000000d, 0.302273000000d, 0.254085000000d, 0.195618000000d, 0.132349000000d,
                0.080507000000d, 0.041072000000d, 0.016172000000d, 0.005132000000d, 0.003816000000d, 0.015444000000d,
                0.037465000000d, 0.071358000000d, 0.117749000000d, 0.172953000000d, 0.236491000000d, 0.304213000000d,
                0.376772000000d, 0.451584000000d, 0.529826000000d, 0.616053000000d, 0.705224000000d, 0.793832000000d,
                0.878655000000d, 0.951162000000d, 1.014160000000d, 1.074300000000d, 1.118520000000d, 1.134300000000d,
                1.123990000000d, 1.089100000000d, 1.030480000000d, 0.950740000000d, 0.856297000000d, 0.754930000000d,
                0.647467000000d, 0.535110000000d, 0.431567000000d, 0.343690000000d, 0.268329000000d, 0.204300000000d,
                0.152568000000d, 0.112210000000d, 0.081260600000d, 0.057930000000d, 0.040850800000d, 0.028623000000d,
                0.019941300000d, 0.013842000000d, 0.009576880000d, 0.006605200000d, 0.004552630000d, 0.003144700000d,
                0.002174960000d, 0.001505700000d, 0.001044760000d, 0.000727450000d, 0.000508258000d, 0.000356380000d,
                0.000250969000d, 0.000177730000d, 0.000126390000d, 0.000090151000d, 0.000064525800d, 0.000046339000d,
                0.000033411700d, 0.000024209000d, 0.000017611500d, 0.000012855000d, 0.000009413630d, 0.000006913000d,
                0.000005093470d, 0.000003767100d, 0.000002795310d, 0.000002082000d, 0.000001553140d
            },
            {
                0.000000013398d, 0.000000100650d, 0.000000651100d, 0.000003625000d, 0.000017364000d, 0.000071560000d,
                0.000253400000d, 0.000768500000d, 0.002004400000d, 0.004509000000d, 0.008756000000d, 0.014456000000d,
                0.021391000000d, 0.029497000000d, 0.038676000000d, 0.049602000000d, 0.062077000000d, 0.074704000000d,
                0.089456000000d, 0.106256000000d, 0.128201000000d, 0.152761000000d, 0.185190000000d, 0.219940000000d,
                0.253589000000d, 0.297665000000d, 0.339133000000d, 0.395379000000d, 0.460777000000d, 0.531360000000d,
                0.606741000000d, 0.685660000000d, 0.761757000000d, 0.823330000000d, 0.875211000000d, 0.923810000000d,
                0.961988000000d, 0.982200000000d, 0.991761000000d, 0.999110000000d, 0.997340000000d, 0.982380000000d,
                0.955552000000d, 0.915175000000d, 0.868934000000d, 0.825623000000d, 0.777405000000d, 0.720353000000d,
                0.658341000000d, 0.593878000000d, 0.527963000000d, 0.461834000000d, 0.398057000000d, 0.339554000000d,
                0.283493000000d, 0.228254000000d, 0.179828000000d, 0.140211000000d, 0.107633000000d, 0.081187000000d,
                0.060281000000d, 0.044096000000d, 0.031800400000d, 0.022601700000d, 0.015905100000d, 0.011130300000d,
                0.007748800000d, 0.005375100000d, 0.003717740000d, 0.002564560000d, 0.001768470000d, 0.001222390000d,
                0.000846190000d, 0.000586440000d, 0.000407410000d, 0.000284041000d, 0.000198730000d, 0.000113955000d,
                0.000098428000d, 0.000069819000d, 0.000049737000d, 0.000035540500d, 0.000025486000d, 0.000018338400d,
                0.000013249000d, 0.000009619600d, 0.000007012800d, 0.000005129800d, 0.000003764730d, 0.000002770810d,
                0.000002046130d, 0.000001516770d, 0.000001128090d, 0.000000842160d, 0.000000629700d
            },
            {
                0.000000535027d, 0.000004028300d, 0.000026143700d, 0.000146220000d, 0.000704776000d, 0.002927800000d,
                0.010482200000d, 0.032344000000d, 0.086010900000d, 0.197120000000d, 0.389366000000d, 0.656760000000d,
                0.972542000000d, 1.282500000000d, 1.553480000000d, 1.798500000000d, 1.967280000000d, 2.027300000000d,
                1.994800000000d, 1.900700000000d, 1.745370000000d, 1.554900000000d, 1.317560000000d, 1.030200000000d,
                0.772125000000d, 0.570060000000d, 0.415254000000d, 0.302356000000d, 0.218502000000d, 0.159249000000d,
                0.112044000000d, 0.082248000000d, 0.060709000000d, 0.043050000000d, 0.030451000000d, 0.020584000000d,
                0.013676000000d, 0.007918000000d, 0.003988000000d, 0.001091000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d,
                0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d, 0.000000000000d
            }
        };

        /// <summary>
        /// Gamma。
        /// </summary>
        internal double Gamma { get; }

        /// <summary>
        /// 光源。
        /// </summary>
        internal Illuminant Illuminant { get; }

        /// <summary>
        /// 调整器矩阵。
        /// </summary>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[,] MtxAdaptation { get; }

        /// <summary>
        /// 调整器逆矩阵。
        /// </summary>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[,] MtxAdaptationI { get; }

        /// <summary>
        /// RGB 转换 XYZ 矩阵。
        /// </summary>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[,] MtxRGB2XYZ { get; }

        /// <summary>
        /// XYZ 转换 RGB 矩阵。
        /// </summary>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[,] MtxXYZ2RGB { get; }

        /// <summary>
        /// 观测角度。
        /// </summary>
        internal Observer Observer { get; }

        /// <summary>
        /// RGB 模型。
        /// </summary>
        internal RGBModel RGBModel { get; }

        /// <summary>
        /// 波长转换常量。
        /// </summary>
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[] RT { get; } = {
                      0.0e-6, 10.0e-6, 20.0e-6, 30.0e-6, 40.0e-6, 50.0e-6,
                      60.0e-6, 70.0e-6, 80.0e-6, 90.0e-6, 100.0e-6, 125.0e-6,
                      150.0e-6, 175.0e-6, 200.0e-6, 225.0e-6, 250.0e-6, 275.0e-6,
                      300.0e-6, 325.0e-6, 350.0e-6, 375.0e-6, 400.0e-6, 425.0e-6,
                      450.0e-6, 475.0e-6, 500.0e-6, 525.0e-6, 550.0e-6, 575.0e-6,
                      600.0e-6
            };

        /// <summary>
        /// 波长转换常量。
        /// </summary>
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[] T { get; } = {
                     -0.24341d, -0.25479d, -0.26876d, -0.28539d, -0.30470d, -0.32675d,
                     -0.35156d, -0.37915d, -0.40955d, -0.44278d, -0.47888d, -0.58204d,
                     -0.70471d, -0.84901d, -1.0182d, -1.2168d, -1.4512d, -1.7298d,
                     -2.0637d, -2.4681d, -2.9641d, -3.5814d, -4.3633d, -5.3762d,
                     -6.7262d, -8.5955d, -11.324d, -15.628d, -23.325d, -40.770d,
                     -116.45d
            };

        /// <summary>
        /// 波长转换常量。
        /// </summary>
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[] U { get; } = {
                     0.18006d, 0.18066d, 0.18133d, 0.18208d, 0.18293d, 0.18388d,
                     0.18494d, 0.18611d, 0.18740d, 0.18880d, 0.19032d, 0.19462d,
                     0.19962d, 0.20525d, 0.21142d, 0.21807d, 0.22511d, 0.23247d,
                     0.24010d, 0.24792d, 0.25591d, 0.26400d, 0.27218d, 0.28039d, /* 0.24792 is correct, W&S shows as 0.24702 which is a typo */
                     0.28863d, 0.29685d, 0.30505d, 0.31320d, 0.32129d, 0.32931d,
                     0.33724d
                };

        /// <summary>
        /// 波长转换常量。
        /// </summary>
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[] V { get; } = {
                     0.26352d, 0.26589d, 0.26846d, 0.27119d, 0.27407d, 0.27709d,
                     0.28021d, 0.28342d, 0.28668d, 0.28997d, 0.29326d, 0.30141d,
                     0.30921d, 0.31647d, 0.32312d, 0.32909d, 0.33439d, 0.33904d,
                     0.34308d, 0.34655d, 0.34951d, 0.35200d, 0.35407d, 0.35577d,
                     0.35714d, 0.35823d, 0.35907d, 0.35968d, 0.36011d, 0.36038d,
                     0.36051
                    };

        /// <summary>
        /// 白光参照。
        /// </summary>
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[] White { get; }

        /// <summary>
        /// RGB 白光参照。
        /// </summary>
        [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "<挂起>")]
        internal double[] WhiteRGB { get; }

        #endregion 成员

        /// <summary>
        /// 初始化 Reference 类的新实例。
        /// </summary>
        /// <param name="illuminant">光源。</param>
        /// <param name="observer">观测角度。</param>
        /// <param name="model">RGB 模型。</param>
        /// <param name="adaptation">调整器。</param>
        [SuppressMessage("样式", "IDE0066:将 switch 语句转换为表达式", Justification = "<挂起>")]
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        internal Reference(Illuminant illuminant, Observer observer, RGBModel model, Adaptation adaptation)
        {
            this.Illuminant = illuminant;
            this.Observer = observer;
            this.RGBModel = model;
            this.Adaptation = adaptation;
            //
            switch (illuminant)
            {
                case Illuminant.A:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1.11144d, 1d, 0.35200d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 1.09850d, 1d, 0.35585d }; break;
                    }
                    break;

                case Illuminant.B:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.99178d, 1d, 0.843493d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.99072d, 1d, 0.85223d }; break;
                    }
                    break;

                case Illuminant.C:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.97285d, 1d, 1.16145d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.98074d, 1d, 1.18232d }; break;
                    }
                    break;

                case Illuminant.D50:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.96720d, 1d, 0.81427d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.96422d, 1d, 0.82521d }; break;
                    }
                    break;

                case Illuminant.D55:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.95799d, 1d, 0.90926d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.95682d, 1d, 0.92149d }; break;
                    }
                    break;

                case Illuminant.D65:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.94811d, 1d, 1.07304d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.95047d, 1d, 1.08883d }; break;
                    }
                    break;

                case Illuminant.D75:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.94416d, 1d, 1.20641d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.94972d, 1d, 1.22638d }; break;
                    }
                    break;

                case Illuminant.F1:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.94791d, 1d, 1.03191d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.92834d, 1d, 1.03665d }; break;
                    }
                    break;

                case Illuminant.F2:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1.03280d, 1d, 0.69026d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.99186d, 1d, 0.67393d }; break;
                    }
                    break;

                case Illuminant.F3:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1.08968d, 1d, 0.51965d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 1.03754d, 1d, 0.49861d }; break;
                    }
                    break;

                case Illuminant.F4:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1.14961d, 1d, 0.40963d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 1.09147d, 1d, 0.38813d }; break;
                    }
                    break;

                case Illuminant.F5:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.93369d, 1d, 0.98636d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.90872d, 1d, 0.98723d }; break;
                    }
                    break;

                case Illuminant.F6:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1.02148d, 1d, 0.62074d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.97309d, 1d, 0.60191d }; break;
                    }
                    break;

                case Illuminant.F7:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.95792d, 1d, 1.07687d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.95041d, 1d, 1.08747d }; break;
                    }
                    break;

                case Illuminant.F8:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.97115d, 1d, 0.81135d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.96413d, 1d, 0.82333d }; break;
                    }
                    break;

                case Illuminant.F9:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1.02116d, 1d, 0.67826d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 1.00365d, 1d, 0.67868d }; break;
                    }
                    break;

                case Illuminant.F10:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 0.99001d, 1d, 0.83134d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 0.96174d, 1d, 0.81712d }; break;
                    }
                    break;

                case Illuminant.F11:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1.03866d, 1d, 0.65627d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 1.00962d, 1d, 0.64350d }; break;
                    }
                    break;

                case Illuminant.F12:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1.11428d, 1d, 0.40353d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 1.08046d, 1d, 0.39228d }; break;
                    }
                    break;

                case Illuminant.E:
                default:
                    switch (observer)
                    {
                        case Observer.CIE1964: this.White = new double[3] { 1d, 1d, 1d }; break;
                        case Observer.CIE1931: default: this.White = new double[3] { 1d, 1d, 1d }; break;
                    }
                    break;
            }
            //
            double xr;
            double yr;
            double xg;
            double yg;
            double xb;
            double yb;
            //
            switch (model)
            {
                case RGBModel.Adobe1998:
                    this.WhiteRGB = new double[3] { 0.95047d, 1d, 1.08883d };
                    this.Gamma = 2.2d;
                    xr = 0.64d;
                    yr = 0.33d;
                    xg = 0.21d;
                    yg = 0.71d;
                    xb = 0.15d;
                    yb = 0.06d;
                    break;

                case RGBModel.Apple:
                    this.WhiteRGB = new double[3] { 0.95047d, 1d, 1.08883d };
                    this.Gamma = 1.8d;
                    xr = 0.625d;
                    yr = 0.340d;
                    xg = 0.280d;
                    yg = 0.595d;
                    xb = 0.155d;
                    yb = 0.070d;
                    break;

                case RGBModel.Best:
                    this.WhiteRGB = new double[3] { 0.96422d, 1d, 0.82521d };
                    this.Gamma = 2.2d;
                    xr = 0.7347d;
                    yr = 0.2653d;
                    xg = 0.2150d;
                    yg = 0.7750d;
                    xb = 0.1300d;
                    yb = 0.0350d;
                    break;

                case RGBModel.Beta:
                    this.WhiteRGB = new double[3] { 0.96422d, 1d, 0.82521d };
                    this.Gamma = 2.2d;
                    xr = 0.6888d;
                    yr = 0.3112d;
                    xg = 0.1986d;
                    yg = 0.7551d;
                    xb = 0.1265d;
                    yb = 0.0352d;
                    break;

                case RGBModel.Bruce:
                    this.WhiteRGB = new double[3] { 0.95047d, 1d, 1.08883d };
                    this.Gamma = 2.2d;
                    xr = 0.64d;
                    yr = 0.33d;
                    xg = 0.28d;
                    yg = 0.65d;
                    xb = 0.15d;
                    yb = 0.06d;
                    break;

                case RGBModel.CIE:
                    this.WhiteRGB = new double[3] { 1d, 1d, 1d };
                    this.Gamma = 2.2d;
                    xr = 0.735d;
                    yr = 0.265d;
                    xg = 0.274d;
                    yg = 0.717d;
                    xb = 0.167d;
                    yb = 0.009d;
                    break;

                case RGBModel.ColorMatch:
                    this.WhiteRGB = new double[3] { 0.96422d, 1d, 0.82521d };
                    this.Gamma = 1.8d;
                    xr = 0.630d;
                    yr = 0.340d;
                    xg = 0.295d;
                    yg = 0.605d;
                    xb = 0.150d;
                    yb = 0.075d;
                    break;

                case RGBModel.Don4:
                    this.WhiteRGB = new double[3] { 0.96422d, 1d, 0.82521d };
                    this.Gamma = 2.2d;
                    xr = 0.696d;
                    yr = 0.300d;
                    xg = 0.215d;
                    yg = 0.765d;
                    xb = 0.130d;
                    yb = 0.035d;
                    break;

                case RGBModel.ECIv2:
                    this.WhiteRGB = new double[3] { 0.96422d, 1d, 0.82521d };
                    this.Gamma = 0d;
                    xr = 0.67d;
                    yr = 0.33d;
                    xg = 0.21d;
                    yg = 0.71d;
                    xb = 0.14d;
                    yb = 0.08d;
                    break;

                case RGBModel.EktaSpacePS5:
                    this.WhiteRGB = new double[3] { 0.96422d, 1d, 0.82521d };
                    this.Gamma = 2.2d;
                    xr = 0.695d;
                    yr = 0.305d;
                    xg = 0.260d;
                    yg = 0.700d;
                    xb = 0.110d;
                    yb = 0.005d;
                    break;

                case RGBModel.NTSC:
                    this.WhiteRGB = new double[3] { 0.98074d, 1d, 1.18232d };
                    this.Gamma = 2.2d;
                    xr = 0.67d;
                    yr = 0.33d;
                    xg = 0.21d;
                    yg = 0.71d;
                    xb = 0.14d;
                    yb = 0.08d;
                    break;

                case RGBModel.PAL_SECAM:
                    this.WhiteRGB = new double[3] { 0.95047d, 1d, 1.08883d };
                    this.Gamma = 2.2d;
                    xr = 0.64d;
                    yr = 0.33d;
                    xg = 0.29d;
                    yg = 0.60d;
                    xb = 0.15d;
                    yb = 0.06d;
                    break;

                case RGBModel.ProPhoto:
                    this.WhiteRGB = new double[3] { 0.96422d, 1d, 0.82521d };
                    this.Gamma = 1.8d;
                    xr = 0.7347d;
                    yr = 0.2653d;
                    xg = 0.1596d;
                    yg = 0.8404d;
                    xb = 0.0366d;
                    yb = 0.0001d;
                    break;

                case RGBModel.SMPTE_C:
                    this.WhiteRGB = new double[3] { 0.95047d, 1d, 1.08883d };
                    this.Gamma = 2.2d;
                    xr = 0.630d;
                    yr = 0.340d;
                    xg = 0.310d;
                    yg = 0.595d;
                    xb = 0.155d;
                    yb = 0.070d;
                    break;

                case RGBModel.WideGamut:
                    this.WhiteRGB = new double[3] { 0.96422d, 1d, 0.82521d };
                    this.Gamma = 2.2d;
                    xr = 0.735d;
                    yr = 0.265d;
                    xg = 0.115d;
                    yg = 0.826d;
                    xb = 0.157d;
                    yb = 0.018d;
                    break;

                case RGBModel.Standard:
                default:
                    this.WhiteRGB = new double[3] { 0.95047d, 1d, 1.08883d };
                    this.Gamma = -2.2d;
                    xr = 0.64d;
                    yr = 0.33d;
                    xg = 0.30d;
                    yg = 0.60d;
                    xb = 0.15d;
                    yb = 0.06d;
                    break;
            }
            double[,] m = new double[3, 3] {
                { xr / yr, xg / yg, xb / yb },
                { 1d, 1d, 1d },
                { (1d - xr - yr) / yr, (1d - xg - yg) / yg, (1d - xb - yb) / yb }
            };
            double[,] mi = Invert3x3(m);
            double sr = this.WhiteRGB[0] * mi[0, 0] + this.WhiteRGB[1] * mi[0, 1] + this.WhiteRGB[2] * mi[0, 2];
            double sg = this.WhiteRGB[0] * mi[1, 0] + this.WhiteRGB[1] * mi[1, 1] + this.WhiteRGB[2] * mi[1, 2];
            double sb = this.WhiteRGB[0] * mi[2, 0] + this.WhiteRGB[1] * mi[2, 1] + this.WhiteRGB[2] * mi[2, 2];
            this.MtxRGB2XYZ = new double[3, 3] {
                { sr * m[0, 0], sg * m[0, 1], sb * m[0, 2] },
                { sr * m[1, 0], sg * m[1, 1], sb * m[1, 2] },
                { sr * m[2, 0], sg * m[2, 1], sb * m[2, 2] }
            };
            Transpose3x3(this.MtxRGB2XYZ);
            this.MtxXYZ2RGB = Invert3x3(this.MtxRGB2XYZ);
            //
            switch (adaptation)
            {
                case Adaptation.Bradford:
                    this.MtxAdaptation = new double[3, 3] {
                        { 0.8951d, -0.7502d, 0.0389d },
                        { 0.2664d, 1.7135d, -0.0685d },
                        { -0.1614d, 0.0367d, 1.0296d }
                    };
                    this.MtxAdaptationI = Invert3x3(this.MtxAdaptation);
                    break;

                case Adaptation.VonKries:
                    this.MtxAdaptation = new double[3, 3] {
                        { 0.40024d, -0.22630d, 0d },
                        { 0.70760d, 1.16532d, 0d },
                        { -0.08081d, 0.04570d, 0.91822d }
                    };
                    this.MtxAdaptationI = Invert3x3(this.MtxAdaptation);
                    break;

                case Adaptation.XYZScaling:
                case Adaptation.None:
                default:
                    this.MtxAdaptation = new double[3, 3] { { 1d, 0d, 0d }, { 0d, 1d, 0d }, { 0d, 0d, 1d } };
                    this.MtxAdaptationI = new double[3, 3] { { 1d, 0d, 0d }, { 0d, 1d, 0d }, { 0d, 0d, 1d } };
                    break;
            }
        }

        /// <summary>
        /// 获取 3x3 矩阵的行列式。
        /// </summary>
        /// <param name="m">3x3 矩阵。</param>
        /// <returns></returns>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        [SuppressMessage("Build", "CA1822:成员 Determinant3x3 不访问实例数据，可标记为 static (在 Visual Basic 中为 Shared)", Justification = "<挂起>")]
        private double Determinant3x3(double[,] m)
        {
            return m[0, 0] * (m[2, 2] * m[1, 1] - m[2, 1] * m[1, 2]) -
                   m[1, 0] * (m[2, 2] * m[0, 1] - m[2, 1] * m[0, 2]) +
                   m[2, 0] * (m[1, 2] * m[0, 1] - m[1, 1] * m[0, 2]);
        }

        /// <summary>
        /// 逆 3x3 矩阵。
        /// </summary>
        /// <param name="m">3x3 矩阵。</param>
        /// <returns></returns>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        private double[,] Invert3x3(double[,] m)
        {
            double scale = 1d / Determinant3x3(m);
            double[,] mi = new double[3, 3];
            mi[0, 0] = scale * (m[2, 2] * m[1, 1] - m[2, 1] * m[1, 2]);
            mi[0, 1] = -scale * (m[2, 2] * m[0, 1] - m[2, 1] * m[0, 2]);
            mi[0, 2] = scale * (m[1, 2] * m[0, 1] - m[1, 1] * m[0, 2]);
            mi[1, 0] = -scale * (m[2, 2] * m[1, 0] - m[2, 0] * m[1, 2]);
            mi[1, 1] = scale * (m[2, 2] * m[0, 0] - m[2, 0] * m[0, 2]);
            mi[1, 2] = -scale * (m[1, 2] * m[0, 0] - m[1, 0] * m[0, 2]);
            mi[2, 0] = scale * (m[2, 1] * m[1, 0] - m[2, 0] * m[1, 1]);
            mi[2, 1] = -scale * (m[2, 1] * m[0, 0] - m[2, 0] * m[0, 1]);
            mi[2, 2] = scale * (m[1, 1] * m[0, 0] - m[1, 0] * m[0, 1]);
            return mi;
        }

        /// <summary>
        /// 3x3 矩阵轴对换。
        /// </summary>
        /// <param name="m">3x3 矩阵。</param>
        [SuppressMessage("Performance", "CA1814:与多维数组相比，首选使用交错数组", Justification = "<挂起>")]
        private void Transpose3x3(double[,] m)
        {
            double tmp = m[0, 1];
            m[0, 1] = m[1, 0];
            m[1, 0] = tmp;

            tmp = m[0, 2];
            m[0, 2] = m[2, 0];
            m[2, 0] = tmp;

            tmp = m[1, 2];
            m[1, 2] = m[2, 1];
            m[2, 1] = tmp;
        }
    }
}