using System;
using System.Collections.Generic;
using System.Net;
using Alba.Controllers.Models;

namespace Alba.Controllers
{
    public class CookieWebClient : WebClient, IWebClient
    {
        Uri _responseUri;
        CookieCollection cookies;

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

            var httpResponse = response as HttpWebResponse;
            if (httpResponse != null)
            {
                if(cookies == null)
                {
                    cookies = new CookieCollection();
                }

                foreach(var cookie in httpResponse.Cookies)
                {
                    cookies.Add((Cookie)cookie);
                }
            }

            return response;
        }

        public void AddCookie(string name, string value, string path, string domain)
        {
            CookieContainer.Add(new Cookie(name, value, path, domain));
        }

        public string GetCookieValue(string name)
        {
            if (cookies == null)
            {
                return null;
            }

            return cookies[name].Value;
        }

        public new string DownloadString(string url)
        {
            return base.DownloadString(url);
        }
    }
}
