namespace ReldawinServerMaster
{
    internal class Program
    {
        ~Program() {
            SQLReader.Shutdown();
        }

        private static void Main( string[] args ) {
            MapData.Setup();
            SQLReader.Setup();
            ServerHandleNetworkData.InitializeNetworkPackages();
            ServerTCP.SetupServer();
            Console.WriteLine( "All good" );

            while( true ) {
                Console.ReadLine();
            }
        }
    }
}