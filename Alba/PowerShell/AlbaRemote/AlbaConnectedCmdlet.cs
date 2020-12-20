using System.Management.Automation;
using TerritoryTools.Alba.Controllers.AlbaServer;

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
    }
}
