namespace Eir.Common.Net.Http
{
    public enum HttpMethod
    {
        Undefined,
        Get,
        Post,
        Put,
        Delete,
        /// <summary>
        /// CORS calls for xss checks support.
        /// </summary>
        Options, 
    }
}