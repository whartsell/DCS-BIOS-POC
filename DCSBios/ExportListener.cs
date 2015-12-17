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
                listener.Bind(new IPEndPoint(address, port));
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
            while (!token.IsCancellationRequested)
            {
                try
                {
                    
                    EndPoint senderRemote = (EndPoint)groupEP;
                    byte[] message = new byte[256];
                    int byteCount = listener.ReceiveFrom(message, ref senderRemote);
                    MessageReceivedEventArgs args = new MessageReceivedEventArgs();
                    args.Message = message;
                    args.ByteCount = byteCount;
                    args.Sender = senderRemote;
                    OnMessageReceived(args);
                    
                }
                catch (SocketException e)
                {
                    
                    isListening = false;
                    
                }
            }

        }
    }



}
