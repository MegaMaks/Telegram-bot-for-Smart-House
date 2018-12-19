using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ConsoleTelegram.Model
{
    class SmartHouseDbInit:DropCreateDatabaseIfModelChanges<SmartContext>
    {

        protected override void Seed(SmartContext context)
        {
            base.Seed(context);
        }
    }
}
