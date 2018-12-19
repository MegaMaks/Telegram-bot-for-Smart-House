using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTelegram
{
    public class Lamp:DeviceStatus,IKeyboardMarkup
    {
        public int LampID { get; set; }
        public int Number { get; set; }
        public int NumberLineKeyboard { get; set; }
        public int Brightness { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Command { get; set; }

        //public int DeviceStatusID { get; set; }

        //[ForeignKey("DeviceStatusID")]
        //[InverseProperty("Lamps")]
        //public virtual DeviceStatus DeviceStatus { get; set; }
        //public new int? DeviceStatusID { get; set; }
        public virtual AutoOffMode AutoOffMode { get; set; }
        public virtual ICollection<PresenceEff> PresenceEff { get; set; }

        public virtual ICollection<LightinNight> LightinNight { get; set; }
    }
}
