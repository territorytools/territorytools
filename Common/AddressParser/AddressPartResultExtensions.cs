using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerritoryTools.Entities.AddressParsers
{
    public static class AddressPartResultExtensions
    {
        public static bool IsAfter(
            this AddressPartResult firstPart,
            AddressPartResult afterThis)
        {
            return firstPart.IsAtLeastNAfter(1, afterThis);
        }

        public static bool IsBefore(
            this AddressPartResult firstPart,
            AddressPartResult secondPart)
        {
            return firstPart.IsAtLeastNBefore(1, secondPart);
        }

        public static bool IsAtLeastNAfter(
            this AddressPartResult firstPart,
            int thisMany,
            AddressPartResult afterThis)
        {
            if (AreEitherNotSet(firstPart, afterThis))
            {
                return false;
            }

            int first = firstPart.Index;
            int second = afterThis.Index + thisMany - 1;

            return first > second;
        }

        public static bool IsAtLeastNBefore(
            this AddressPartResult firstPart,
            int thisMany,
            AddressPartResult beforeThis)
        {
            if(AreEitherNotSet(firstPart, beforeThis))
            {
                return false;
            }

            int first = firstPart.Index + thisMany - 1;
            int second = beforeThis.Index;

            return first < second;
        }

        public static bool AreEitherNotSet(
            AddressPartResult firstPart, 
            AddressPartResult secondPart)
        {
            return firstPart.IsNotSet() || secondPart.IsNotSet();
        }

        public static bool IsNotSet(this AddressPartResult part)
        {
            return string.IsNullOrWhiteSpace(part.Value);
        }

        public static AddressPartResult GetItemAfter(
            this IList<AddressPartResult> list,
            AddressPartResult part)
        {
            var nextIndex = list.IndexOf(part) + 1;

            if (nextIndex < list.Count)
            {
                return list[nextIndex];
            }
            else
            {
                return null;
            }
        }

        public static AddressPartResult GetItemAfterMeIn(
             this AddressPartResult part,
             IList<AddressPartResult> list)
        {
            if(!part.IsLastItemIn(list))
            {
                var nextIndex = part.Index + 1;

                return list[nextIndex];
            }
            else
            {
                return null;
            }
        }

        public static bool IsLastItemIn(
            this AddressPartResult part, 
            IList<AddressPartResult> list)
        {
            var partIndex = list.IndexOf(part);

            return partIndex == (list.Count - 1);
        }
    }
}
