var DonateView = Backbone.View.extend({
	defaultAmount: 50,
	tagName: 'div',
	template: _.template($('#donate_template').html()),

	events: {
		'click .js-donate-button': 'donate',
		'change .js-donate-field': 'valueChange',
		'keydown .js-donate-field': 'keypressInside'
	},
	initialize: function () {

	},
	donate: function () {
		if (this.$el.find(".js-donate-field").valid()) {
			window.location.href = this.model.get('domain') + '/cart/AddFund.aspx?TCMID=' + this.model.get('tcmid') + '&AMOUNT=' + this.$el.find('.js-donate-field').val() + '&frequency=OneTime&advcid=' + this.model.get('campaignId') + '&advsc=' + this.model.get('sourceCode') + '&advname=' + this.model.get('name') + '&advurl=' + this.model.get('campaignUrl');
		}
	},
	render: function () {
		this.$el.html(this.template(this.model.toJSON()));
		var name = Math.floor(Math.random() * (999 - 111) + 111);
		this.$el.find(".js-donate-field").prop("name", name);
		this.$el.find(".error").prop("for", name);
		return this;
	},

	valueChange: function () {
		this.$el.find(".js-donate-button").addClass("disabled");
		if (this.$el.find(".js-donate-field").valid()) {
			this.$el.find(".js-donate-button").removeClass("disabled");
		}

	},
	keypressInside: function (ev) {
		if ((ev.keyCode || ev.which) === 13) {
			this.$el.find(".js-donate-button").click();
			ev.preventDefault();
		}
		var context = this;
		var t = window.setTimeout(function () { context.valueChange(context) }, 100);

	},


});