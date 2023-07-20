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
            Console.WriteLine( "[program] setup complete, SQLReader" );
            SQLReader.IntegrityCheck();
            Console.WriteLine( "[Program] check complete, integrity check" );
            
            //XMLDevice.Setup();
            //Console.WriteLine( "[Program] setup complete, XMLDevice" );
            
            //World.Setup();
            //Console.WriteLine( "[Program] Setup complete, world" );
            
            ServerHandleNetworkData.InitializeNetworkPackages();
            //Console.WriteLine( "[Program] Setup complete, network packages" );
            
            ServerTCP.SetupServer();
            //Console.WriteLine( "[Program] Setup complete, Server" );

            while( true ) {

                Console.ReadLine();

            }
        }
    }
}
