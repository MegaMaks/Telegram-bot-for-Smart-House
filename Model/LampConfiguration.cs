using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace ConsoleTelegram.Model
{
    class LampConfiguration:EntityTypeConfiguration<Lamp>
    {
        public LampConfiguration()
        {
            this.HasOptional(s => s.AutoOffMode)
                .WithRequired(ad => ad.Lamp);
        }

    }
}
