

/* 
* The purpose of this program is to provide a minimal example of using UDP to 
* receive data.
* It picks up broadcast packets and displays the text in a console window.
* This was created to work with the program UDP_Minimum_Talker.
* Run both programs, send data with Talker, receive the data with Listener.
* Run multiple copies of each on multiple computers, within the same LAN of course.
* If the broadcast packet contains numbers or binary data or anything other than 
* plain text it may well crash and burn. 
* Adding code to handle unexpected conditions such as that would defeat the 
* simplistic nature of this example program. So would adding code for a gracefull
* exit. Just kill it.
*/
using multicastListener;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
public class UDPListener
{
    private const int listenPort = 5010;
    
    public static string ByteArrayToString(byte[] ba)
    {
        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", "");
    }

    public static int Main()
    {
        bool done = false;
        ProtocolParser parser = new ProtocolParser();
        UdpClient listener = new UdpClient(listenPort);
        IPAddress group = IPAddress.Parse("239.255.50.10");
        listener.JoinMulticastGroup(group);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
        string received_data;
        byte[] receive_byte_array;

        parser.DcsBiosWrite += DcsBiosWrite;

        

        //parser.DcsBiosFrameSync += FrameSyncHandler;
        try
        {
            while (!done)
            {
                //Console.WriteLine("Waiting for broadcast");
                // this is the line of code that receives the broadcase message.
                // It calls the receive function from the object listener (class UdpClient)
                // It passes to listener the end point groupEP.
                // It puts the data from the broadcast message into the byte array
                // named received_byte_array.
                // I don't know why this uses the class UdpClient and IPEndPoint like this.
                // Contrast this with the talker code. It does not pass by reference.
                // Note that this is a synchronous or blocking call.
                receive_byte_array = listener.Receive(ref groupEP);
                //Console.WriteLine("Received a broadcast from {0}", groupEP.ToString());
                received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                //Console.WriteLine("data follows \n{0}\n\n", ByteArrayToString(receive_byte_array));
                foreach (byte c in receive_byte_array)
                {
                    parser.processChar(c);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        listener.Close();
        return 0;
    }

    static void DcsBiosWrite(object sender, DcsBiosWriteEventArgs e)
    {
        if (e.Address >= 0x11c0 & e.Address <= (0x11c0 + 24))
        {
            byte[] data = BitConverter.GetBytes(e.Data);
            String sData = Encoding.ASCII.GetString(data, 0, data.Length);
            Console.WriteLine("Found IT address:" + e.Address.ToString("X") + "  value:" +sData);
        }
    }

    static void FrameSyncHandler(object sender,EventArgs e)
    {
        Console.WriteLine("FrameSync");
    }
} // end of class UDPListener