
var AdvocateCampaign = Backbone.Model.extend({
	urlRoot: '/api/AdvocateCampaign',
	defaults: { 
		id: null,
		productType: 'Donation',
		teamName: null,
		teamType: null,
		category: 'race',
		cause: '86840',
		title: '',
		url: '',
		profilePhoto: '',
		goal: '',
		donations: '1250', //TODO: rip out this dummy value
		headline: '',
		description: '',
		eventDate: null,
		startDate: null,
		endDate: null,
		displayEndDate: '',
		dateCreated: null,
		status: null,
		image: null,
		token: '',
		ownerName: '',
		ownerFirstName: function() {
			return this.get('ownerName').split(' ')[0];
		},
		ownerEmail: '',
		daysLeft: '',
		displayDonations: function() {
			return s.numberFormat(parseInt(this.get('total')), 0);
		},
		displayGoal: function () {
			return s.numberFormat(parseInt(this.get('goal')), 0);
		},
		percentAchieved: function () {
			return parseInt((this.get('total').replace(/[^0-9-.]/g, '') / this.get('goal')) * 100);
		},
		causeData: null,
		total: '0.00',
		sourceCode: null,
	},
	//this method allows us to use calculated properties in the model
	get: function (attr) {
		var value = Backbone.Model.prototype.get.call(this, attr);
		return _.isFunction(value) ? value.call(this) : value;
	},
	//this is useful for templating with the calculated fields
	toJSON: function() {
		var data = {};
		var json = Backbone.Model.prototype.toJSON.call(this);
		_.each(json, function(value, key) {
		  data[key] = this.get(key);
		}, this);
		return data;
	  },

	isInStatus: function(s) {
		if (this.get('status') === s)
			return true;

		return false;
	},
	statuses: {
		CREATED: 'Created',
		ACTIVE: 'Active',
		COMPLETED: 'Completed',
		SUSPENDED: 'Suspended'
	},
	validate: function (attrs) {
		var errors = '';
		if (attrs.endDate !== null && attrs.eventDate !== null) 
		{
			if (Date.parse(attrs.endDate) < Date.parse(attrs.eventDate))
			{
				errors += "<li>Event End Date must be after the Event Dates</li>";
			}

			if (Date.parse(attrs.endDate) < Date.parse(attrs.Date)) {
				errors += "<li>Event End Date must be set after Current Date</li>";
			}
		}	

		var text_field_validate = function (rules, field_name, field_value) {
			if ((rules.required && !field_value) || (rules.minlength && field_value.length < rules.minlength)) {
				errors += "<li>Please enter a " + field_name + "</li>";
			}
			if (rules.maxlength && field_value.length > rules.maxlength) {
				errors += "<li>Your " + field_name + " must be no longer than " + rules.maxlength + " characters</li>";
			}
		};

		text_field_validate(this.validationOptions.rules.title, "title", attrs.title);

		// Only validate these fields from the preview page. The campaign creation wizard uses other validation.
		if (attrs.id) {
			text_field_validate(this.validationOptions.rules.headline, "headline", attrs.headline);
			text_field_validate(this.validationOptions.rules.description, "description", attrs.description);
		}

		if (errors !== '') {
			return '<ul>' + errors + '</ul>';
		}
	},
	validationOptions: {
		debug: true,
		rules: {
			'title': {
				required: true,
				minlength: 6,
				maxlength: 100
			},
			'url': {
				required: true,
				minlength: 4,
				maxlength: 100,
				remote: {
					url: '/api/AdvocateCampaign/CheckUrl',
					type: 'get',
					data: {
						CampaignId: 0
					}
				}
			},
			'goal': {
				required: true,
				digits: true,
				min: 50,
				max: 1000000
			},
			'eventdate': {
				required: false,
				date: true,
				isBeforeToday: true
			},
			'enddate': {
				date: true,
				isBeforeToday: true,
				isBeforeEvent: true
			},
			'headline': {
				required: true,
				maxlength: 250
			},
			'description': {
				required: true,
				maxlength: 5000
			}
		},
		messages: {
			'url': {
				required: "This field is required.",
				minlength: "Please use a minimum of 4 characters in your URL.",
				maxlength: "A maximum limit of 100 characters.",
				remote: "That url is currently taken, please choose a new one."
			}
		}
	}

});

$.extend({
	getUrlVars: function () {
		var vars = [], hash;
		var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
		for (var i = 0; i < hashes.length; i++) {
			hash = hashes[i].split('=');
			vars.push(hash[0]);
			vars[hash[0]] = hash[1];
		}
		return vars;
	},
	getUrlVar: function (name) {
		return $.getUrlVars()[name];
	}
});


if ($.validator !== undefined) {

	$.validator.addMethod("isBeforeToday", function (value, element) {
		var inDate = new Date(value);
		var tDate = new Date();
		if (inDate != null) {
			if (inDate < tDate) {
				return false;
			} else {
				return true;
			}
		}
	}, "Please use a date greater than today.");

	$.validator.addMethod("isBeforeEvent", function (value, element) {
		if (value != null && $("#eventdate").val() != null) {
			var endD = new Date(value);
			var eventD = new Date($("#eventdate").val());
			if (endD < eventD) {
				return false;
			} else {
				return true;
			}
		} else {
			return true;
		}
	}, "Event date cannot be before campaign end date.");

}

