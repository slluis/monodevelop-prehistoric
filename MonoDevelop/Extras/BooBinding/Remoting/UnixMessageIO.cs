// System.Runtime.Remoting.Channels.Unix.UnixMessageIO.cs
//
// Author: Lluis Sanchez Gual (lluis@ideary.com)
//
// (C) 2002 Lluis Sanchez Gual

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;

namespace BooBinding.Remoting 
{
    enum MessageStatus { MethodMessage = 0, CancelSignal = 1, Unknown = 10}

    internal class UnixMessageIO
    {
        static byte[][] _msgHeaders = 
            {
                new byte[] { (byte)'.', (byte)'N', (byte)'E', (byte)'T', 1, 0 },
                new byte[] { 255, 255, 255, 255, 255, 255 }
            };
                            
        public static int DefaultStreamBufferSize = 1000;

        // Identifies an incoming message
        public static MessageStatus ReceiveMessageStatus (Stream networkStream)
        {
            try
            {
                bool[] isOnTrack = new bool[_msgHeaders.Length];
                bool atLeastOneOnTrack = true;
                int i = 0;

                while (atLeastOneOnTrack)
                {
                    atLeastOneOnTrack = false;
                    byte c = (byte)networkStream.ReadByte();
                    for (int n = 0; n<_msgHeaders.Length; n++)
                    {
                        if (i > 0 && !isOnTrack[n]) continue;

                        isOnTrack[n] = (c == _msgHeaders[n][i]);
                        if (isOnTrack[n] && (i == _msgHeaders[n].Length-1)) return (MessageStatus) n;
                        atLeastOneOnTrack = atLeastOneOnTrack || isOnTrack[n];
                    }
                    i++;
                }
                return MessageStatus.Unknown;
            }
            catch (IOException e)
            {
                // Stream closed
                return MessageStatus.CancelSignal;
            }
        }

        public static void SendMessageStream (Stream networkStream, Stream data, ITransportHeaders requestHeaders, byte[] buffer)
        {
            if (buffer == null) buffer = new byte[DefaultStreamBufferSize];

            // Writes the message start header
            byte[] dotnetHeader = _msgHeaders[(int) MessageStatus.MethodMessage];
            networkStream.Write(dotnetHeader, 0, dotnetHeader.Length);

            // Writes header tag (0x0000 if request stream, 0x0002 if response stream)
            if(requestHeaders[CommonTransportKeys.RequestUri]!=null) buffer [0] = (byte) 0;
            else buffer[0] = (byte) 2;
            buffer [1] = (byte) 0 ;

            // Writes ID
            buffer [2] = (byte) 0;

            // Writes assemblyID????
            buffer [3] = (byte) 0;

            // Writes the length of the stream being sent (not including the headers)
            int num = (int)data.Length;
            buffer [4] = (byte) num;
            buffer [5] = (byte) (num >> 8);
            buffer [6] = (byte) (num >> 16);
            buffer [7] = (byte) (num >> 24);
            networkStream.Write(buffer, 0, 8);
    
            // Writes the message headers
            SendHeaders (networkStream, requestHeaders, buffer);

            // Writes the stream
            if (data is MemoryStream)
            {
                // The copy of the stream can be optimized. The internal
                // buffer of MemoryStream can be used.
                MemoryStream memStream = (MemoryStream)data;
                networkStream.Write (memStream.GetBuffer(), 0, (int)memStream.Length);
            }
            else
            {
                int nread = data.Read (buffer, 0, buffer.Length);
                while (nread > 0)
                {
                    networkStream.Write (buffer, 0, nread);
                    nread = data.Read (buffer, 0, buffer.Length);
                }
            }
        }

        private static void SendHeaders(Stream networkStream, ITransportHeaders requestHeaders, byte[] buffer)
        {
            // Writes the headers as a sequence of strings
            if (networkStream != null)
            {
                IEnumerator e = requestHeaders.GetEnumerator();
                while (e.MoveNext())
                {
                    DictionaryEntry hdr = (DictionaryEntry)e.Current;
                    switch (hdr.Key.ToString())
                    {
                    case CommonTransportKeys.RequestUri: 
                        buffer[0] = 4; buffer[1] = 0; buffer[2] = 1;
                        networkStream.Write(buffer, 0, 3);
                        break;
                    case "Content-Type": 
                        buffer[0] = 6; buffer[1] = 0; buffer[2] = 1;
                        networkStream.Write(buffer, 0, 3);
                        break;
                    default: 
                        buffer[0] = 1; buffer[1] = 0; buffer[2] = 1;
                        networkStream.Write(buffer, 0, 3);
                        SendString (networkStream, hdr.Key.ToString(), buffer);
                        break;
                    }
                    networkStream.WriteByte (1);
                    SendString (networkStream, hdr.Value.ToString(), buffer);
                }
            }
            networkStream.WriteByte (0);    // End of headers
            networkStream.WriteByte (0);
        }
        
        public static ITransportHeaders ReceiveHeaders (Stream networkStream, byte[] buffer)
        {
            byte headerType;
            headerType = (byte) networkStream.ReadByte ();
            networkStream.ReadByte ();

            TransportHeaders headers = new TransportHeaders ();

            while (headerType != 0)
            {
                string key;
                networkStream.ReadByte ();    // byte 1
                switch (headerType)
                {
                case 4: key = CommonTransportKeys.RequestUri; break;
                case 6: key = "Content-Type"; break;
                case 1: key = ReceiveString (networkStream, buffer); break;
                default: throw new NotSupportedException ("Unknown header code: " + headerType);
                }
                networkStream.ReadByte ();    // byte 1
                headers[key] = ReceiveString (networkStream, buffer);

                headerType = (byte) networkStream.ReadByte ();
                networkStream.ReadByte ();
            }

            return headers;
        }
        
        public static Stream ReceiveMessageStream (Stream networkStream, out ITransportHeaders headers, byte[] buffer)
        {
            headers = null;

            if (buffer == null) buffer = new byte[DefaultStreamBufferSize];

            // Reads header tag:  0 -> Stream with headers or 2 -> Response Stream
            byte head = (byte)networkStream.ReadByte();
            byte c = (byte)networkStream.ReadByte();

            c = (byte)networkStream.ReadByte();
            c = (byte)networkStream.ReadByte();

            // Gets the length of the data stream
            int nr = 0;
            while (nr < 4)
                nr += networkStream.Read (buffer, nr, 4 - nr);

            int byteCount = (buffer [0] | (buffer [1] << 8) |
                             (buffer [2] << 16) | (buffer [3] << 24));

            // Reads the headers
            headers = ReceiveHeaders (networkStream, buffer);

            byte[] resultBuffer = new byte[byteCount];

            nr = 0;
            while (nr < byteCount)
                nr += networkStream.Read (resultBuffer, nr, byteCount - nr);

            
            return new MemoryStream(resultBuffer);
        }        

        private static void SendString (Stream networkStream, string str, byte[] buffer)
        {
            // Allocates a buffer. Use the internal buffer if it is 
            // big enough. If not, create a new one.

            int maxBytes = Encoding.UTF8.GetMaxByteCount(str.Length)+4;    //+4 bytes for storing the string length
            if (maxBytes > buffer.Length)
                buffer = new byte[maxBytes];

            int num = Encoding.UTF8.GetBytes (str, 0, str.Length, buffer, 4);

            // store number of bytes (not number of chars!)

            buffer [0] = (byte) num;
            buffer [1] = (byte) (num >> 8);
            buffer [2] = (byte) (num >> 16);
            buffer [3] = (byte) (num >> 24);

            // Write the string bytes
            networkStream.Write (buffer, 0, num + 4);
        }

        private static string ReceiveString (Stream networkStream, byte[] buffer)
        {
            int nr = 0;
            while (nr < 4)
                nr += networkStream.Read (buffer, nr, 4 - nr);

            // Reads the number of bytes (not chars!)

            int byteCount = (buffer [0] | (buffer [1] << 8) |
                             (buffer [2] << 16) | (buffer [3] << 24));

            if (byteCount == 0) return string.Empty;

            // Allocates a buffer of the correct size. Use the
            // internal buffer if it is big enough

            if (byteCount > buffer.Length)
                buffer = new byte[byteCount];

            // Reads the string

            nr = 0;
            while (nr < byteCount)
                nr += networkStream.Read (buffer, nr, byteCount - nr);

            char[] chars = Encoding.UTF8.GetChars(buffer, 0, byteCount);
    
            return new string(chars);
        }
        
    }
}
