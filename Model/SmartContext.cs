using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleTelegram.Model
{
    class SmartContext:DbContext
    {
        public SmartContext() : base("DBConnection")
        {
            Database.SetInitializer<SmartContext>(new SmartHouseDbInit());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LampConfiguration());
            //modelBuilder.Entity<Lamp>().Property(m => m.IconCurrent)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //modelBuilder.Entity<Lamp>().Property(m => m.DeviceStatusID)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            //modelBuilder.Entity<DeviceStatus>().Map(m =>
            //{
            //    //m.MapInheritedProperties();
            //    m.ToTable("DeviceStatus");

            //});

            //modelBuilder.Entity<Lamp>().Map(m =>
            //{
            //    //m.MapInheritedProperties();
            //    m.ToTable("Lamps");
            //});


            //modelBuilder.Entity<DeviceStatus>()
            //    .HasKey(t => t.DeviceStatusID);
            //modelBuilder.Entity<Lamp>()
            //    .HasKey(t => t.LampID);

            //modelBuilder.Entity<AutoOffMode>()
            //    .ToTable("AutoOffMode");
            //Adds configurations for Student from separate class
            //modelBuilder.Configurations.Add(new StudentConfigurations());

            //modelBuilder.Entity<Teacher>()
            //    .ToTable("TeacherInfo");

            //modelBuilder.Entity<Teacher>()
            //    .MapToStoredProcedures();
        }

        public DbSet<Lamp> Lamps { get; set; }
        public DbSet<AutoOffMode> AutoOffMode { get; set; }
        //public DbSet<DeviceStatus> DeviseStatus { get; set; }
    }

}
