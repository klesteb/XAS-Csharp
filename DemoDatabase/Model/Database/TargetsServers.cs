using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDatabase.Model.Database {
 
    public class TargetsServers: XAS.Model.Database.Base {

        public Int32 ServerKey { get; set; }
        public Servers Server { get; set; }

        public Int32 TargetKey { get; set; }
        public Targets Target { get; set; }

    }

    public class TargetsServersMapping: EntityTypeConfiguration<TargetsServers> {

        public TargetsServersMapping() {

            HasKey(t => t.Id);

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.ServerKey).IsRequired();
            Property(t => t.TargetKey).IsRequired();
            Property(t => t.Revision).IsRowVersion();

            ToTable("TargetsServers");

            // define relationships

            HasRequired(t => t.Server)
                .WithMany(t => t.Targets)
                .HasForeignKey(t => t.ServerKey);

            HasRequired(T => T.Target)
                .WithMany(t => t.Servers)
                .HasForeignKey(t => t.TargetKey);

        }

    }

}
