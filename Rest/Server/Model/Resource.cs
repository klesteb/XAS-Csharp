using System.Collections.Generic;

namespace XAS.Rest.Server.Model {

    /// <summary>
    /// A class to define a resource for an action.
    /// </summary>
    /// 
    public class Resource {

        /// <summary>
        /// Get/Set the href for this resource.
        /// </summary>
        /// 
        public string Href { get; set;  }

        /// <summary>
        /// Get/Set wither authentication is needed, default is false.
        /// </summary>
        /// 
        public bool Authenticate { get; set; }

        /// <summary>
        /// Get/Set the actions for this resource.
        /// </summary>
        /// 
        public List<Action> Actions { get; set; }

        /// <summary>
        /// Get/Set the mappings for this resource.
        /// </summary>
        /// 
        public List<Mapping> Mappings { get; set; }

        /// <summary>
        /// Get/Set the template parameters for the href.
        /// </summary>
        /// 
        public List<Template> Templates { get; set; }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// 
        public Resource() {

            this.Authenticate = false;

        }

    }

}
