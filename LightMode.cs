using System;
using System.Collections.Generic;

namespace ConsoleTelegram
{
    class LightMode: Device
    {
        List<Lamp> selectedlamps = new List<Lamp>();
    }

    class PresenceEff:LightMode
    {
        public PresenceEff()
        {
            Status = 0;
        }
        DateTime DateBegin { get; set; }
        DateTime DateEnd { get; set; }
        TimeSpan Interval { get; set; }
        
    }

    class AutoOff : LightMode
    {
        public AutoOff()
        {
            Status = 1;
            TimeBegin = new TimeSpan(0, 23, 0, 0, 0);
        }
        public TimeSpan TimeBegin { get; set; }
    }

    class LightinNight : LightMode
    {
        public LightinNight()
        {
            Status = 0;
        }
        int Interval { get; set; } = 60;
    }

}
