using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoModel.Schema {

    public class Dinosaurs: XAS.Model.Database.Base {

        public String Name { get; set; }
        public String Status { get; set; }
        public Int32 HeightInFeet { get; set; }

    }

    public class DinosaursMapping: EntityTypeConfiguration<Dinosaurs> {

        public DinosaursMapping() {

            HasKey(t => t.Id);

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Name).IsRequired().HasMaxLength(32);
            Property(t => t.Status).IsRequired().HasMaxLength(32);
            Property(t => t.HeightInFeet).IsRequired();
            Property(t => t.Revision).IsRowVersion();

            ToTable("Dinosaurs");

        }

    }

}
