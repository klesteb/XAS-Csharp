using System;

namespace XAS.Core.Mime {
    
    /// <summary>
    /// Public interface to MimeTypes.
    /// </summary>
    /// 
    public interface IMimeTypes {

        String Type(String extension);
        String Extension(String mimeType);

    }

}
