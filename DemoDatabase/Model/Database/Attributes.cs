using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDatabase.Model.Database {

    public class Attributes: XAS.Model.Database.Base {

        public String Name { get; set; }
        public String Type { get; set; }
        public String StrValue { get; set; }
        public Int32 NumValue { get; set; }

        // relationships

        public virtual ICollection<ServersAttributes> Servers { get; set; }

    }

    public class AttributesMapping: EntityTypeConfiguration<Attributes> {

        public AttributesMapping() {

            HasKey(t => t.Id);

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Name).IsRequired().HasMaxLength(32);
            Property(t => t.Type).IsRequired().HasMaxLength(32);
            Property(t => t.StrValue).IsOptional().HasMaxLength(256);
            Property(t => t.NumValue).IsOptional();
            Property(t => t.Revision).IsRowVersion();

            ToTable("Attributes");

        }

    }

}
