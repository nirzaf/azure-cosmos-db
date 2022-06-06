function trgUpdateMetadata() {

	var context = getContext();
	var collection = context.getCollection();
	var collectionLink = collection.getSelfLink();
	var request = context.getRequest();
	var docToCreate = request.getBody();
	var docSize = JSON.stringify(docToCreate).length;
	var docId = docToCreate.id;
	var metaDocId = "_meta";

	if (docId.isMetaDoc || docSize === undefined) {
		return;
	}

	var sql = 'SELECT * FROM c WHERE c.id = "' + metaDocId + '"';
	collection.queryDocuments(collectionLink, sql,
		function (err, metaDocResult, options) {
			if (err) {
				throw new Error('Error querying for metadata document ID ' + metaDocId + ': ' + err.Message);
			}
			if (!metaDocResult || !metaDocResult.length) {
				createNewMetaDoc();
			}
			else {
				var metaDoc = metaDocResult[0];
				updateExistingMetaDoc(metaDoc);
			}
		}
	);

	function createNewMetaDoc() {
		var metaDoc = {
			id: metaDocId,
			address: {
				postalCode: docToCreate.address.postalCode
			},
			isMetaDoc: true,
			minSize: docSize,
			maxSize: docSize,
			minSizeId: docId,
			maxSizeId: docId,
			totalSize: docSize
		};
		collection.createDocument(collectionLink, metaDoc,
			function (err) {
				if (err) {
					throw new Error('Error creating metadata document ID ' + metaDocId + ': ' + err.Message);
				}
			}
		);
	}

	function updateExistingMetaDoc(metaDoc) {
		if (docSize < metaDoc.minSize) {
			metaDoc.minSize = docSize;
			metaDoc.minSizeId = docId;
		}
		if (docSize > metaDoc.maxSize) {
			metaDoc.maxSize = docSize;
			metaDoc.maxSizeId = docId;
		}
		metaDoc.totalSize += docSize;

		collection.replaceDocument(metaDoc._self, metaDoc,
			function (err) {
				if (err) {
					throw new Error('Error replacing metadata document ID ' + metaDocId + ': ' + err.Message);
				}
			}
		);
	}

}
