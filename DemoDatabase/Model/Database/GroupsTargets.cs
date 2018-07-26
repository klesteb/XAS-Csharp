using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDatabase.Model.Database {
 
    public class GroupsTargets: XAS.Model.Database.Base {

        public Int32 GroupKey { get; set; }
        public Groups Group { get; set; }

        public Int32 TargetKey { get; set; }
        public Targets Target { get; set; }

    }

    public class GroupsTargetsMapping: EntityTypeConfiguration<GroupsTargets> {

        public GroupsTargetsMapping() {

            HasKey(t => t.Id);

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.GroupKey).IsRequired();
            Property(t => t.TargetKey).IsRequired();
            Property(t => t.Revision).IsRowVersion();

            ToTable("GroupsTargets");

            // define relationships

            HasRequired(t => t.Group)
                .WithMany(t => t.Targets)
                .HasForeignKey(t => t.GroupKey);

            HasRequired(T => T.Target)
                .WithMany(t => t.Groups)
                .HasForeignKey(t => t.TargetKey);

        }

    }

}
