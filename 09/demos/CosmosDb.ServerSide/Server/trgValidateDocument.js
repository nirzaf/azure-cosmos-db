function trgValidateDocument() {

	var context = getContext();
	var request = context.getRequest();
	var doc = request.getBody();

    var weekday = doc.weekdayOff;
	if (!weekday || !weekday.length || weekday.length < 3) {
		throw new Error('Expected document to contain weekdayOff property');
	}

	var testForWeekDay = weekday.substring(0, 3).toLowerCase();
	var testForWeekDays = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"];
	var properWeekDays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

	var isValid = false;
	for (var i = 0; i < 7; i++) {
		if (testForWeekDays[i] === testForWeekDay) {
			doc.weekdayOff = properWeekDays[i];		// Set proper weekday spelling
			doc.weekdayNumberOff = i + 1;			// Set weekday number property (1-7)
			isValid = true;
			break;
		}
	}
	if (!isValid) {
		throw new Error('The weekdayOff property is not valid: ' + weekday);
	}

	request.setBody(doc);
}
