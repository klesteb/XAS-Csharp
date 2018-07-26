using System;
using System.Collections.Generic;

namespace XAS.Rest.Server.Model {

    /// <summary>
    /// Used to define parameters for an action.
    /// </summary>
    /// 
    public class Parameter {

        private string type;
        private List<string> valids = new List<string> { 
            "string", "number", "datetime", "list"
        };

        /// <summary>
        /// Get/Set the name.
        /// </summary>
        /// 
        public string Name { get; set; }

        /// <summary>
        /// Get/Set the default value.
        /// </summary>
        /// 
        public string Default { get; set; }

        /// <summary>
        /// Get/Set the possible values (comma delimited list).
        /// </summary>
        /// 
        public string Possible { get; set; }

        /// <summary>
        /// Get/Set the HTML type.
        /// </summary>
        /// 
        public string Type {
            get { return this.type; }
            set {

                if (valids.Contains(value.ToLower())) {

                    this.type = value.ToLower();

                } else {

                    throw new ArgumentException(String.Format("{0} is not valid type", value));

                }

            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Parameter() { }

    }

}
