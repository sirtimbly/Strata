
var Cause = Backbone.Model.extend({
	urlRoot: '/api/cause',
	defaults: {
		id: null,
		name: '',
		fundId: '',
		logo: null,
		completedFundraiserText: null,
		completedFundraiserLink: null,
		descriptions: null,
		video: null,
		headlines: null,
		coverPhotos: null,
		emailThanksComplete: null,
		emailThanksCompleteSubject: null
	}
});