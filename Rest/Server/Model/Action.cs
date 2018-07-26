using System.Collections.Generic;

namespace XAS.Rest.Server.Model {

    /// <summary>
    /// Define an action that can be taken by a HAL link
    /// </summary>
    /// 
    public class Action {

        /// <summary>
        /// Get/Set the name.
        /// </summary>
        ///                
        public string Name { get; set; }

        /// <summary>
        /// Get/Set the content type.
        /// </summary>
        /// 
        public string Type { get; set; }

        /// <summary>
        /// Get/Set the accept type.
        /// </summary>
        /// 
        public string Accept { get; set; }

        /// <summary>
        /// Get/Set the HTTP method.
        /// </summary>
        /// 
        public string Method { get; set; }

        /// <summary>
        /// Get/Set the possible fields for this action.
        /// </summary>
        /// 
        public List<Field> Fields { get; set; }

        /// <summary>
        /// Get/Set the possible parametrs for this action.
        /// </summary>
        /// 
        public List<Parameter> Parameters { get; set; }

        /// <summary>
        /// Get/Set the possible properties for this action.
        /// </summary>
        /// 
        public List<Property> Properties { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Action() { }

    }

}
