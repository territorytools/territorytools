namespace Alba.Controllers.AlbaServer
{
    public class TerritoryAssignmentParser
    {
        public static string Parse(string value)
        {
            return AlbaJsonResultParser.ParseDataHtml(value, "territories");
        }
    }
}
