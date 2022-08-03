using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface ITelemetryService
    {
        void Trace(string message, string userName);
        void Exception(Exception exception, string userName);
    }

    public class TelemetryService : ITelemetryService
    {
        private readonly TelemetryClient _client;

        public TelemetryService(TelemetryClient client)
        {
            _client = client;
        }

        public void Trace(string message, string userName)
        {
            _client.TrackTrace(
                    message: message,
                    severityLevel: SeverityLevel.Information,
                    properties: new Dictionary<string, string>()
                    {
                        { "UserName", $"{userName}" }
                    });
        }

        public void Exception(Exception exception, string userName)
        {
            _client.TrackException(
                    exception: exception,
                    properties: new Dictionary<string, string>()
                    {
                        { "UserName", $"{userName}" }
                    });
        }
    }
}
