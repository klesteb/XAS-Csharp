
namespace XAS.Rest.Server.Repository {

    /// <summary>
    /// Used for creating HAL links.
    /// </summary>
    /// <remarks>
    /// This class should be inherited to provide a data model for creating 
    /// HAL links. Expecially the root links which dosen't actually have a 
    /// data model associated with them. 
    /// </remarks>
    /// 
    public class HalLinks {

        public string rel;
        public string href;
        public string titles;

    }

}
