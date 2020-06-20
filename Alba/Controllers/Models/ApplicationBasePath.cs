namespace TerritoryTools.Alba.Controllers.Models
{
    public class ApplicationBasePath
    {
        /// <summary>
        /// Returns the base path for the application on it's server.  All other requests are 
        /// appended to this.
        /// </summary>
        /// <param name="protocolPrefix">Example: "http://"</param>
        /// <param name="site">Example: www.domain.com</param>
        /// <param name="applicationPath">Start with a forward slash.  Example: "/alba" </param>
        public ApplicationBasePath(string protocolPrefix, string site, string applicationPath)
        {
            ProtocolPrefix = protocolPrefix;
            Site = site;
            ApplicationPath = applicationPath;
        }

        public string ProtocolPrefix;
        public string Site;
        public string ApplicationPath;

        public string BaseUrl
        {
            get
            {
                return ProtocolPrefix + Site + ApplicationPath;
            }
        }
    }
}
