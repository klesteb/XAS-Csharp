using System;

namespace XAS.Core.Exceptions {
    
    /// <summary>
    /// Public interface to general purpose error handling routines.
    /// </summary>
    /// 
    public interface IErrorHandler {

        Int32 Exit(Exception ex);
        void Errors(Exception ex);
        void Exceptions(Exception ex);

    }

}
