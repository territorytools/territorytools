namespace AlbaClient
{
    public interface IWebClient
    {
        void AddCookie(string name, string value, string path, string domain);

        string GetCookieValue(string name);

        string DownloadString(string url);
    }
}
