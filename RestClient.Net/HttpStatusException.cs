using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public class HttpStatusException : Exception
    {
        public HttpResponseMessage HttpResponseMessage { get; }

        public HttpStatusException(string message, HttpResponseMessage httpResponseMessage) : base(message)
        {
            HttpResponseMessage = httpResponseMessage;
        }

        public HttpStatusException()
        {
        }

        public HttpStatusException(string message) : base(message)
        {
        }

        public HttpStatusException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}