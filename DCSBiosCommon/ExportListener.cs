using System;
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
        private bool receivePending;


        
       




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
            receivePending = false;
            EndPoint senderRemote = (EndPoint)groupEP;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!receivePending)
                    {
                        socketAsyncEventArgs = new SocketAsyncEventArgs();
                        socketAsyncEventArgs.SetBuffer(new byte[256],0,256);
                        socketAsyncEventArgs.RemoteEndPoint = senderRemote;
                        socketAsyncEventArgs.Completed += SocketAsyncEventArgs_Completed;
                        //byte[] message = new byte[256];
                        //int byteCount = listener.ReceiveFrom(message, ref senderRemote);
                        receivePending = listener.ReceiveFromAsync(socketAsyncEventArgs);
                        if (!receivePending)
                        {
                            SocketAsyncEventArgs_Completed(this, socketAsyncEventArgs);
                        }

                        
                    }
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
            args.Message = e.Buffer;
            args.ByteCount = e.BytesTransferred;
            args.Sender = e.RemoteEndPoint;
            OnMessageReceived(args);
            receivePending = false;
            //throw new NotImplementedException();
        }
    }



}
