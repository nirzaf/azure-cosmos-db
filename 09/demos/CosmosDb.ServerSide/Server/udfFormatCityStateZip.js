function udfFormatCityStateZip(document) {
	var city = document.address.location.city;
	var state = document.address.location.stateProvinceName;
	var zip = document.address.postalCode;

	if (!city && !state && !zip) {
		return '';
	}

	if (city && !state && !zip) {
		return city;
	}

	var result = '';

	if (city) {
		result += city + ', ';
	}

	if (state) {
		result += state + ' ';
	}

	if (zip) {
		result += zip;
	}

	return result.trim();
}
