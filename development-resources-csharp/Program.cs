using Honoo.Net;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Honoo
{
    internal class Program
    {
        //private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        #region Main

        private static async Task Main()
        {
            using (UPnP uPnP = new UPnP())
            {
                UPnPDevice[] devices = await uPnP.Discover();
                UPnPService service = devices[0].FindServices(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1)[0];
                await uPnP.AddPortMapping(service, false, "TCP", 4788, IPAddress.Parse("192.168.1.1"), 4788, "test", 0, true);
                UPnPPortMappingEntry entry = await uPnP.GetSpecificPortMappingEntry(service, false, "TCP", 4788);
                await uPnP.DeletePortMapping(service, false, "TCP", 4788);
                var a = 1;
            }

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