function spHelloWorld() {
	var context = getContext();
	var response = context.getResponse();
	response.setBody('Greetings from the Cosmos DB server!');
}
