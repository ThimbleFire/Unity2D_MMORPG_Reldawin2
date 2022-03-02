using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace ReldawinServerMaster
{
    internal class SQLReader
    {
        private static MySqlConnection[] connectionPool;
        public enum Stream
        {
            INPUT,
            OUTPUT,
            INPUT_BULK
        };

        public static bool IsRunning()
        {
            int i = GetConnectionFromPool();

            try
            {
                connectionPool[i].Open();
            }
            catch ( Exception )
            {
                connectionPool[i].Close();
                return false;
            }
            finally
            {
                connectionPool[i].Close();
            }

            return true;
        }

        public static object RunQueryScalar( string sqlCommand, Parameter[] parameters = null )
        {
            int i = GetConnectionFromPool();
            connectionPool[i].Open();
            object myReader = null;

            try
            {
                // Set the query command
                MySqlCommand cmd = connectionPool[i].CreateCommand();
                cmd.CommandText = sqlCommand;

                // Add parameters where applicable
                if ( parameters != null )
                    foreach ( Parameter p in parameters )
                        cmd.Parameters.AddWithValue( p.parameterName, p.value );

                // Run the query and yield results
                myReader = cmd.ExecuteScalar();
            }
            catch ( Exception e )
            {
                Console.WriteLine( "[SQLReader] " + "Query error: " + e.Message );
            }
            finally
            {
                if ( connectionPool[i].State == System.Data.ConnectionState.Open )
                {
                    connectionPool[i].Close();
                }
            }

            return myReader;
        }

        public static List<object> RunQuery( string sqlCommand, out bool executed, Stream stream = Stream.INPUT, Parameter[] parameters = null, params string[] columns )
        {
            List<object> result = null;

            int i = GetConnectionFromPool();
            connectionPool[i].Open();
            MySqlDataReader myReader = null;

            try
            {
                // Set the query command
                MySqlCommand cmd = connectionPool[i].CreateCommand();
                cmd.CommandText = sqlCommand;

                // Add parameters where applicable
                if ( parameters != null )
                    foreach ( Parameter p in parameters )
                        cmd.Parameters.AddWithValue( p.parameterName, p.value );

                // Run the query and yield results
                myReader = cmd.ExecuteReader();

                executed = myReader != null ? true : false;

                if ( stream == Stream.OUTPUT )
                {
                    // If results have rows...
                    if ( myReader.HasRows )
                    {
                        result = new List<object>();

                        while ( myReader.Read() )
                        {
                            // Get the field values for each of the specified column names
                            foreach ( string column in columns )
                            {
                                result.Add( myReader.GetString( column ) );
                            }
                        }
                    }
                }
            }
            catch ( Exception e )
            {
                executed = false;
                Console.WriteLine( "[SQLReader] " + "Query error: " + e.Message );
            }
            finally
            {
                myReader.Dispose();
                if ( connectionPool[i].State == System.Data.ConnectionState.Open )
                {
                    connectionPool[i].Close();
                }
            }


            return result;
        }

        public static void Setup()
        {
            connectionPool = new MySqlConnection[5];

            for ( int i = 0; i < connectionPool.Length; i++ )
            {
                connectionPool[i] = new MySqlConnection
                (
                    "Database=reldawin;" +
                    "Data Source=localhost;" +
                    "User Id=root;" +
                    //"Password=WTfLtGA49KRhrTp;" +
                    "Port=3306;"
                );
            }

            Console.WriteLine( "[SQLReader] " + "ConnectionPool created." );
        }

        private static int GetConnectionFromPool()
        {
            for ( int i = 0; i < connectionPool.Length; i++ )
            {
                if ( connectionPool[i].State == System.Data.ConnectionState.Closed )
                    return i;
            }

            return -1;
        }

        public struct Parameter
        {
            public string parameterName;

            public object value;

            public Parameter( string parameterName, object value )
            {
                this.parameterName = parameterName;
                this.value = value;
            }
        }

        public static void IntegrityCheck()
        {
            DBIntegrityCheck.MapDefaults();
            DBIntegrityCheck.Entities();
            DBIntegrityCheck.Players();
            DBIntegrityCheck.Terrain();
            DBIntegrityCheck.Doodads();
        }
    }
}