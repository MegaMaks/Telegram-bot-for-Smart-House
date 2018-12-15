using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTelegram
{
    public class Climate
    {
        public Climate()
        {

        }
        public void SetClimate(string tempandhum)
        {
            string str = tempandhum;
            str = str.Replace("\r", null);
            NumSensor = Convert.ToInt32(str[1].ToString());
            str = str.Remove(0, 3);
            int currint = str.IndexOf('T');
            Humidity = Convert.ToInt32(str.Substring(0, currint));
            str = str.Remove(0, currint + 1);
            Temperature = Convert.ToInt32(str);
        }
        public int NumSensor { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }



    }
}
