namespace ReldawinServerMaster
{ 
    internal class Program
    {
        private static byte selectedPlayerIndex = 0;

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

                if( command == "selectplayer" ) {
                    Console.WriteLine( "Specify num" );
                    selectedPlayerIndex = byte.Parse( Console.ReadLine() );
                    Console.WriteLine( $"{selectedPlayerIndex} selected." );
                    continue;
                }

                if( selectedPlayerIndex > byte.MaxValue )
                    continue;

                if( command == "setmf" ) {
                    Console.WriteLine( "Specify num between 0 (inclusive) and 256 (exclusive)" );
                    ServerTCP.clients[selectedPlayerIndex].properties.MagicFind = byte.Parse( Console.ReadLine() );
                    Console.WriteLine( $"MF has been set to {ServerTCP.clients[selectedPlayerIndex].properties.MagicFind}" );
                }

                if( command == "setlvl" ) {
                    Console.WriteLine( "Specify num between 1 and 61" );
                    ServerTCP.clients[selectedPlayerIndex].properties.CharacterLevel = byte.Parse( Console.ReadLine() );
                    Console.WriteLine( $"Level has been set to {ServerTCP.clients[selectedPlayerIndex].properties.CharacterLevel}" );
                }

                if( command == "rrl" ) {
                    List<byte> data = ItemFactory.RollGlobalLootTable( ServerTCP.clients[selectedPlayerIndex].properties );
                    ServerTCP.PlaceItemInInventory( selectedPlayerIndex, data );
                    foreach( byte item in data ) {
                        Console.WriteLine( Convert.ToString( item, 2 ).PadLeft( 8, '0' ) );
                    }

                    continue;
                }
            }
        }
    }
}