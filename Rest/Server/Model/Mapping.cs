

namespace XAS.Rest.Server.Model {

    /// <summary>
    /// A class to map the HAL rel link to an action.
    /// </summary>
    /// 
    public class Mapping {

        /// <summary>
        /// Get/Set the rel.
        /// </summary>
        /// 
        public string Rel { get; set; }

        /// <summary>
        /// Get/Set the corresponding action.
        /// </summary>
        /// 
        public string Action { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Mapping() { }

    }

}
