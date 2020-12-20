using System.Collections.Generic;
using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;
using TerritoryTools.Alba.Controllers.UseCases;

namespace TerritoryTools.Alba.PowerShell
{
    public class AlbaConnectedCmdlet : PSCmdlet
    {
        AlbaConnection _Connection;

        [Parameter]
        public AlbaConnection Connection 
        { 
            get
            {
                if (_Connection == null)
                {
                    _Connection = SessionState
                        .PSVariable
                        .Get(nameof(Names.CurrentAlbaConnection))?
                        .Value as AlbaConnection
                        ?? throw new MissingConnectionException();
                }

                return _Connection;
            }
            set
            {
                _Connection = value;
            } 
        }

        List<AlbaLanguage> _Languages;

        [Parameter]
        public List<AlbaLanguage> Languages
        {
            get
            {
                if (_Languages == null)
                {
                    _Languages = SessionState
                        .PSVariable
                        .Get(nameof(Names.AlbaLanguages))?
                        .Value as List<AlbaLanguage>
                        ?? throw new MissingLanguagesException();
                }

                return _Languages;
            }
            set
            {
                _Languages = value;
            }
        }
    }
}
