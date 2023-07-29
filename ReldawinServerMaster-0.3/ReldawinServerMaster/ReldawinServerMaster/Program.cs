namespace ReldawinServerMaster
{
    internal class Program
    {
        ~Program() {
            SQLReader.Shutdown();
        }

        private static void Main( string[] args ) {
            SQLReader.Setup();

            ServerHandleNetworkData.InitializeNetworkPackages();
            Console.WriteLine( "[Program] Setup complete, network packages" );

            ServerTCP.SetupServer();
            Console.WriteLine( "[Program] Setup complete, Server" );

            while( true ) {
                Console.ReadLine();
            }
        }
    }
}