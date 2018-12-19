using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTelegram
{
    public class DeviceStatus
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
        //[InverseProperty("DeviceStatus")]
        //public virtual ICollection<Lamp> Lamps { get; set; }
        //public virtual ICollection<AutoOffMode> AutoOffMode { get; set; }

    }
}
