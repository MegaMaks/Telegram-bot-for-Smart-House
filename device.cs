using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTelegram
{
    public class Device
    {
        private int status;
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value == 0)
                {
                    IconCurrent = "⚪️";
                    status = 0;
                }
                else if(value==1)
                {
                    IconCurrent = "🎾";
                    status = 1;
                }
            }
        } 
        public string IconCurrent { get; set; }

    }
}
