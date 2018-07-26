using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;

namespace XAS.Core.DataStructures {

    /// <summary>
    /// DynmicObjects
    /// </summary>
    /// 
    public class DynamicObjects: DynamicObject {

        /// <summary>
        /// Internal properties
        /// </summary>
        /// 
        protected Dictionary<string, object> properties = null;

        /// <summary>
        /// Returns the properties names.
        /// </summary>
        /// 
        public List<String> Properties {
            get { return properties.Keys.ToList(); }
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// 
        public DynamicObjects() : base() {

            properties = new Dictionary<string, object>();

        }

        /// <summary>
        /// Trys to retrieve the value of a property.
        /// </summary>
        /// <param name="binder">Represents the dynamic set member operation at the call site, providing the binding semantic and the details about the operation.</param>
        /// <param name="result">The stored value associated with the property.</param>
        /// <returns>Returns true if successful.</returns>
        /// 
        public override Boolean TryGetMember(GetMemberBinder binder, out Object result) {

            bool stat = false;
            var name = binder.Name;

            result = this.GetProperty(name);

            if (result != null) {

                stat = true;

            }

            return stat;

        }

        /// <summary>
        /// Trys to create a property and set its value.
        /// </summary>
        /// <param name="binder">Represents the dynamic set member operation at the call site, providing the binding semantic and the details about the operation.</param>
        /// <param name="value">The value to associate with the property.</param>
        /// <returns>Return true if successful.</returns>
        /// 
        public override Boolean TrySetMember(SetMemberBinder binder, Object value) {

            this.AddProperty(binder.Name, value);

            return true;

        }

        /// <summary>
        /// Add the property name to the internal dictionary.
        /// </summary>
        /// <param name="name">The name of the proeprty.</param>
        /// <param name="value">The associated value.</param>
        /// 
        public virtual void AddProperty(String name, object value) {

            if (properties.ContainsKey(name)) {

                properties[name] = value;

            } else {

                properties.Add(name, value);

            }

        }

        /// <summary>
        /// Return the value of property
        /// </summary>
        /// <param name="name">the name of the property.</param>
        /// <returns>the value associated with the prperty.</returns>
        /// 
        public virtual Object GetProperty(String name) {

            object value = null;

            if (properties.ContainsKey(name)) {

                value = properties[name];

            }

            return value;

        }
        
        /// <summary>
        /// Remove the property from the internal dictionary.
        /// </summary>
        /// <param name="name">The name of the propery.</param>
        /// 
        public virtual void RemoveProperty(String name) {

            if (properties.ContainsKey(name)) {

                properties.Remove(name);

            }

        }

        /// <summary>
        /// Add the method name to the internal dictionay.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="action">The delegate, associated with the name.</param>
        /// 
        public virtual void AddMethod(String name, Delegate action) {

            this.AddProperty(name, action);

        }

        /// <summary>
        /// Remove the method from the internal dictionay.
        /// </summary>
        /// <param name="name">The methods name.</param>
        /// 
        public virtual void RemoveMethod(String name) {

            this.RemoveProperty(name);

        }

    }

}
