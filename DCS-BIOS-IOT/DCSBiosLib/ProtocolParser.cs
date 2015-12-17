using System;

namespace net.willshouse.dcs.dcsbios
{
    public enum DcsBiosState
    {
        WaitForSync = 0,
        AddressLow = 1,
        AddressHigh = 2,
        CountLow = 3,
        CountHigh = 4,
        DataLow = 5,
        DataHigh = 6
    } 

    public class DcsBiosWriteEventArgs : EventArgs
    {
        public ushort Address { get; set; }
        public ushort Data { get; set; }
    }

    public class ProtocolParser
    {

        public event EventHandler<DcsBiosWriteEventArgs>  DcsBiosWrite;
        public event EventHandler DcsBiosFrameSync;

        private DcsBiosState state;
        private ushort address;
        private ushort count;
        private ushort data;
        private byte sync_byte_count;

        public ProtocolParser()
        {
            state = DcsBiosState.WaitForSync;
            sync_byte_count = 0;
        }

        public void processChar(byte c)
        {
            switch (state)
            {
                case DcsBiosState.WaitForSync:
                    break;

                case DcsBiosState.AddressLow:
                    address = c;
                    state = DcsBiosState.AddressHigh;
                    break;

                case DcsBiosState.AddressHigh:
                    address = (ushort)((c << 8) | address);
                    if (address != 0x5555)
                        state = DcsBiosState.CountLow;
                    else
                        state = DcsBiosState.WaitForSync;
                    break;

                case DcsBiosState.CountLow:
                    count = c;
                    state = DcsBiosState.CountHigh;
                    break;

                case DcsBiosState.CountHigh:
                    count = (ushort)((c << 8) | count);
                    state = DcsBiosState.DataLow;
                    break;

                case DcsBiosState.DataLow:
                    data = c;
                    count--;
                    state = DcsBiosState.DataHigh;
                    break;

                case DcsBiosState.DataHigh:
                    data = (ushort)((c << 8) | data);
                    count--;
                    //onDCSBiosWrite would go here
                    // were ready to do something with the data
                    // exportStreamListener:handleDCSBioswrite goes here

                    //if (address == 0x1012)
                    //    Console. WriteLine("Found IT address:" + address.ToString("X") + "  value:" + data.ToString("X"));
                    DcsBiosWriteEventArgs args = new DcsBiosWriteEventArgs();
                    args.Address = address;
                    args.Data = data;
                    OnDcsBiosWrite(args);
                    address += 2;
                    if (count == 0)
                        state = DcsBiosState.AddressLow;
                    else
                        state = DcsBiosState.DataLow;
                    break;
            }

            if (c == 0x55)
                sync_byte_count++;
            else
                sync_byte_count = 0;

            if (sync_byte_count == 4)
            {
                state = DcsBiosState.AddressLow;
                sync_byte_count = 0;
                // handle frame sync here
                OnDcsBiosFrameSync(EventArgs.Empty);
            }
        }

        //public void MessageReceivedHandler(object sender, MessageReceivedEventArgs e)
        //{
        //    //Console.WriteLine("MessageReceivedHandler");
        //    foreach (byte b in e.Message) {
        //        this.processChar(b);
        //    }
        //}

        protected virtual void OnDcsBiosWrite(DcsBiosWriteEventArgs e)
        {
            //Console.WriteLine("OnBiosWrite");
            EventHandler<DcsBiosWriteEventArgs> handler = DcsBiosWrite;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnDcsBiosFrameSync(EventArgs e)
        {
           // Console.WriteLine("OnDCSBiosFrameSync");
            EventHandler handler = DcsBiosFrameSync;
            if (handler != null)
                handler(this, e);
        }

    }
}
