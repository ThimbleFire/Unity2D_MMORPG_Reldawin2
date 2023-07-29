namespace ReldawinServerMaster
{
    internal class Program
    {
        ~Program() {
            SQLReader.Shutdown();
        }

        private static void Main( string[] args ) {
            SQLReader.Setup();
            MapData.Setup();
            ServerHandleNetworkData.InitializeNetworkPackages();
            ServerTCP.SetupServer();
            Console.WriteLine( "All good" );

            while( true ) {
                Console.ReadLine();
            }
        }
    }
}