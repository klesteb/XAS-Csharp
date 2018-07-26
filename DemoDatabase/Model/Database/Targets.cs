using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDatabase.Model.Database {

    public class Targets: XAS.Model.Database.Base {

        public String Name { get; set; }

        // relationships

        public virtual ICollection<GroupsTargets> Groups { get; set; }
        public virtual ICollection<TargetsServers> Servers { get; set; }

    }

    public class TargetsMapping: EntityTypeConfiguration<Targets> {

        public TargetsMapping() {

            HasKey(t => t.Id);

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Name).IsRequired().HasMaxLength(32);
            Property(t => t.Revision).IsRowVersion();

            ToTable("Targets");

        }

    }

}
