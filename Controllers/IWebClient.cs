namespace AlbaClient
{
    public interface IWebClient
    {
        void AddCookie(string name, string value, string path, string domain);

        string DownloadString(string url);
    }
}
