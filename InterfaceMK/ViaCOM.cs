using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ConsoleTelegram.InterfaceMK
{
    class ViaCOM
    {
        public static string SendtoCOM(string port, int pin)
        {
            string timehum = "";
            SerialPort serialPort1 = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            serialPort1.Open();

            serialPort1.Write(pin.ToString());
            timehum += serialPort1.ReadLine();
            timehum += serialPort1.ReadLine();
            timehum += serialPort1.ReadLine();
            serialPort1.Close();

            return timehum;
        }
    }
}
