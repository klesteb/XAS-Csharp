using System;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDatabase.Model.Database {

    public class Authentication: XAS.Model.Database.Base {

        public String Domain { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }

    }

    public class AuthenticationMapping: EntityTypeConfiguration<Authentication> {

        public AuthenticationMapping() {

            HasKey(t => t.Id);

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Domain).IsOptional().HasMaxLength(32);
            Property(t => t.Username).IsRequired().HasMaxLength(32);
            Property(t => t.Password).IsRequired().HasMaxLength(32);
            Property(t => t.Revision).IsRowVersion();

            ToTable("Autentication");

        }

    }

}
