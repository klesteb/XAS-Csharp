using System.Xml.Serialization;

namespace XAS.Rest.Server.Errors {

    // taken from: https://github.com/bytefish/NancySamples/tree/master/ErrorHandling
    // with modifications

    /// <summary>
    /// The defined service error codes.
    /// </summary>
    /// 
    public enum ServiceErrorCode {

        [XmlEnum("10")]
        GeneralError = 10,

        [XmlEnum("20")]
        InvalidToken = 20,

        [XmlEnum("400")]
        BadRequest = 400,

        [XmlEnum("401")]
        Unauthorized = 401,

        [XmlEnum("403")]
        Forbidden = 403,

        [XmlEnum("404")]
        NotFound = 404,

        [XmlEnum("406")]
        NotAcceptable = 406,

        [XmlEnum("411")]
        LengthRequired = 411,

        [XmlEnum("422")]
        UnprocessableEntity = 422,

        [XmlEnum("500")]
        InternalServerError = 500,

        [XmlEnum("503")]
        ServiceUnavailable = 503

    }

}
