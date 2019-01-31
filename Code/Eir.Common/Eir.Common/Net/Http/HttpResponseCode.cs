namespace Eir.Common.Net.Http
{
    public enum HttpResponseCode
    {
        Undefined = 0,
        // ReSharper disable once InconsistentNaming
        OK=200,
        Created=201,
        BadRequest = 400,
        Unauthorized=401,
        Forbidden = 403,
        NotFound = 404,
        NotAcceptable = 406,
        ResourceLocked = 423,
        InternalServerError = 500,
        ServiceUnavailable = 503
    }
}