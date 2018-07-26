using System.Collections.Generic;

using XAS.Rest.Server.Model;

namespace XAS.Rest.Server.Repository {

    /// <summary>
    /// A class to create HAL resources.
    /// </summary>
    /// 
    public class ResourceConfiguration: IResourceConfiguration {

        /// <summary>
        /// Get/Set Resources.
        /// </summary>
        /// 
        public List<Resource> Resources { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public ResourceConfiguration() { }

    }

}
