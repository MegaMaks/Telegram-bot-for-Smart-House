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
        public static string StatusHome { get; } = $@"

🌡 Система отопления:
Давление в системе: 1.9 атм.
Температура теплоносителя: 55 °C

🚰 Система водоснабжения:
Давление в системе: 2.3 атм.
Температура горячей воды °C

🚽 Септик заполнен на 45%

🛎 Тревожные сообщения: нет";

        public static string StatusClimate { get; set; }
        public static void SetInfoClimate(List<Climate> climates,double tempstreet,double humstreet,int speedwind)
        {
            InfoMesage.StatusClimate = $@"
Температура, влажность:

🏣 гостинная {climates[0].Temperature}°C, {climates[0].Humidity}%
🚇 прихожая {climates[1].Temperature}°C, {climates[1].Humidity}%
🏝 улица {tempstreet}°C, {humstreet}%, ветер {speedwind}м/с";
        }
    }

}
