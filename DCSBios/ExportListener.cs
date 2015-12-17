﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace net.willshouse.dcs.dcsbios
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public byte[] Message { get; set; }
        public int ByteCount { get; set; }
        public EndPoint Sender { get; set; }
    }

    public class ExportListener
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        
        private Socket listener;
        private int port;
        private IPAddress address;
        private IPEndPoint groupEP;
        private bool isListening;
        private CancellationTokenSource tokenSource;
        private SocketAsyncEventArgs socketAsyncEventArgs;
        private bool waitForReceive;








        public ExportListener(int aPort,String aAddress)
        {
            isListening = false;
            port = aPort;
            address = IPAddress.Parse(aAddress);
            groupEP = new IPEndPoint(IPAddress.Any, port);
            tokenSource = new CancellationTokenSource();
           
        }

        public void Start()
        {
            if (!isListening)
            {
                
               
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                listener.Bind(new IPEndPoint(IPAddress.Any, port));
                Task.Run(() =>listen(tokenSource.Token), tokenSource.Token);
            }

        }

        public void Stop()
        {
            tokenSource.Cancel();
            listener.Dispose();

        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {

            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
                handler(this, e);
        }

        private void listen(CancellationToken token)
        {
             waitForReceive = false;
           
          
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!waitForReceive)
                    {
                        socketAsyncEventArgs = new SocketAsyncEventArgs();
                        socketAsyncEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, port); 
                        socketAsyncEventArgs.SetBuffer(new byte[256], 0, 256);
                        socketAsyncEventArgs.Completed += SocketAsyncEventArgs_Completed;
                        waitForReceive = listener.ReceiveFromAsync(socketAsyncEventArgs);
                        if (!waitForReceive)
                        {
                            SocketAsyncEventArgs_Completed(this, socketAsyncEventArgs);
                        }
                        else waitForReceive = true;
                        //EndPoint senderRemote = (EndPoint)groupEP;

                    }
                    
                    
                    
                    //int byteCount = listener.ReceiveFrom(message, ref senderRemote);
                    
                    
                    
                }
                catch (SocketException e)
                {
                    
                    isListening = false;
                    
                }


            }

        }

        private void SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            MessageReceivedEventArgs args = new MessageReceivedEventArgs();
            args.Message = socketAsyncEventArgs.Buffer;
            args.ByteCount = socketAsyncEventArgs.BytesTransferred;
            args.Sender = socketAsyncEventArgs.RemoteEndPoint;
            OnMessageReceived(args);
            waitForReceive = false;
        }

        
    }



}
