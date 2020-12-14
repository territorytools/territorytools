# Territory Tools Address Parser
This library has functions that will break a U.S. postal address into
components that can be compared when checking for duplicates and for making
addresses look consistent.

## Sample Address
`1234 NE Main St, Apt #A-5, Bellevue, WA 98004-5678`

## Sample Results
- StreetNamePrefix:
- StreetNumber: `1234`
- StreetNumberFraction:
- StreetName:
  - StreetTypePrefix:
  - DirectionalPrefix: `NE`
  - StreetName: `Main`
  - StreetType: `St`
  - DirectionalSuffix:
- Unit:
  - UnitType: `Apt`
  - UnitNumber: `A-5`
- City: `Bellevue`
- Region:
  - Code: `WA`
  - Name: `Washington`
- PostalCode:
  - Code: `98004`
  - Extra: `5678`

## Normalization & Comparison
### Street Types
Street types like `Street` or `Avenue` are normalized into `ST` or `AVE` respectively.

### Directionals
Directionals words like `Northwest` are normalized to `NW`.

### Unit Numbers and Types
When comparing addresses with a unit numbers, the unit type, for example `Apt`, `Suite` or `#` is ignored and only the unit numbers are compared.