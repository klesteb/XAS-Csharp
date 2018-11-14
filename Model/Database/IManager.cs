using System;
using System.Data.Entity;

namespace XAS.Model.Database {

    public interface IManager {

        DbContext Context { get; set; }
        Repositories Repository { get; set; }

    }

}
