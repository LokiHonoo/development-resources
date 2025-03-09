using Honoo.Windows;
using System;
using System.Xml.Linq;

namespace Honoo
{
    internal sealed class Program
    {
        #region Main

        private static void Main()
        {
            TestWMI();
            Console.ReadKey(true);
        }

        #endregion Main

        private static void TestWMI()
        {
            WMI.TryGetValue(Win32Class.Win32_Processor, out XElement[] result);
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
    }
}