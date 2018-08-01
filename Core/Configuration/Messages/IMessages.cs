using System;


namespace XAS.Core.Configuration.Messages {

    /// <summary>
    /// Public interface for a message loader.
    /// </summary>
    /// 
    public interface IMessages {

        void Load(IConfiguration config);

    }

}
