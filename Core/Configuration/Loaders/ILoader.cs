using System;


namespace XAS.Core.Configuration.Loaders {

    /// <summary>
    /// Public interface for a configuration loader.
    /// </summary>
    /// 
    public interface ILoader {

        void Load(IConfiguration config);

    }

}
