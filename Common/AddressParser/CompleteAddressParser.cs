using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace TerritoryTools.Entities.AddressParsers
{
    public class CompleteAddressParser : IAddressParser
    {
        AddressParseContainer container { get; set; }
        string addressToParse;
        List<StreetType> streetTypes;
        List<StreetType> streetNameAfterTypes;
        Dictionary<string, StreetType> streetTypeMap = new Dictionary<string, StreetType>();
        Dictionary<string, StreetType> streetNameAfterTypeMap = new Dictionary<string, StreetType>();


        public CompleteAddressParser(
            IEnumerable<StreetType> streetTypes,
            IEnumerable<StreetType> streetNameAfterTypes = null)
        {
            this.streetTypes = streetTypes.ToList();
            foreach (var t in this.streetTypes)
            {
                streetTypeMap[t.Full.ToUpper()] = t;
                if (t.Abbreviation != null)
                {
                    streetTypeMap[t.Abbreviation?.ToUpper()] = t;
                }
            }

            this.streetNameAfterTypes = (streetNameAfterTypes ?? new List<StreetType>()).ToList();
            foreach (var t in this.streetNameAfterTypes)
            {
                streetNameAfterTypeMap[t.Full.ToUpper()] = t;
                if (t.Abbreviation != null)
                {
                    streetNameAfterTypeMap[t.Abbreviation?.ToUpper()] = t;
                }
            }
        }

        public Address Normalize(
            string text,
            string city = null,
            string state = null,
            string postalCode = null)
        {
            try
            {
                return NormalizeTry(text, city, state, postalCode);
            }
            catch
            {
                throw;
            }
        }

        private Address NormalizeTry(string text, string city, string state, string postalCode)
        {
            var parsed = Parse(text, city, state, postalCode);

            string poBoxExpression = @"P\.?\s*O\.?\s*B(ox)?\s+(\d+)";
            var m = Regex.Match(text, poBoxExpression);
            if (m.Success)
            {
                parsed.Number = m.Groups[2].Value;
                parsed.IsNotPhysical = true;
                parsed.StreetName = "PO Box";
            }

            string noStreetType = @"^(\d+)\s+(\w+)$";
            var broadway = Regex.Match(text, noStreetType);
            if (broadway.Success)
            {
                parsed.IsNotPhysical = true; // it is physical, just weird, like '1234 Broadway'
                parsed.Number = broadway.Groups[1].Value;
                parsed.StreetName = broadway.Groups[2].Value;
            }

            parsed.DirectionalPrefix = DirectionalFinder
                .ToDirectionalAbbreviation(parsed.DirectionalPrefix);
            parsed.StreetName = TitleCase(parsed.StreetName);
            parsed.DirectionalSuffix = DirectionalFinder
                .ToDirectionalAbbreviation(parsed.DirectionalSuffix);
            parsed.StreetName = StringNormalizer
                .SetOrdinalSuffixesToLowerCase(
                    TitleCase(parsed.StreetName));

            if (!string.IsNullOrEmpty(parsed.StreetType))
            {
                string normalizedStreetType = parsed.StreetType.ToUpper();
                if (!streetTypeMap.ContainsKey(normalizedStreetType))
                {
                    throw new InvalidStreetTypeException(normalizedStreetType, text);
                }

                string abbreviation = streetTypeMap[normalizedStreetType].Abbreviation;
                if (!string.IsNullOrWhiteSpace(abbreviation))
                {
                    parsed.StreetType = TitleCase(abbreviation);
                }
                else
                {
                    parsed.StreetType = TitleCase(parsed.StreetType);
                }
            }

            parsed.UnitType = TitleCase(parsed.UnitType);
            parsed.UnitNumber = parsed.UnitNumber.Replace("-", string.Empty).ToUpper();
            parsed.City = TitleCase(parsed.City);
            parsed.State = parsed.State.ToUpper();
            parsed.PostalCode = parsed.PostalCode.ToUpper();
            parsed.PostalCodeExt = parsed.PostalCodeExt.ToUpper();

            return parsed;
        }

        string TitleCase(string text)
        {
            return CultureInfo
              .CurrentCulture
              .TextInfo
              .ToTitleCase((text ?? string.Empty).ToLower())
              .Replace("Po Box", "PO Box");
        }

        public Address Parse(string addressToParse)
        {
            return Parse(addressToParse, null, null, null);
        }

        public Address Parse(
            string addressToParse,
            string city = null,
            string state = null,
            string postalCode = null)
        {
            if (string.IsNullOrWhiteSpace(addressToParse))
                throw new AddressParsingException($"Blank Address: '{nameof(addressToParse)}'");

            this.addressToParse = addressToParse;

            container = new AddressParseContainer(this.addressToParse);
            var splitter = new AddressSplitter(container);

            splitter.SplitAndClean();

            // Detect 'Non Physical Addresses' like PO Boxes
            // TODO: Maybe call them StreetNameIsFirst addresses
            //string poBoxTest = Regex.Replace(container.CompleteAddressToParse, @"\.", " ");
            //poBoxTest = Regex.Replace(poBoxTest, @"\s+", " ");
            //if (poBoxTest.StartsWith("POB ")
            //    || poBoxTest.StartsWith("PO Box ")
            //    || poBoxTest.StartsWith("P O Box "))
            //{
            //    container.Address.IsNotPhysical = true;
            //    container.ParsedAddress.StreetName.Value = "PO Box";
            //    container.ParsedAddress.StreetName.Index = 0;
            //}


            string poBoxExpression = @"P\.?\s*O\.?\s*B(ox)?\s+(\d+)";
            var m = Regex.Match(container.CompleteAddressToParse, poBoxExpression);
            if (m.Success)
            {
                container.Address.IsNotPhysical = true;
                container.ParsedAddress.Number.Value = m.Groups[2].Value;
                container.ParsedAddress.StreetName.Value = "PO Box";
            }

            string noStreetType = @"^(\d+)\s+([a-zA-Z-]+)$";
            var broadway = Regex.Match(container.CompleteAddressToParse, noStreetType);
            if (broadway.Success)
            {
                container.Address.IsNotPhysical = true; // it is physical, just weird
                container.ParsedAddress.Number.Value = broadway.Groups[1].Value;
                container.ParsedAddress.StreetName.Value = broadway.Groups[2].Value;
            }

            // Find Required Things
            if (!container.Address.IsNotPhysical)
            {
                //new NonPhysicalStreetTypeFinder(container).Find();
                new PhysicalPrefixStreetTypeFinder(container).Find();
                new NormalStreetTypeFinder(container, StreetTypes()).Find();
                new StreetTypeStreetNameFinder(container, StreetTypes()).Find();
                VerifyThatStreetTypeIsSet();
            }

            new AddressNumberFinder(container).Find();

            VerifyThatStreetNumberIsSet();

            if (container.ParsedAddress.Number.Index == 0)
            {
                if(string.Equals(container.AddressPartResults[1].Value, "HWY", StringComparison.OrdinalIgnoreCase))
                {
                    container.Address.StreetNameIsAfterType = true;
                    container.Address.StreetType = "HWY";
                    container.Address.StreetName = container.AddressPartResults[2].Value;
                }
            }



            new UnitTypeFinder(container).Find();

            if (string.IsNullOrWhiteSpace(postalCode))
            {
                new PostalCodeFinder(container).Find();
            }
            else
            {
                if (Regex.IsMatch(postalCode, @"^(\d{5,})\-(\d{4,})$"))
                {
                    container.ParsedAddress.PostalCode = new AddressPartResult()
                    {
                        Index = int.MaxValue-1,
                        Value = postalCode.Split('-')[0]
                    };

                    container.ParsedAddress.PostalCodeExt = new AddressPartResult()
                    {
                        Index = int.MaxValue,
                        Value = postalCode.Split('-')[1]
                    };
                }
                else
                {
                    container.ParsedAddress.PostalCode = new AddressPartResult()
                    {
                        Index = int.MaxValue-1,
                        Value = postalCode
                    };

                }
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                new StateFinder(container).Find();
            }
            else
            {
                container.ParsedAddress.State = new AddressPartResult() 
                {  
                    Index = int.MaxValue - 2,
                    Value = state 
                };
            }

            new DirectionalPrefixFinder(container).Find();
            new DirectionalSuffixFinder(container).Find();

            // Find things in between other things
            new AddressNumberFractionFinder(container).Find();
            new StreetNameFinder(container).Find();
            new UnitNumberFinder(container).Find();

            if (string.IsNullOrWhiteSpace(city))
            {
                new CityFinder(container).Find();
            }
            else
            {
                container.ParsedAddress.City = new AddressPartResult() 
                {
                    Index = int.MaxValue - 3,
                    Value = city 
                };
            }
          
            //new AddressNumberFractionFinder(container).Find();
            new PostalCodeExtFinder(container).Find();

            CopyParsedAddressResultsToAddress();

            return container.Address;
        }

        private List<StreetType> StreetTypes()
        {
            return StreetType.From(streetTypes.Select(t => t.Full).ToArray());
        }

        private void VerifyThatStreetTypeIsSet()
        {
            if (container.ParsedAddress.StreetType.IsNotSet())
                throw new MissingStreetTypeException(addressToParse);
        }

        private void VerifyThatStreetNumberIsSet()
        {
            if (container.ParsedAddress.Number.IsNotSet())
                throw new MissingStreetNumberException(addressToParse);
        }

        private void CopyParsedAddressResultsToAddress()
        {
            var address = container.Address;
            var parsed = container.ParsedAddress;

            address.Number            = parsed.Number.Value ?? string.Empty ?? string.Empty;
            address.NumberFraction    = parsed.NumberFraction.Value ?? string.Empty;
            address.DirectionalPrefix = parsed.DirectionalPrefix.Value ?? string.Empty;
            address.StreetName        = parsed.StreetName.Value ?? string.Empty;
            address.StreetType        = parsed.StreetType.Value ?? string.Empty;
            address.DirectionalSuffix = parsed.DirectionalSuffix.Value ?? string.Empty;
            address.UnitType          = parsed.UnitType.Value ?? string.Empty;
            address.UnitNumber        = parsed.UnitNumber.Value ?? string.Empty;
            address.City              = parsed.City.Value ?? string.Empty;
            address.State             = parsed.State.Value ?? string.Empty;
            address.PostalCode        = parsed.PostalCode.Value ?? string.Empty;
            address.PostalCodeExt     = parsed.PostalCodeExt.Value ?? string.Empty;
        }

        public static List<string> SplitAtPoundSign(string value)
        {
            var pattern = @"^(#)(\S)";
            var matches = new List<string>();
            var match = Regex.Match(value, pattern);
            var prefix = match.Groups[1];
            var suffix = match.Groups[2];

            if (prefix.Success)
                matches.Add(prefix.Value);
            else
                throw new Exception("Cannot get pound sign component");
            
            if (suffix.Success)
                matches.Add(suffix.Value);
            else
                throw new Exception("Cannot get the address component after pound sign.");

            return matches;
        }
    }
}
