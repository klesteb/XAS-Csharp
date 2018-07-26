using Nancy;

namespace XAS.Rest.Server {

    /// <summary>
    /// Where the root directory is for content.
    /// </summary>
    /// 
    public class AppRootPathProvider: IRootPathProvider {

        /// <summary>
        /// Get/Set the root path.
        /// </summary>
        /// 
        public string RootPath { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public AppRootPathProvider() { }

        /// <summary>
        /// Get the root path.
        /// </summary>
        /// <returns>A string.</returns>
        /// 
        public string GetRootPath() {

            return this.RootPath;

        }

    }

}
