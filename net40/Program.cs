using Honoo.IO;
using System;

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
            Console.ReadKey(true);
        }

        #endregion Main

        private static void Created(int[] result, object userState)
        {
            for (int i = 0; i < result.Length; i++)
            {
                if (i == result.Length - 1)
                {
                    Console.WriteLine(result[i]);
                }
                else
                {
                    Console.Write(result[i]);
                    Console.Write(' ');
                }
            }
            ((Counter)userState).Count++;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:请勿使用不安全的随机性", Justification = "<挂起>")]
        private static void TestBinaries()
        {
            byte[] bytes = new byte[19];
            new Random().NextBytes(bytes);
            Console.WriteLine(BitConverter.ToInt16(bytes, 6));
            Console.WriteLine(Honoo.Binaries.ToInt16(true, bytes, 6, 2));
            Console.WriteLine(BitConverter.ToInt32(bytes, 4));
            Console.WriteLine(Honoo.Binaries.ToInt32(true, bytes, 4, 4));
            Console.WriteLine(BitConverter.ToInt64(bytes, 0));
            Console.WriteLine(Honoo.Binaries.ToInt64(true, bytes));
            Console.WriteLine(BitConverter.ToUInt16(bytes, 6));
            Console.WriteLine(Honoo.Binaries.ToUInt16(true, bytes, 6, 2));
            Console.WriteLine(BitConverter.ToUInt32(bytes, 4));
            Console.WriteLine(Honoo.Binaries.ToUInt32(true, bytes, 4, 4));
            Console.WriteLine(BitConverter.ToUInt64(bytes, 0));
            Console.WriteLine(Honoo.Binaries.ToUInt64(true, bytes));

            Console.WriteLine(BitConverter.ToString(bytes));
            Console.WriteLine(Honoo.Binaries.ToHex(false, bytes));
            Console.WriteLine(Honoo.Binaries.ToString(bytes));
            Console.WriteLine(Honoo.Binaries.ToString(bytes, 0, bytes.Length, ":", 4, "    "));

            Console.WriteLine(Honoo.Binaries.ToUInt64(true, Honoo.Binaries.GetBytes(true, BitConverter.ToUInt64(bytes, 0))));
        }

        private static void TestNumericChange()
        {
            long length = 934772112;
            var value = Numeric.GetSize(length, Numeric.SizeThousands.Auto, 2, out string unit);
            Console.WriteLine(value + " " + unit);
            value = Numeric.GetSize(length, Numeric.SizeThousands.GB, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numeric.GetSize(length, Numeric.SizeKilo.Auto, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numeric.GetSize(length, Numeric.SizeKilo.GiB, 2, out unit);
            Console.WriteLine(value + " " + unit);

            value = Numeric.GetSpeed(length, Numeric.SpeedThousands.Auto, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numeric.GetSpeed(length, Numeric.SpeedThousands.GBps, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numeric.GetSpeed(length, Numeric.SpeedKilo.Auto, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numeric.GetSpeed(length, Numeric.SpeedKilo.GiBps, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numeric.GetSpeed(length, Numeric.SpeedBits.Auto, 2, out unit);
            Console.WriteLine(value + " " + unit);
            value = Numeric.GetSpeed(length, Numeric.SpeedBits.Gbps, 2, out unit);
            Console.WriteLine(value + " " + unit);
        }

        private static void TestPermutationAndCombination()
        {
            int[] a = new int[] { 11, 22, 33, 44, 55, 66, 77, 88, 99 };
            int m = 5;
            Counter counter = new Counter();
            Collections.Generic.Combination<int> combination = new Collections.Generic.Combination<int>(a, m);
            combination.Output(Created, counter);
            Console.WriteLine($"combination n={a.Length} m={m} Due count={combination.Count}");
            Console.WriteLine($"combination output count={counter.Count}");
            Console.ReadKey(true);
            counter.Count = 0;
            Collections.Generic.Permutation<int> permutation = new Collections.Generic.Permutation<int>(a, m);
            permutation.Output(Created, counter);
            Console.WriteLine($"permutation n={a.Length} m={m} Due count={permutation.Count}");
            Console.WriteLine($"permutation output count={counter.Count}");
        }

        internal sealed class Counter
        {
            internal long Count { get; set; }
        }
    }
}