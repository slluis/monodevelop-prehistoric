//
// System.Runtime.Remoting.Channels.Unix.UnixClientTransportSink.cs
//
// Author: Dietmar Maurer (dietmar@ximian.com)
//     Lluis Sanchez Gual (lluis@ideary.com)
//
// 2002 (C) Copyright, Ximian, Inc.
//

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
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Collections;
using System.IO;
using System.Threading;

namespace BooBinding.Remoting
{
    internal class UnixClientTransportSink : IClientChannelSink
    {
        string _path;
        string _url;
        
        public UnixClientTransportSink (string url)
        {
            string objectUri;
            _path = UnixChannel.ParseUnixURL (url, out objectUri);
            _url = url;
        }

        public IDictionary Properties
        {
            get 
            {
                return null;
            }
        }

        public IClientChannelSink NextChannelSink
        {
            get 
            {
                // we are the last one
                return null;
            }
        }

        public void AsyncProcessRequest (IClientChannelSinkStack sinkStack, IMessage msg,
                                         ITransportHeaders headers, Stream requestStream)
        {
            UnixConnection connection = null;
            bool isOneWay = RemotingServices.IsOneWay (((IMethodMessage)msg).MethodBase);

            try
            {
                if (headers == null) headers = new TransportHeaders();
                headers [CommonTransportKeys.RequestUri] = ((IMethodMessage)msg).Uri;
                
                // Sends the stream using a connection from the pool
                // and creates a WorkItem that will wait for the
                // response of the server

                connection = new UnixConnection (new UnixClient (_path));
                UnixMessageIO.SendMessageStream (connection.Stream, requestStream, headers, connection.Buffer);

                if (!isOneWay) 
                {
                    sinkStack.Push (this, connection);
                    ThreadPool.QueueUserWorkItem (new WaitCallback(ReadAsyncUnixMessage), sinkStack);
                }
                else
                    connection.Close();
            }
            catch
            {
                if (connection != null) connection.Close();
                if (!isOneWay) throw;
            }
        }

        private void ReadAsyncUnixMessage(object data)
        {
            // This method is called by a new thread to asynchronously
            // read the response to a request

            // The stack was provided as state data in QueueUserWorkItem
            IClientChannelSinkStack stack = (IClientChannelSinkStack)data;

            // The first sink in the stack is this sink. Pop it and
            // get the status data, which is the UnixConnection used to send
            // the request
            UnixConnection connection = (UnixConnection)stack.Pop(this);

            try
            {
                ITransportHeaders responseHeaders;

                // Read the response, blocking if necessary
                MessageStatus status = UnixMessageIO.ReceiveMessageStatus (connection.Stream);

                if (status != MessageStatus.MethodMessage)
                    throw new RemotingException ("Unknown response message from server");

                Stream responseStream = UnixMessageIO.ReceiveMessageStream (connection.Stream, out responseHeaders, connection.Buffer);

                // Free the connection, so it can be reused
                connection.Close();
                connection = null;

                // Ok, proceed with the other sinks
                stack.AsyncProcessResponse (responseHeaders, responseStream);
            }
            catch
            {
                if (connection != null) connection.Close();
                throw;
            }
        }
    
        public void AsyncProcessResponse (IClientResponseChannelSinkStack sinkStack,
                                          object state, ITransportHeaders headers,
                                          Stream stream)
        {
            // Should never be called
            throw new NotSupportedException();
        }

        public Stream GetRequestStream (IMessage msg, ITransportHeaders headers)
        {
            return null;
        }
        
        public void ProcessMessage (IMessage msg,
                                    ITransportHeaders requestHeaders,
                                    Stream requestStream,
                                    out ITransportHeaders responseHeaders,
                                    out Stream responseStream)
        {
            UnixConnection connection = null;
            try
            {
                if (requestHeaders == null) requestHeaders = new TransportHeaders();
                requestHeaders [CommonTransportKeys.RequestUri] = ((IMethodMessage)msg).Uri;
                
                // Sends the message
                connection = new UnixConnection (new UnixClient (_path));

                UnixMessageIO.SendMessageStream (connection.Stream, requestStream, requestHeaders, connection.Buffer);

                // Reads the response
                MessageStatus status = UnixMessageIO.ReceiveMessageStatus (connection.Stream);

                if (status != MessageStatus.MethodMessage)
                    throw new RemotingException ("Unknown response message from server, status: " + status);

                responseStream = UnixMessageIO.ReceiveMessageStream (connection.Stream, out responseHeaders, connection.Buffer);
            }
            finally
            {
                if (connection != null) 
                    connection.Close();
            }
        }

    }

    internal class UnixConnection
    {
        DateTime _controlTime;
        Stream _stream;
        UnixClient _client;
        byte[] _buffer;

        public UnixConnection (UnixClient client)
        {
            _client = client;
            _client.ReceiveTimeout = 10000; // 10 seconds
            _stream = client.GetStream();
            _controlTime = DateTime.Now;
            _buffer = new byte[UnixMessageIO.DefaultStreamBufferSize];
        }

        public UnixClient Client {
            get { return _client; }
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        public DateTime ControlTime
        {
            get { return _controlTime; }
            set { _controlTime = value; }
        }

        // This is a "thread safe" buffer that can be used by 
        // UnixClientTransportSink to read or send data to the stream.
        // The buffer is "thread safe" since only one thread can
        // use a connection at a given time.
        public byte[] Buffer
        {
            get { return _buffer; }
        }

        public void Close()
        {
            _client.Close();
        }
    }

}
