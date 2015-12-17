
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Net.Sockets;
using System.Net;

namespace net.willshouse.dcs.dcsbios
{
    class Listener
    {

        //private Socket s;
        private DateTime prevTime;
        private int delay;
        private Manager manager;
        private HostName address;
        private string port;
        private DatagramSocket socket;
        private IOutputStream outputStream;
        //private UdpSocketMulticastClient receiver;
        public bool Pulse { get; set; }
        public Listener(Manager manager)
        {
            prevTime = DateTime.Now;
            Pulse = false;
            delay = 1;
            this.manager = manager;
            
            address = new HostName(manager.Address);
            port = manager.Port.ToString();
            socket = new DatagramSocket();
        }

         async public void Start()
        {
            IPAddress local = IPAddress.Parse("192.168.1.2");
            //s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //s.Bind(new IPEndPoint(local, 5010));
            
            //mcastOption = new MulticastOption(manager.Address, local);
            socket.MessageReceived += messageReceived;
            socket.Control.MulticastOnly = true;
            await socket.BindServiceNameAsync(port);
            socket.JoinMulticastGroup(address);
            outputStream = await socket.GetOutputStreamAsync(address, port);
            const string stringToSend = "Hello";
            DataWriter writer = new DataWriter(outputStream);
            writer.WriteString(stringToSend);
            await writer.StoreAsync();
           
        }

        public void Stop()
        {
            socket.Dispose();
        }

         public void messageReceived(object s,DatagramSocketMessageReceivedEventArgs e)
        {

            DataReader message = e.GetDataReader();
            while (message.UnconsumedBufferLength > 0)
            {
                manager.Parser.processChar(message.ReadByte());
            }
            Send();

        }

        async private void Send()
        {
            outputStream = await socket.GetOutputStreamAsync(address, port);
            const string stringToSend = "Hello";
            DataWriter writer = new DataWriter(outputStream);
            writer.WriteString(stringToSend);
            await writer.StoreAsync();
        }
        public void ping()
        {

            DateTime curTime = DateTime.Now;
            TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks - prevTime.Ticks);
            if (elapsed.Seconds > delay)
            {
                prevTime = curTime;
                Pulse = !Pulse;
            }
            

            
        }
        
    }
}
