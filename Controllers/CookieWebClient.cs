using System;
using System.Net;
using AlbaClient.Models;

namespace AlbaClient
{
    public class CookieWebClient : WebClient, IWebClient
    {
        Uri _responseUri;

        public Uri ResponseUri
        {
            get { return _responseUri; }
        }

        public CookieContainer CookieContainer { get; private set; }

        public CookieWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null)
                return base.GetWebRequest(address);

            request.CookieContainer = CookieContainer;

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            _responseUri = response.ResponseUri;

            return response;
        }

        public void AddCookie(string name, string value, string path, string domain)
        {
            CookieContainer.Add(new Cookie(name, value, path, domain));
        }

        public new string DownloadString(string url)
        {
            return base.DownloadString(url);
        }
    }
}
