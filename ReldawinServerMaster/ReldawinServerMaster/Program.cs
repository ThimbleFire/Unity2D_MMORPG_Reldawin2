using System;
using System.Threading;

namespace ReldawinServerMaster
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            StartServer();

            bool quitting = false;

            while ( quitting == false )
            {
                string input = Console.ReadLine().ToLower();

                switch ( input )
                {
                    case "start":
                        StartServer();
                        break;
                    case "help":
                        PrintHelp();
                        break;
                    case "quit":
                    case "close":
                    case "end":
                        quitting = true;
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "toggle debug":
                        CommandsToggleDebug();
                        break;
                }

                if ( SQLReader.IsRunning() )
                {
                    if ( input.Contains( "generate" ) )
                        CommandGenerate( "generate".Length, input );

                    if ( input.Contains( "setwidth" ) )
                        CommandSetWidth( "setwidth".Length, input );

                    if ( input.Contains( "setheight" ) )
                        CommandSetHeight("setheight".Length, input );

                    if ( input.Contains( "setsize" ) )
                        CommandSetSize( "setsize".Length, input );

                    if ( input.Contains( "setseed" ) )
                        CommandSetSeed( "setseed".Length, input );

                    if ( input.Contains( "setscale" ) )
                        CommandSetScale( "setscale".Length, input );

                    if ( input.Contains( "save" ) )
                        CommandSave( "save".Length, input );
                }
            }
        }

        private static void CommandsToggleDebug()
        {
            Properties.Config.Debugging = !Properties.Config.Debugging;
            Console.WriteLine( "\n[Program] Debugging set to {0}", Properties.Config.Debugging );
        }

        private static void CommandGenerate(int length, string input)
        {
            try
            {
                int.TryParse( input.Substring( length, input.Length - length ), out int seed );
                MapGen.Generate( World.Width, World.Height, World.Scale, false, seed );
                World.Seed = seed;
                Console.WriteLine( "\n[Program] Operation TRUE" );
            }
            catch ( Exception )
            {
                MapGen.Generate( World.Width, World.Height, World.Scale, false );
                Console.WriteLine( "\n[Program] Operation TRUE" );
            }
        }

        private static void CommandSetSize( int length, string input )
        {
            try
            {
                int.TryParse( input.Substring( length, input.Length - length ), out int setsize );
                SQLReader.RunQuery( string.Format( @"UPDATE mapdefaults SET Height = {0};", setsize ), out bool executed );
                SQLReader.RunQuery( string.Format( @"UPDATE mapdefaults SET Width = {0};", setsize ), out executed );
                World.Height = setsize;
                Console.WriteLine( "\n[Program] Operation TRUE" );
            }
            catch ( Exception ) { Console.WriteLine( "\n[Program] Operation FALSE. \nsetsize takes 1 parameter type <int>." ); }
        }

        private static void CommandSave( int length, string input )
        {
            try
            {
                int.TryParse( input.Substring( length, input.Length - length ), out int seed );
                World.Seed = seed;
                World.LoadNewMap();
                Console.WriteLine( "\n[Program] Operation TRUE" );
                CommonSQL.EmptyTable( "doodads" );
            }
            catch ( Exception ) { Console.WriteLine( "\n[Program] Operation FALSE. \nsave takes 1 parameter type <int>." ); }
        }

        private static void CommandSetScale( int length, string input )
        {
            try
            {
                double.TryParse( input.Substring( length, input.Length - length ), out double setscale );
                SQLReader.RunQuery( string.Format( @"UPDATE mapdefaults SET Scale = {0};", setscale ), out bool executed );
                World.Scale = setscale;
                Console.WriteLine( "\n[Program] Operation TRUE" );
            }
            catch ( Exception ) { Console.WriteLine( "\n[Program] Operation FALSE. \nsetscale takes 1 parameter type <int>." ); }
        }

        private static void CommandSetSeed( int length, string input )
        {
            try
            {
                int.TryParse( input.Substring( length, input.Length - length ), out int setseed );
                SQLReader.RunQuery( string.Format( @"UPDATE mapdefaults SET Seed = {0};", setseed ), out bool executed );
                World.Seed = setseed;
                Console.WriteLine( "\n[Program] Operation TRUE" );
            }
            catch ( Exception ) { Console.WriteLine( "\n[Program] Operation FALSE. \nsetseed takes 1 parameter type <int>." ); }
        }

        private static void CommandSetHeight( int length, string input )
        {
            try
            {
                int.TryParse( input.Substring( length, input.Length - length ), out int setheight );
                SQLReader.RunQuery( string.Format( @"UPDATE mapdefaults SET Height = {0};", setheight ), out bool executed  );
                World.Height = setheight;
                Console.WriteLine( "\n[Program] Operation TRUE" );
            }
            catch ( Exception ) { Console.WriteLine( "\n[Program] Operation FALSE. \nsetheight takes 1 parameter type <int>." ); }
        }

        private static void CommandSetWidth( int length, string input )
        {
            try
            {
                int.TryParse( input.Substring( length, input.Length - length ), out int setwidth );
                SQLReader.RunQuery( string.Format( @"UPDATE mapdefaults SET Width = {0};", setwidth ), out bool executed  );
                World.Width = setwidth;
                Console.WriteLine( "\n[Program] Operation TRUE" );
            }
            catch ( Exception ) { Console.WriteLine( "\n[Program] Operation FALSE. \nsetwidth takes 1 parameter type <int>." ); }
        }

        private static void PrintHelp()
        {
                Console.WriteLine( "Command List" +
           "\n Commands are not case sensitive. " +
           "\n Usernames are interchangible with IDs. " +
           "\n" +
           "\n                   Start - Launches the server" +
           "\n                   Close - Closes the application." +
           "\n                   Clear - Clears the console window." +
           "\n                   Count - Returns the count of online players." +
           "\n            Toggle Debug - Returns the milliseconds of certain server commands." +
           "\n             WhoIsOnline - Returns a list of every single online player." +
           "\n     Broadcast <message> - Sends a message to every player character." +
           "\n /p <username> <message> - Sends a private message to the specified user." +
           "\n         Kick <Username> - Safely terminates the specified user's connection." +
           "\n         SetWidth X      - Changes the world width." +
           "\n         SetHeight X     - Changes the world height" +
           "\n         SetSize X       - Changes the world height and world width" +
           "\n         SetSeed X       - Changes the world seed" +
           "\n         SetScale X      - Changes the world scale" +
           "\n         Generate        - Creates a new map" +
           "\n         Save            - Creates a new map" +
           "\n         DeleteMap       - Deletes all map data on the database\n\n" );
        }

        private static void StartServer()
        {
            SQLReader.Setup();

            while ( SQLReader.IsRunning() == false )
            {
                Console.WriteLine( "Failed to establish a connection to MySQL database" );
                Countdown(5);
                SQLReader.Setup();
            }

            Console.Clear();
            Console.WriteLine( "[program] setup complete, SQLReader" );
            SQLReader.IntegrityCheck();
            Console.WriteLine( "[Program] check complete, integrity check" );
            XMLDevice.Setup();
            Console.WriteLine( "[Program] setup complete, XMLDevice" );
            World.Setup();
            Console.WriteLine( "[Program] Setup complete, world" );
            ServerHandleNetworkData.InitializeNetworkPackages();
            Console.WriteLine( "[Program] Setup complete, network packages" );
            ServerTCP.SetupServer();
            Console.WriteLine( "[Program] Setup complete, Server" );

            Console.WriteLine( "\n\n[Program] Type 'Help' for command list." );
        }

        private static void Countdown(int count)
        {
            Console.WriteLine( "Trying again in {0} seconds", count );

            for ( int i = count; i > 0; i-- )
            {
                Thread.Sleep( 1000 );

            }

            return;
        }
    }
}
