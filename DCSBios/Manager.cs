using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.willshouse.dcs.dcsbios
{
    public class Manager
    {
        private MemoryMap map;
        private ExportListener listener;
        private ProtocolParser parser;
        int Port { get; }
        string Address { get;}

        public Manager(string address,int port)
        {
            listener = new ExportListener(port, address);
            map = new MemoryMap();
            parser = new ProtocolParser();
            listener.MessageReceived += parser.MessageReceivedHandler;
            parser.DcsBiosWrite += map.DcsBiosWriteHandler;
            parser.DcsBiosWrite += map.FrameSyncHandler;
            
        }

        public void Start()
        {
            listener.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }

        public ushort getDataAtAddress(ushort address) 
        {
            return map.GetDataAtAddress(address);
        }

        public ushort[] getDataAtAddress(ushort address,int length)
        {
            return map.GetDataAtAddress(address, length);
        }
    }
}
