using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsoleTelegram
{

    public class StatusMode
    {
        private bool status;
        public bool Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value == false)
                {
                    IconCurrent = "⚪️";
                    status = false;
                }
                else if (value)
                {
                    IconCurrent = "🎾";
                    status = true;
                }
            }
        }
        public string IconCurrent { get; set; }
    }

    public class PresenceEff: StatusMode
    {
        public int PresenceEffID { get; set; }
        public PresenceEff()
        {
            Status = true;
        }
        DateTime DateBegin { get; set; }
        DateTime DateEnd { get; set; }
        TimeSpan Interval { get; set; }

        public virtual ICollection<Lamp> Lamps { get; set; }
    }

    public class AutoOffMode : StatusMode
    {
        public int AutoOffModeID { get; set; }
        public AutoOffMode()
        {
            Status = true;
            TimeBegin = new TimeSpan(0, 23, 0, 0, 0);
        }        
        public TimeSpan TimeBegin { get; set; }

        [MaxLength(50)]
        public string Command { get; set; }

        public virtual Lamp Lamp { get; set; }
    }

    public class LightinNight : StatusMode
    {
        public int LightinNightID { get; set; }
        public LightinNight()
        {
            Status = false;
        }
        int Interval { get; set; } = 60;

        public virtual ICollection<Lamp> Lamps { get; set; }
    }

}
