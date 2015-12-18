namespace net.willshouse.dcs.dcsbios
{
    public class Manager
    {
        private MemoryMap map;
        //private ExportListener listener;
        private ProtocolParser parser;
        public int Port { get; }
        public string Address { get;}
        public ProtocolParser Parser
        {
            get {
                return parser;
            }
        }

        public Manager( )
        {
            //listener = new ExportListener(port, address);
            Port = 5010;
            Address = "239.255.50.10";
            map = new MemoryMap();
            parser = new ProtocolParser();
            //listener.MessageReceived += parser.MessageReceivedHandler;
            parser.DcsBiosWrite += map.DcsBiosWriteHandler;
            parser.DcsBiosWrite += map.FrameSyncHandler;
            
        }

        public void Start()
        {
            //listener.Start();
        }

        public void Stop()
        {
            //listener.Stop();
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
