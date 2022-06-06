SELECT
	Name				AS [name],
	AddressType			AS [address.addressType],
	AddressLine1		AS [address.addressLine1],
	City				AS [address.location.city],
	StateProvinceName	AS [address.location.stateProvinceName],
	PostalCode			AS [address.postalCode],
	CountryRegionName	AS [address.countryRegionName]
 FROM
	Sales.vStoreWithAddresses
 WHERE
	AddressType = 'Main Office'
