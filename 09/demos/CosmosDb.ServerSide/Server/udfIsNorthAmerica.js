function udfIsNorthAmerica(countryRegionName) {

	var isNorthAmerica =
		countryRegionName === 'United States' ||
		countryRegionName === 'Canada' ||
		countryRegionName === 'Mexico';

	return isNorthAmerica;
}
