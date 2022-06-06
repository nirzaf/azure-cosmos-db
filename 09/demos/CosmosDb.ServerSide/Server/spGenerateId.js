function spGenerateId(docToCreate) {

    var context = getContext();
    var collection = context.getCollection();
    var collectionLink = collection.getSelfLink();
    var response = context.getResponse();

    var baseId =
        docToCreate.lastName.substring(0, 5).toUpperCase() +
        docToCreate.firstName.substring(0, 1).toUpperCase();

    docToCreate.id = baseId;

    createDocument();

    function createDocument() {
        var isAccepted = collection.createDocument(collectionLink, docToCreate,
            function (err, docCreated) {
                if (err) {
                    if (err.message.indexOf('Resource with specified id or name already exists') === -1) {
                        throw new Error('Error creating document with id "' + docToCreate.id + '": ' + JSON.stringify(err));
                    }
                    docToCreate.id = baseId + generateRandom5();
                    createDocument();
                }
                response.setBody(docCreated);
            }
        );
        if (!isAccepted) {
            throw new Error('Timeout creating new document');
        }
    }

    function generateRandom5() {
        var text = "";
        var possible = "0123456789";

        for (var i = 0; i < 5; i++) {
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }

        return text;
    }

}
