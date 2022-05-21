using System;

namespace Honoo
{
    internal class Program
    {
        //private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        #region Main

        private static void Main()
        {
            TestPermutationAndCombination();
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