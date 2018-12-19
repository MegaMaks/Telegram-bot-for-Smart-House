using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTelegram
{
    interface IKeyboardMarkup
    {
        int Number { get; set; }
        int NumberLineKeyboard { get; set; }
        string Name { get; set; }
        string Command { get; set; }
        string IconCurrent { get; set; }
    }
}
