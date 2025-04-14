using Honoo.Windows.Management;
using System;
using System.Management;

namespace Honoo
{
    internal class Program
    {

        #region Main
        static void Main()
        {
            TestWMI();
            Console.ReadKey(true);
        }
        #endregion


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
