using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ConsoleTelegram
{
    class ComPort
    {
        static SerialPort SP1;

        public ComPort()
        {
            SP1 = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        }

        
        //serialPort1.Open();

        //    serialPort1.Write("st");
        //    string str = serialPort1.ReadLine();
        //bt = Convert.ToByte(str);
    }
}
