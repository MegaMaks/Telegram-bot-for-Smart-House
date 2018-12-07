using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ConsoleTelegram.InterfaceMK
{
    class ViaTCP
    {
        public static byte SendCMD(TcpClient newClient, byte[] sendBytes)
        {
            NetworkStream tcpStream = newClient.GetStream();
            tcpStream.Write(sendBytes, 0, sendBytes.Length);

            byte[] bytes = new byte[newClient.ReceiveBufferSize];
            int bytesRead = tcpStream.Read(bytes, 0, newClient.ReceiveBufferSize);
            return bytes[1];
        }
    }
}
