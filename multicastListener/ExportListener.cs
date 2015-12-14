using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace net.willshouse.dcs.dcsbios
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public byte[] Message { get; set; }
    }

    public class ExportListener
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private UdpClient listener;
        private IPEndPoint endPoint;
        private int port;
        private IPAddress multicastGroup;
        private IPEndPoint groupEP;
        private Thread listenThread;
        private bool isListening;

        
       




        public ExportListener(int aPort,String aMulticastAddress)
        {
            isListening = false;
            port = aPort;
            multicastGroup = IPAddress.Parse(aMulticastAddress);
            groupEP = new IPEndPoint(IPAddress.Any, port);
           
        }

        public void Start()
        {
            if (!isListening)
            {
                listener = new UdpClient(port);
                listener.JoinMulticastGroup(multicastGroup);
                listenThread = new Thread(listen);
                listenThread.Start();
                isListening = true;
               
            }

        }

        public void Stop()
        {
            if (isListening)
            {
                isListening = false;
                listener.DropMulticastGroup(multicastGroup);
                listener.Close();
            }

        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            //Console.WriteLine("OnMessageReceived");
            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
                handler(this, e);
        }

        private void listen()
        {
            while (isListening)
            {
                try
                {
                    byte[] message = listener.Receive(ref groupEP);
                    MessageReceivedEventArgs args = new MessageReceivedEventArgs();
                    args.Message = message;
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
