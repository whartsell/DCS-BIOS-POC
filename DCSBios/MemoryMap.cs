using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.willshouse.dcs.dcsbios
{
    public class MemoryMap
    {
        private Dictionary<ushort, ushort> activeMap, receiveMap, mergeMap;
        private Object mapLock;
        public MemoryMap()
        {
            activeMap = new Dictionary<ushort, ushort>();
            receiveMap = new Dictionary<ushort, ushort>();
            mapLock = new object();
        }

        public ushort GetDataAtAddress(ushort aAddress)
        {
            lock (mapLock)
            {
                if (activeMap.ContainsKey(aAddress))
                {
                    return activeMap[aAddress];
                }
                else return 0x00;
            }
        }

        public ushort[] GetDataAtAddress(ushort aAddress, int length)
        {
            lock (mapLock)
            {
                List<ushort> data = new List<ushort>();
                ushort key;
                for (int i = 0; i < length; i = i +2)
                {
                    key = (ushort)(aAddress + i);
                    
                    if (activeMap.ContainsKey(key))
                    {
                        data.Add(activeMap[key]);
                    }
                    else
                    {
                        data = new List<ushort>(0x00);
                    
                    }
                }
                
                return data.ToArray();
            }
        }

        public void writeDataAtAddress(ushort aAddress, ushort data)
        {
            lock (mapLock)
            {
                
                    receiveMap[aAddress] = data;
                
            }
        }

        public void FrameSyncHandler (object sender, EventArgs e)
        {
            lock (mapLock)
            {
                
                Dictionary<ushort, ushort> results =
                    receiveMap.Concat(activeMap.Where(kvp => !receiveMap.ContainsKey(kvp.Key))).OrderBy(c => c.Value).ToDictionary(c => c.Key, c => c.Value);
                activeMap = new Dictionary<ushort, ushort>(results);
                
            }   
        }

        public void DcsBiosWriteHandler(object sender, DcsBiosWriteEventArgs e)
        {
            lock (mapLock)
            {
                this.writeDataAtAddress(e.Address, e.Data);
            }
        }

    }
}
