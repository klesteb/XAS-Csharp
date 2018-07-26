using System;
using Nancy;

namespace XAS.Rest.Server.Modules {

    /// <summary>
    /// Implement a base module that incorparates the WPM.Base singletons().
    /// </summary>
    /// 
    public class Base: NancyModule {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Base(): base() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Base(String url): base(url) { }

    }

}
