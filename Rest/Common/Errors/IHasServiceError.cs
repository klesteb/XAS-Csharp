using Nancy;


namespace XAS.Rest.Common.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications

    /// <summary>
    /// An interface for handling service errors.
    /// </summary>
    /// 
    public interface IHasServiceError {

        ServiceErrorModel HttpServiceError { get; }

    }

}
