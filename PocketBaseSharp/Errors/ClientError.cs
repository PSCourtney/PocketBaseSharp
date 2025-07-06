using FluentResults;

namespace PocketBaseSharp.Errors
{
    public class ClientError : Error
    {
        public ClientError(HttpMethod method, string url, int statusCode) : base($"{method}request to {url} resulted in {statusCode}")
        {
            Metadata.Add("Method", method);
            Metadata.Add("URL", url);
            Metadata.Add("Statuscode", statusCode);
        }
    }
}