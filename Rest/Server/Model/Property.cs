using System;
using System.Collections.Generic;

namespace XAS.Rest.Server.Model {

    /// <summary>
    /// A class define the properties for an action.
    /// </summary>
    /// 
    public class Property {

        private string type;
        private List<string> valids = new List<string> {
            "hidden", "text", "search", "tel", "url", "email", "password",
            "datetime", "date", "month", "week", "time", "datetime-local",
            "number", "range", "color", "checkbox", "radio", "file",
            "textarea", "select", "datalist",
        };

        /// <summary>
        /// Get/Set the id.
        /// </summary>
        /// 
        public string Id { get; set; }

        /// <summary>
        /// Get/Set the name.
        /// </summary>
        /// 
        public string Name { get; set; }

        /// <summary>
        /// Get/Set wither this item is required.
        /// </summary>
        /// 
        public bool Required { get; set; }

        /// <summary>
        /// Get/Set the default value.
        /// </summary>
        /// 
        public string Default { get; set; }

        /// <summary>
        /// Get/Set the HTML regex for validation.
        /// </summary>
        /// 
        public string Pattern { get; set; }

        /// <summary>
        /// Get/Set the max length.
        /// </summary>
        /// 
        public int? MaxLength { get; set; }

        /// <summary>
        /// Get/Set the maximun value for a number.
        /// </summary>
        /// 
        public int? Max { get; set; }

        /// <summary>
        /// Get/Set the minimum value for a number.
        /// </summary>
        /// 
        public int? Min { get; set; }

        /// <summary>
        /// Get/Set the step increase for a number.
        /// </summary>
        /// 
        public int? Step { get; set; }

        /// <summary>
        /// Get/Set the options.
        /// </summary>
        /// 
        public List<string> Options { get; set; }

        /// <summary>
        /// Get/Set the HTML type.
        /// </summary>
        /// 
        public string Type {
            get {
                return this.type;
            }
            set {

                if (valids.Contains(value.ToLower())) {

                    this.type = value.ToLower();

                } else {

                    throw new ArgumentException(String.Format("{0} is not valid type", value));

                }

            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public Property() {

            this.Default = "";

        }


    }

}
