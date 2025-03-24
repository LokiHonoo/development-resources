using Honoo.IO;
using Honoo.Windows.Management.WMI;
using System;
using System.Management;
using System.Numerics;

namespace Honoo
{
    internal sealed class Program
    {
        #region Main

        private static void Main()
        {
            TestBinaries();
            TestPermutationAndCombination();
            TestNumericChange();
            TestWMI();
            Console.ReadKey(true);
        }

        #endregion Main

        private static void Created(int[] result, BigInteger index, BigInteger total, object userState)
        {
            for (int i = 0; i < result.Length; i++)
            {
                if (i == result.Length - 1)
                {
                    Console.WriteLine($"{result[i]}    index {index}/{total}");
                }
                else
                {
                    Console.Write(result[i]);
                    Console.Write(' ');
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:请勿使用不安全的随机性", Justification = "<挂起>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        private static void TestBinaries()
        {
            byte[] bytes = new byte[19];
            new Random().NextBytes(bytes);
            Console.WriteLine(BitConverter.ToInt16(bytes, 6));
            Console.WriteLine(Honoo.Binaries.LEToInt16(bytes, 6, 2));
            Console.WriteLine(BitConverter.ToInt32(bytes, 4));
            Console.WriteLine(Honoo.Binaries.LEToInt32(bytes, 4, 4));
            Console.WriteLine(BitConverter.ToInt64(bytes, 0));
            Console.WriteLine(Honoo.Binaries.LEToInt64(bytes, 0, 8));
            Console.WriteLine(BitConverter.ToUInt16(bytes, 6));
            Console.WriteLine(Honoo.Binaries.LEToUInt16(bytes, 6, 2));
            Console.WriteLine(BitConverter.ToUInt32(bytes, 4));
            Console.WriteLine(Honoo.Binaries.LEToUInt32(bytes, 4, 4));
            Console.WriteLine(BitConverter.ToUInt64(bytes, 0));
            Console.WriteLine(Honoo.Binaries.LEToUInt64(bytes, 0, 8));

            Console.WriteLine(BitConverter.ToString(bytes));
            Console.WriteLine(Honoo.Binaries.GetHex(bytes, false));
            Console.WriteLine(Honoo.Binaries.GetHex(bytes, 0, bytes.Length, false, ":", 4, "    "));

            Console.WriteLine(Honoo.Binaries.LEToUInt64(Honoo.Binaries.UInt64ToLE(BitConverter.ToUInt64(bytes, 0)), 0, 8));

            Console.ReadKey(true);
        }

        private static void TestNumericChange()
        {
            long length = 934772112;
            var value = Numerics.GetSize(length, SizeThousands.Auto, 2, out string unit);
            Console.WriteLine(value + " " + unit);
            value = Numerics.GetSize(length, SizeThousands.GB, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numerics.GetSize(length, SizeKilo.Auto, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numerics.GetSize(length, SizeKilo.GiB, 2, out unit);
            Console.WriteLine(value + " " + unit);

            value = Numerics.GetSpeed(length, SpeedThousands.Auto, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numerics.GetSpeed(length, SpeedThousands.GBps, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numerics.GetSpeed(length, SpeedKilo.Auto, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numerics.GetSpeed(length, SpeedKilo.GiBps, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numerics.GetSpeed(length, SpeedBits.Auto, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numerics.GetSpeed(length, SpeedBits.Gbps, 2, out unit);
            Console.WriteLine(value + " " + unit);

            Console.ReadKey(true);
        }

        private static void TestPermutationAndCombination()
        {
            int[] a = new int[] { 11, 22, 33, 44, 55, 66, 77, 88, 99 };
            int m = 5;
            var combination = new Collections.Generic.Combination<int>(a, m);
            combination.Output(Created, null);
            Console.WriteLine($"combination n={a.Length} m={m} Due count={combination.Count}");
            Console.ReadKey(true);
            var permutation = new Collections.Generic.Permutation<int>(a, m);
            permutation.Output(Created, null);
            Console.WriteLine($"permutation n={a.Length} m={m} Due count={permutation.Count}");
            Console.ReadKey(true);
        }

        private static void TestWMI()
        {
            if (WMI.Query(Win32Class.Win32_Processor, TimeSpan.FromSeconds(2), out ManagementObjectCollection baseObjects) == ManagementStatus.NoError)
            {
                foreach (var o in baseObjects)
                {
                    Console.WriteLine(o.GetText(TextFormat.Mof));
                }
            }
            if (WMI.Query(Win32Class.Win32_Processor, TimeSpan.FromSeconds(2), TextFormat.Mof, out string[] texts1) == ManagementStatus.NoError)
            {
                foreach (var text in texts1)
                {
                    Console.WriteLine(text);
                }
            }
            if (WMI.Query(Win32Class.Win32_Processor, TimeSpan.FromSeconds(2), TextFormat.WmiDtd20, out string[] texts2) == ManagementStatus.NoError)
            {
                foreach (var text in texts2)
                {
                    Console.WriteLine(text);
                }
            }
            if (WMI.Query(Win32Class.Win32_Processor, TimeSpan.FromSeconds(2), TextFormat.CimDtd20, out string[] texts3) == ManagementStatus.NoError)
            {
                foreach (var text in texts3)
                {
                    Console.WriteLine(text);
                }
            }
            Console.ReadKey(true);
        }
    }
}