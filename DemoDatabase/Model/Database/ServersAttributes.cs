using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDatabase.Model.Database {
 
    public class ServersAttributes: XAS.Model.Database.Base {

        public Int32 ServerKey { get; set; }
        public Servers Server { get; set; }

        public Int32 AttributeKey { get; set; }
        public Attributes Attribute { get; set; }

    }

    public class ServersAttributesMapping: EntityTypeConfiguration<ServersAttributes> {

        public ServersAttributesMapping() {

            HasKey(t => t.Id);
            HasKey(t => new { t.ServerKey, t.AttributeKey });

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.ServerKey).IsRequired();
            Property(t => t.AttributeKey).IsRequired();
            Property(t => t.Revision).IsRowVersion();

            ToTable("ServersAttributes");

            // define relationships

            HasRequired(t => t.Server)
                .WithMany(t => t.Attributes)
                .HasForeignKey(t => t.ServerKey);

            HasRequired(T => T.Attribute)
                .WithMany(t => t.Servers)
                .HasForeignKey(t => t.AttributeKey);

        }

    }

}
