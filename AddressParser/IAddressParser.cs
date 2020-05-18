using System;
namespace TerritoryTools.Entities
{
    public interface IAddressParser
    {
        Address Parse(string address);
    }
}
