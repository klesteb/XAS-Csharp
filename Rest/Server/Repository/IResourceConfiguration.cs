using System.Collections.Generic;

using XAS.Rest.Server.Model;

namespace XAS.Rest.Server.Repository {

    /// <summary>
    /// Interface for creating HAL resources.
    /// </summary>
    /// 
    public interface IResourceConfiguration {

        List<Resource> Resources { get; set; }

    }

}
