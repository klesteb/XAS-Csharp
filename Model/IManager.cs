using System;
using System.Data.Entity;

namespace XAS.Model {

    public interface IManager {

        DbContext Context { get; set; }
        Repositories Repository { get; set; }

    }

}
