-- Link users from GUID
INSERT INTO TerritoryUserAlbaAccountLink
SELECT
    Id AS TerritoryUserId,
    '<GUID>' AS AlbaAccountId,
    'Added' AS Role,
    '<DATE>' AS Created,
    '<DATE>' AS Updated
FROM TerritoryUser

