using System;
using System.Collections.Generic;

namespace ConsoleTelegram
{

    class PresenceEff: Device
    {
        public PresenceEff()
        {
            Status = 0;
        }
        DateTime DateBegin { get; set; }
        DateTime DateEnd { get; set; }
        TimeSpan Interval { get; set; }
        
    }

    class AutoOff : Device
    {
        public AutoOff()
        {
            Status = 1;
            TimeBegin = new TimeSpan(0, 23, 0, 0, 0);
        }
        public TimeSpan TimeBegin { get; set; }
    }

    class LightinNight : Device
    {
        public LightinNight()
        {
            Status = 0;
        }
        int Interval { get; set; } = 60;
    }

}
