﻿using System.Text;

namespace Bindings
{
    public class PacketBuffer : IDisposable
    {
        private List<byte> bufferList;
        private bool bufferUpdate = false;
        // IDisposable
        private bool disposedValue = false;

        private byte[] readBuffer;
        private int readPosition = 0;

        ///<summary>Packet buffer for reading</summary>
        public PacketBuffer( byte[] data) {
            bufferList = new List<byte>();
            readPosition = 0;
            WriteBytes( data );
        }

        ///<summary>Packet buffer for writing</summary>
        public PacketBuffer( Packet packet ) {
            bufferList = new List<byte>();
            readPosition = 0;
            WriteInteger( ( int )packet );
        }

        public void Clear() {
            bufferList.Clear();
            readPosition = 0;
        }

        public int Count() {
            return bufferList.Count;
        }

        public void Dispose() {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public int GetReadPosition { get { return readPosition; } }
        public int Length { get { return Count() - readPosition; } }
        
        public bool ReadBoolean( bool peek = true ) {
            return Convert.ToBoolean( ReadByte( peek ) );
        }

        public byte ReadByte( bool peek = true ) {
            if( bufferList.Count > readPosition ) {
                if( bufferUpdate ) {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }

                byte value = readBuffer[readPosition];

                bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

                if( IsThereAdditionalData ) {
                    readPosition += 1;
                }

                return value;
            } else {
                throw new Exception( "Buffer is past its limit" );
            }
        }

        public byte[] ReadBytes( int length, bool peek = true ) {
            if( bufferUpdate ) {
                readBuffer = bufferList.ToArray();
                bufferUpdate = false;
            }

            byte[] value = bufferList.GetRange( readPosition, length ).ToArray();

            bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

            if( IsThereAdditionalData ) {
                readPosition += length;
            }

            return value;
        }

        public float ReadFloat( bool peek = true ) {
            if( bufferList.Count > readPosition ) {
                if( bufferUpdate ) {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }

                float value = BitConverter.ToSingle( readBuffer, readPosition );

                bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

                if( IsThereAdditionalData ) {
                    readPosition += 4;
                }

                return value;
            } else {
                throw new Exception( "Buffer is past its limit" );
            }
        }

        // Read Data
        public int ReadInteger( bool peek = true ) {
            if( bufferList.Count > readPosition ) {
                if( bufferUpdate ) {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }

                int value = BitConverter.ToInt32( readBuffer, readPosition );

                bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

                if( IsThereAdditionalData ) {
                    readPosition += 4;
                }

                return value;
            } else {
                throw new Exception( "Buffer is past its limit" );
            }
        }

        public string ReadString( bool peek = true ) {
            int length = ReadInteger( true );

            if( bufferUpdate ) {
                readBuffer = bufferList.ToArray();
                bufferUpdate = false;
            }

            string value = Encoding.ASCII.GetString( readBuffer, readPosition, length );

            bool IsThereAdditionalData = peek && bufferList.Count > readPosition;

            if( IsThereAdditionalData ) {
                readPosition += length;
            }

            return value;
        }

        public byte[] ToArray() {
            return bufferList.ToArray();
        }
        public void WriteBoolean( bool input ) {
            bufferList.AddRange( BitConverter.GetBytes( input ) );
            bufferUpdate = true;
        }

        public void WriteByte( byte input ) {
            bufferList.Add( input );
            bufferUpdate = true;
        }

        // Write Data
        public void WriteBytes( byte[] input ) {
            bufferList.AddRange( input );
            bufferUpdate = true;
        }
        public void WriteFloat( float input ) {
            bufferList.AddRange( BitConverter.GetBytes( input ) );
            bufferUpdate = true;
        }

        public void WriteInteger( int input ) {
            bufferList.AddRange( BitConverter.GetBytes( input ) );
            bufferUpdate = true;
        }
        public void WriteString( string input ) {
            bufferList.AddRange( BitConverter.GetBytes( input.Length ) );
            bufferList.AddRange( Encoding.ASCII.GetBytes( input ) );
            bufferUpdate = true;
        }
        protected virtual void Dispose( bool disposing ) {
            if( !disposedValue ) {
                if( disposing ) {
                    bufferList.Clear();
                }
            }
            readPosition = 0;
            disposedValue = true;
        }
    }
}