using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.willshouse.dcs.dcsbios;

namespace TestApplication
{
    class Program
    {
        const string address = "239.255.50.10";
        const int aPort = 5010;


        static int Main(string[] args)
        {
            bool isRunning = true;
            MemoryMap map = new MemoryMap();
            ExportListener listener = new ExportListener(aPort, address);
            ProtocolParser parser = new ProtocolParser();
            Console.WriteLine("starting listener on {0}:{1}", address, aPort);
            listener.MessageReceived += parser.MessageReceivedHandler;
            parser.DcsBiosWrite += map.DcsBiosWriteHandler;
            parser.DcsBiosWrite += map.FrameSyncHandler;
            listener.Start();
            Console.WriteLine("started");
            Console.WriteLine("press any key to exit");
            ushort oldData = 65535;
            String oldCDU = "";
            ushort oldAirspeed = 65535;
            while (isRunning)
            {

                ushort data = map.GetDataAtAddress(0x1012);
                ushort masterCautionValue =(ushort) ((data & 0x0800) >> 11);

                if (data != oldData)
                {
                    Console.WriteLine("Master Caution is: {0}", masterCautionValue);
                    oldData = data;
                }

                ushort airspeed = map.GetDataAtAddress(0x107a);
                if (airspeed != oldAirspeed)
                {
                    Console.WriteLine("Airspeed:{0}", airspeed);
                    oldAirspeed = airspeed;
                }

                
                ushort[] cdu = map.GetDataAtAddress(0x11c0, 24);
                
                
                    //ushort c = cdu;
                    StringBuilder builder = new StringBuilder();
                    foreach (ushort c in cdu)
                    {
                        byte[] bytes = BitConverter.GetBytes(c);
                        //Console.WriteLine("c:{0}", c.ToString("X"));
                        builder.Append(Encoding.ASCII.GetString(bytes, 0, bytes.Length));

                    }
                if (builder.ToString() != oldCDU) {
                    Console.WriteLine("CDU Line 1:{0}", builder.ToString());
                    oldCDU = builder.ToString();
                }
                

                if (Console.KeyAvailable)
                {
                    isRunning = false;
                }
            }
            Console.WriteLine("shutting down");
            listener.Stop();
            Console.WriteLine("Listener stopped press any key to exit");
            Console.ReadKey();
            Console.ReadKey();
            return 0;
        }
    }
}
