[CmdletBinding()]
Param(
    [Parameter(Mandatory = $true, ValueFromPipeline = $true)]$input
)

Process {
    $input | Select `
        Address_id, `
        Territory_id, `
        @{ Name = "Language"; Expression = { "Chinese" } }, `
        @{ Name = "Status"; Expression = { "New" } }, `
        @{ Name = "Name"; Expression = { ($input."Name" + " " + $_."First Name" + " " + $_."Last Name").Trim()  } }, `
        Address, `
        @{ Name = "Suite"; Expression = { $_."Apt #" } }, `
        City, `
        @{ Name = "Province"; Expression = { "WA" } }, `
        @{ Name = "Postal_Code"; Expression = { $_."Zip" } }, `
        @{ Name = "Country"; Expression = { "USA" } }, `
        Latitude, `
        Longitude, `
        @{ Name = "Telephone"; Expression = { $_."Phone" } }, `
        Notes, `
        Notes_private
}