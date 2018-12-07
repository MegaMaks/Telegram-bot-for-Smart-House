using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTelegram
{
    public static class InfoMesage
    {
        public static string Starttext { get; set; } = @"Usage:/start";
        public static string Lightmode { get; set; } = $@"Режимы освещения";
        public static string Lighcontol { get; set; } = $@"Управление освещением";
        public static string FirstMsg { get; set; } = $@"Выберите категорию";

        public static string AutoOffSuccess { get; set; } = @"Вы забыли выключить свет
на улице, но можете не беспокоится,
я его выключил, 
Сладких снов!!!";
        public static string AutoOffFail { get; set; } = @"Хьюстон, на улице горит свет, 
но я не могу его выключить";
    }
}
