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
                string command = Console.ReadLine();

                switch( command ) {
                    case "":
                        ItemFactory.Generate( 1, out byte[] data );
                        for( int i = 0; i < data.Length; i++ ) {
                            Console.WriteLine( Convert.ToString( data[i], 2 ).PadLeft( 8, '0' ) );
                        }
                        break;
                }
            }
        }
    }
}