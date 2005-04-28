//
// System.Runtime.Remoting.Channels.Unix.UnixChannel.cs
//
// Author: Rodrigo Moya (rodrigo@ximian.com)
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
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;
using System.Text.RegularExpressions;

namespace BooBinding.Remoting
{
    public class UnixChannel : IChannelReceiver, IChannel, IChannelSender
    {
        private UnixClientChannel _clientChannel;
        private UnixServerChannel _serverChannel = null;
        private string _name = "unix";
        private int _priority = 1;
    
        public UnixChannel (): this ("")
        {
        }

        public UnixChannel (string path)
        {
            Hashtable ht = new Hashtable();
            ht["path"] = path;
            Init(ht, null, null);
        }

        void Init (IDictionary properties, IClientChannelSinkProvider clientSink, IServerChannelSinkProvider serverSink)
        {
            _clientChannel = new UnixClientChannel (properties,clientSink);

            if(properties["path"] != null)
                _serverChannel = new UnixServerChannel(properties, serverSink);
            
            object val = properties ["name"];
            if (val != null) _name = val as string;
            
            val = properties ["priority"];
            if (val != null) _priority = Convert.ToInt32 (val);
        }


        public UnixChannel (IDictionary properties,
                            IClientChannelSinkProvider clientSinkProvider,
                            IServerChannelSinkProvider serverSinkProvider)
        {
            Init (properties, clientSinkProvider, serverSinkProvider);
        }

        public IMessageSink CreateMessageSink(string url, object remoteChannelData, out string objectURI)
        {
            return _clientChannel.CreateMessageSink(url, remoteChannelData, out objectURI);
        }

        public string ChannelName
        {
            get { return _name; }
        }

        public int ChannelPriority
        {
            get { return _priority; }
        }

        public void StartListening (object data)
        {
            if (_serverChannel != null) _serverChannel.StartListening (data);
        }
        
        public void StopListening (object data)
        {
            if (_serverChannel != null) _serverChannel.StopListening(data);
        }

        public string[] GetUrlsForUri (string uri)
        {
            if (_serverChannel != null) return _serverChannel.GetUrlsForUri(uri);
            else return null;
        }

        public object ChannelData
        {
            get 
            {
                if (_serverChannel != null) return _serverChannel.ChannelData;
                else return null;
            }
        }

        public string Parse (string url, out string objectURI)
        {
            return UnixChannel.ParseUnixURL (url, out objectURI);
        }

        internal static string ParseUnixURL (string url, out string objectURI)
        {
            // format: "unix:///path/to/unix/socket?/path/to/object"
            
            objectURI = null;
            
            Match m = Regex.Match (url, "unix://?([^?]*)[?]?(.*)$");

            if (!m.Success)
                return null;
            
            string sockPath = m.Groups[1].Value;
            objectURI = m.Groups[2].Value;

            
            if (objectURI == string.Empty)
                objectURI = null;

            return sockPath;
        }
    }
}
