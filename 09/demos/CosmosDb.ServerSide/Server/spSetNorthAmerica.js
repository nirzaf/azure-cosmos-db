function spSetNorthAmerica(docToCreate, enforceSchema) {
	if (docToCreate.address !== undefined && docToCreate.address.countryRegionName != undefined) {
		docToCreate.address.isNorthAmerica =
			docToCreate.address.countryRegionName === 'United States' ||
			docToCreate.address.countryRegionName === 'Canada' ||
			docToCreate.address.countryRegionName === 'Mexico';
	}
	else if (enforceSchema) {
		throw new Error('Expected document to contain address.countryRegionName property');
	}

	var context = getContext();
	var collection = context.getCollection();
	var response = context.getResponse();

	collection.createDocument(collection.getSelfLink(), docToCreate, {},
		function (err, docCreated) {
			if (err) {
				throw new Error('Error creating document: ' + err.Message);
			}
			response.setBody(docCreated);
		}
	);
}
