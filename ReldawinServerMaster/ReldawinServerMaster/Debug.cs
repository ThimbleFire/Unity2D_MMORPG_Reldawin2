using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReldawinServerMaster
{
    class Debug
    {
        public static void Log( string message )
        {
            Console.ResetColor();
            Console.WriteLine( message );
        }

        public static void LogWarning( string message )
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine( message );
            Console.ResetColor();
        }
        public static void LogError( string message )
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine( message );
            Console.ResetColor();
        }
    }
}
