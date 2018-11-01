using System.Data.Entity;

using XAS.Model;
using DemoModel.Schema;

namespace DemoModel {

    public class Context: XAS.Model.Context {

        // Your context has been configured to use a 'Model' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'DemoDatabase' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model' 
        // connection string in the application configuration file.

        public Context(Initializer initializer, string model): base(initializer, model) { }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Dinosaurs> Dinosaurs { get; set; }

    }

}
