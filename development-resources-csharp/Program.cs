using Honoo.Collections.Generic;
using Honoo.Net;
using System;
using System.Net.Http;
using System.Text;
using System.Xml;

namespace Honoo
{
    internal class Program
    {
        //private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        #region Main

        private static void Main()
        {

            TestUPnP();
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
            ((Counter)userState).Count++;
        }

        private static void TestPermutationAndCombination()
        {
            int[] a = new int[] { 11, 22, 33, 44, 55, 66, 77, 88, 99 };
            int m = 5;
            Counter counter = new Counter();
            Combination<int> combination = new Combination<int>(a, m);
            combination.Output(Created, counter);
            Console.WriteLine($"combination n={a.Length} m={m} Due count={combination.GetCount()}");
            Console.WriteLine($"combination output count={counter.Count}");
            Console.ReadKey(true);
            counter.Count = 0;
            Permutation<int> permutation = new Permutation<int>(a, m);
            permutation.Output(Created, counter);
            Console.WriteLine($"permutation n={a.Length} m={m} Due count={permutation.GetCount()}");
            Console.WriteLine($"permutation output count={counter.Count}");
            Console.ReadKey(true);
        }

        private static async void TestUPnP()
        {
            using (UPnP uPnP = new UPnP())
            {
                UPnPRootDevice[] devices = await uPnP.Discover();
                UPnPService service = devices[0].FindServices(UPnP.URN_UPNP_SERVICE_WAN_IP_CONNECTION_1)[0];
                string scpd = await uPnP.GetScpdInformation(service);
                var a = await uPnP.GetNATRSIPStatus(service, false);
                var b = await uPnP.GetExternalIPAddress(service, false);
                //await uPnP.AddPortMapping(service, false, "TCP", 4788, IPAddress.Parse("192.168.18.11"), 4788, "test", 0, true);
                UPnPPortMappingEntry entry = await uPnP.GetSpecificPortMappingEntry(service, false, "TCP", 4788);
                //await uPnP.DeletePortMapping(service, false, "TCP", 4788);
                var ssa = 1;
            }
        }

        internal sealed class Counter
        {
            internal long Count { get; set; }
        }
    }
}