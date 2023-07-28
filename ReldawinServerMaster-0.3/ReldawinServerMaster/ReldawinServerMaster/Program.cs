using SQLite;
using System;
using System.Security.AccessControl;
using System.Threading;
using System.Xml;

namespace ReldawinServerMaster
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            SQLReader.Setup();

            ServerHandleNetworkData.InitializeNetworkPackages();
            Console.WriteLine( "[Program] Setup complete, network packages" );

            ServerTCP.SetupServer();
            Console.WriteLine( "[Program] Setup complete, Server" );

            while( true ) {

                Console.ReadLine();

            }
        }

        ~Program() {
            SQLReader.Shutdown();
        }
    }
}
