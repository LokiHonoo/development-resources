using System;
using System.Collections.Generic;
using System.Text;

namespace Honoo
{
    internal class Program
    {
        //private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        #region Main

        private static void Main()
        {
            List<char> aaaaa = new List<char>("100001011110000111000011110101110101001111010100011011010010011".ToCharArray());
            Console.WriteLine(Convert.ToString(0x04C11DB7, 2));
            Console.WriteLine(Convert.ToString(~0x04C11DB7, 2));
            Console.WriteLine(Convert.ToString(0xEDB88320, 2));

            byte[] bytes = Encoding.UTF8.GetBytes("11111333333333333444444444444444444444444444444444444444444455556666666666666666666666666666666666666666666666111113333333333333333333333333333344444444444444444444444444444444444444444444444444444444444444555555555555666666666666666666666666666666666666666666661111133333333333333333333333333333444444444444444444444444444444444444444444444444444444444444445555555555556666666666666666666666666666666666666666666666");

            TestCRC(new Honoo.IO.Hashing.CRC_4_ITU(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_5_EPC(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_5_ITU(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_5_USB(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_6_ITU(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_7_MMC(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_8(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_8_ITU(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_8_MAXIM(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_8_ROHC(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_CCITT(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_CCITT_FALSE(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_DNP(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_IBM(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_MAXIM(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_MODBUS(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_USB(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_X25(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_XMODEM(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_16_XMODEM2(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_32(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_32_C(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_32_KOOPMAN(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_32_MPEG2(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_64_ECMA(), bytes);
            TestCRC(new Honoo.IO.Hashing.CRC_64_ISO(), bytes);

            //TestPermutationAndCombination();
            //
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
            ((int[])userState)[0]++;
        }

        private static void TestCRC(Honoo.IO.Hashing.CRC crc, byte[] input)
        {
            byte[] value = crc.DoFinalBytes(input);
            Console.WriteLine(crc.Name + "     " + BitConverter.ToString(value));
        }

        private static void TestPermutationAndCombination()
        {
            int[] a = new int[] { 11, 22, 33, 44, 55, 66, 77, 88, 99 };
            int m = 5;
            int[] count = new int[1];
            Honoo.Collections.Generic.Combination<int> combination = new Honoo.Collections.Generic.Combination<int>(a, m);
            Console.WriteLine($"combination m={m} count = {combination.GetCount()}");
            Console.ReadKey(true);
            combination.Output(Created, count);
            Console.WriteLine($"combination output count = {count[0]}");
            Console.ReadKey(true);
            count[0] = 0;
            Honoo.Collections.Generic.Permutation<int> permutation = new Honoo.Collections.Generic.Permutation<int>(a, m);
            Console.WriteLine($"permutation m={m} count = {permutation.GetCount()}");
            Console.ReadKey(true);
            permutation.Output(Created, count);
            Console.WriteLine($"permutation output count = {count[0]}");
            Console.ReadKey(true);
        }
    }
}