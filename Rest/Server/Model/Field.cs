using System;
using System.Collections.Generic;

namespace XAS.Rest.Server.Model {

    /// <summary>
    /// Used to define fields for a resource.
    /// </summary>

    public class Field {

        private string _type;
        private List<string> valids = new List<string> {
            "hidden", "text", "search", "tel", "url", "email", "password", 
            "datetime", "date", "month", "week", "time", "datetime-local", 
            "number", "boolean"
        };

        /// <summary>
        /// Get/Set the name of the field.
        /// </summary>
        /// 
        public string Name { get; set; }

        /// <summary>
        /// Get/Set the html type of the field.
        /// </summary>
        /// 
        public string Type {
            get {
                return this._type;
            }
            set {

                if (valids.Contains(value.ToLower())) {

                    this._type = value.ToLower();

                } else {

                    throw new ArgumentException(String.Format("{0} is not valid type", value));

                }

            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Field() { }

    }

}
