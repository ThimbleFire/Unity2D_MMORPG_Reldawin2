using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    public class PacketBuffer : IDisposable
    {
        private List<byte> bufferList;
        private byte[] readBuffer;
        private int readPosition;
        private bool bufferUpdate = false;

        public PacketBuffer()
        {
            bufferList = new List<byte>();
            readPosition = 0;
        }

        public PacketBuffer(Packet packet)
        {
            bufferList = new List<byte>();
            readPosition = 0;

            WriteInteger( (int)packet );
        }

        public int GetReadPosition()
        {
            return readPosition;
        }

        public byte[] ToArray()
        {
            return bufferList.ToArray();
        }

        public int Count()
        {
            return bufferList.Count;
        }

        public int Length()
        {
            return Count() - readPosition;
        }

        public void Clear()
        {
            bufferList.Clear();
            readPosition = 0;
        }

        // Write Data
        public void WriteBytes( byte[] input )
        {
            bufferList.AddRange( input );
            bufferUpdate = true;
        }
        public void WriteBoolean(bool input)
        {
            bufferList.AddRange( BitConverter.GetBytes(input) );
            bufferUpdate = true;
        }
        public void WriteByte( byte input )
        {
            bufferList.Add( input );
            bufferUpdate = true;
        }
        public void WriteInteger( int input )
        {
            bufferList.AddRange( BitConverter.GetBytes( input ) );
            bufferUpdate = true;
        }
        public void WriteFloat( float input )
        {
            bufferList.AddRange( BitConverter.GetBytes( input ) );
            bufferUpdate = true;
        }
        public void WriteString( string input )
        {
            bufferList.AddRange( BitConverter.GetBytes( input.Length ) );
            bufferList.AddRange( Encoding.ASCII.GetBytes( input ) );
            bufferUpdate = true;
        }

        // Read Data
        public int ReadInteger( bool peek = true )
        {
            if ( bufferList.Count > readPosition )
            {
                if ( bufferUpdate )
                {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }

                int value = BitConverter.ToInt32( readBuffer, readPosition );

                bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

                if ( IsThereAdditionalData )
                {
                    readPosition += 4;
                }

                return value;
            }
            else
            {
                throw new Exception( "Buffer is past its limit" );
            }
        }
        public float ReadFloat( bool peek = true )
        {
            if ( bufferList.Count > readPosition )
            {
                if ( bufferUpdate )
                {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }

                float value = BitConverter.ToSingle( readBuffer, readPosition );

                bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

                if ( IsThereAdditionalData )
                {
                    readPosition += 4;
                }

                return value;
            }
            else
            {
                throw new Exception( "Buffer is past its limit" );
            }
        }
        public byte ReadByte( bool peek = true )
        {
            if ( bufferList.Count > readPosition )
            {
                if ( bufferUpdate )
                {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }

                byte value = readBuffer[readPosition];

                bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

                if ( IsThereAdditionalData )
                {
                    readPosition += 1;
                }

                return value;
            }
            else
            {
                throw new Exception( "Buffer is past its limit" );
            }
        }
        public byte[] ReadBytes( int length, bool peek = true )
        {
            if ( bufferUpdate )
            {
                readBuffer = bufferList.ToArray();
                bufferUpdate = false;
            }

            byte[] value = bufferList.GetRange( readPosition, length ).ToArray();

            bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

            if ( IsThereAdditionalData )
            {
                readPosition += length;
            }

            return value;
        }
        public string ReadString( bool peek = true )
        {
            int length = ReadInteger( true );

            if ( bufferUpdate )
            {
                readBuffer = bufferList.ToArray();
                bufferUpdate = false;
            }

            string value = Encoding.ASCII.GetString( readBuffer, readPosition, length );

            bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

            if ( IsThereAdditionalData )
            {
                readPosition += length;
            }

            return value;
        }
        public bool ReadBoolean(bool peek = true)
        {
            return Convert.ToBoolean( ReadByte( peek ) );
        }

        // IDisposable
        private bool disposedValue = false;
        protected virtual void Dispose( bool disposing )
        {
            if ( !disposedValue )
            {

                if ( disposing )
                {
                    bufferList.Clear();
                }
            }
            readPosition = 0;
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}