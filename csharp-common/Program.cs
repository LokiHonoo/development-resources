using System;
using System.Diagnostics.CodeAnalysis;

namespace CSharpCommon
{
    internal class Program
    {
        [SuppressMessage("Globalization", "CA1303:请不要将文本作为本地化参数传递", Justification = "<挂起>")]
        private static void Main()
        {
            LH.BouncyCastleHelpers.Test.TestAll();
            Console.ReadKey(true);
        }
    }
}