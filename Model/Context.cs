using System;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace XAS.Model {

    public class Context: DbContext {

        private readonly Initializer initializer = null;

        // Your context has been configured to use a 'Model' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'DemoDatabase3' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model' 
        // connection string in the application configuration file.

        public Context(Initializer initializer, String model): base(model) {

            this.initializer = initializer;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            // autoload the mappings

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !String.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType
                 && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

            foreach (var type in typesToRegister) {

                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);

            }

            // Turn off cascade delete (shotgun approach)
            // redefine in the mapping classes on a per table basis

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Create the database if it dosen't exist.

            if (initializer != null) {

                initializer.ModelBuilder = modelBuilder;
                System.Data.Entity.Database.SetInitializer(initializer);

            }

            //var initializer = new Initializer(config, handler, logFactory, modelBuilder);

            base.OnModelCreating(modelBuilder);

        }

    }

}
