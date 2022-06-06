function spBulkDelete(whereClause) {

	var context = getContext();
	var collection = context.getCollection();
	var collectionLink = collection.getSelfLink();
	var response = context.getResponse();

	if (!whereClause) {
		throw new Error("The whereClause parameter is undefined or null.");
	}

	var query = "SELECT VALUE c._self FROM c WHERE " + whereClause;

	var responseBody = {
		count: 0,
		continuationFlag: true
	};

	queryForDocumentsToDelete();

	function queryForDocumentsToDelete() {
		var isAccepted = collection.queryDocuments(collectionLink, query,
			function (err, documents, responseOptions) {
				if (err) {
					throw err;
				}
				if (documents.length > 0) {
					deleteDocuments(documents);
				}
				else {
					responseBody.continuationFlag = false;
					response.setBody(responseBody);
				}
			}
		);

		if (!isAccepted) {
			response.setBody(responseBody);
		}
	}

	function deleteDocuments(documents) {
		if (documents.length > 0) {
			var documentLink = documents[0];
			var isAccepted = collection.deleteDocument(documentLink,
				function (err) {
					if (err) {
						throw err;
					}
					responseBody.count++;
					documents.shift();
					deleteDocuments(documents);
				}
			);
			if (!isAccepted) {
				response.setBody(responseBody);
			}
		}
		else {
			queryForDocumentsToDelete();
		}
	}
}
