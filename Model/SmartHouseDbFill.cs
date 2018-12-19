using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTelegram.Model
{
    public static class SmartHouseDbFill
    {
        public static List<Lamp> lamps = new List<Lamp>
        {
            new Lamp(){Number=0, NumberLineKeyboard=1, Name=" Гостинная",Command="living" },
            new Lamp(){Number=1, NumberLineKeyboard=1, Name="   Кабинет",Command="study" },
            new Lamp(){Number=2, NumberLineKeyboard=2, Name="         Кухня",Command="kitchen" },
            new Lamp(){Number=3, NumberLineKeyboard=2, Name="   Детская",Command="child" },
            new Lamp(){Number=4, NumberLineKeyboard=3, Name="   Коридор",Command="hall" },
            new Lamp(){Number=5, NumberLineKeyboard=3, Name="    Ванная" ,Command="bath"},
            new Lamp(){Number=6, NumberLineKeyboard=4, Name=" Прихожая" ,Command="nobody"},
            new Lamp(){Number=7, NumberLineKeyboard=4, Name="    Улица" ,Command="street"},
        };

        public static List<AutoOffMode> autooff = new List<AutoOffMode>
        {
            new AutoOffMode(){AutoOffModeID=1,TimeBegin=new TimeSpan(0,0,0,0,0), Status=false,Command="am1"},
            new AutoOffMode(){AutoOffModeID=2,TimeBegin=new TimeSpan(0,0,0,0,0), Status=false,Command="am2"},
            new AutoOffMode(){AutoOffModeID=3,TimeBegin=new TimeSpan(0,0,0,0,0), Status=false,Command="am3"},
            new AutoOffMode(){AutoOffModeID=4,TimeBegin=new TimeSpan(0,0,0,0,0), Status=false,Command="am4"},
            new AutoOffMode(){AutoOffModeID=5,TimeBegin=new TimeSpan(0,0,0,0,0), Status=false,Command="am5"},
            new AutoOffMode(){AutoOffModeID=6,TimeBegin=new TimeSpan(0,0,0,0,0), Status=false,Command="am6"},
            new AutoOffMode(){AutoOffModeID=7,TimeBegin=new TimeSpan(0,0,0,0,0), Status=false,Command="am7"},
            new AutoOffMode(){AutoOffModeID=8,TimeBegin=new TimeSpan(0,0,0,0,0), Status=false,Command="am8"},
        };
        public static void DbInit()
        {
            using (var shc = new Model.SmartContext())
            {
                shc.Lamps.AddRange(Model.SmartHouseDbFill.lamps);
                shc.SaveChanges();
                shc.AutoOffMode.AddRange(Model.SmartHouseDbFill.autooff);
                shc.SaveChanges();

            }
            Console.WriteLine("Base initialized");
            Console.ReadLine();
        }
    }
}
