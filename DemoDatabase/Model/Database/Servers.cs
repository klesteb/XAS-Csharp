using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDatabase.Model.Database {

    public class Servers: XAS.Model.Database.Base {

        public String Name { get; set; }

        // relationships

        [ForeignKey("Authentication")]
        public Int32 AuthenticationKey { get; set; }
        public virtual Authentication Authentication { get; set; }

        public virtual ICollection<TargetsServers> Targets { get; set; }
        public virtual ICollection<ServersAttributes> Attributes { get; set; }

    }

    public class ServersMapping: EntityTypeConfiguration<Servers> {

        public ServersMapping() {

            HasKey(t => t.Id);

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Name).IsRequired().HasMaxLength(32);
            Property(t => t.Revision).IsRowVersion();

            ToTable("Servers");

        }

    }

}
