using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface ITerritoryAssignmentService
    {
        IEnumerable<AlbaAssignmentValues> GetAllAssignmentsFresh(string userName);
    }

    public class TerritoryAssignmentService : ITerritoryAssignmentService
    {
        readonly IAlbaCredentialService albaCredentialService;
        readonly WebUIOptions options;

        public TerritoryAssignmentService(
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.albaCredentialService = albaCredentialService;
            options = optionsAccessor.Value;
        }

        public IEnumerable<AlbaAssignmentValues> GetAllAssignmentsFresh(string userName)
        {
            LoadForCurrentAccountFresh(userName);

            var credentials = albaCredentialService.GetCredentialsFrom(userName);
            var client = AuthorizedConnection();
            
            // TODO: Refactoring to use TerritoryAssignmentService.cs
            client.Authenticate(credentials);

            // TODO: Probably don't need a dependency on client here
            var downloader = new DownloadTerritoryAssignments(client);

            return downloader.GetAssignments();
        }

        void LoadForCurrentAccountFresh(string userName)
        {
            Guid albaAccountId = albaCredentialService
                .GetAlbaAccountIdFor(userName);

            LoadFresh(albaAccountId, userName);
        }

        void LoadFresh(Guid albaAccountId, string userName)
        {
            string path = string.Format(
                options.AlbaAssignmentsHtmlPath,
                albaAccountId);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            var client = AuthorizedConnection();

            var credentials = albaCredentialService.GetCredentialsFrom(userName);

            client.Authenticate(credentials);

            var resultString = client.DownloadString(
                RelativeUrlBuilder.GetTerritoryAssignments());

            string html = TerritoryAssignmentParser.Parse(resultString);

            System.IO.File.WriteAllText(path, html);
        }

        AlbaConnection AuthorizedConnection()
        {
            return AlbaConnection.From(options.AlbaHost);
        }
    }
}
