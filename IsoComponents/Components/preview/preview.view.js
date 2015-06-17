//etch.config.buttonClasses = {
//	'default': [],
//	'all': ['bold', 'italic', 'underline', 'unordered-list', 'ordered-list', 'link', 'clear-formatting'],
//	'title': ['bold']
//};

//etch.config.selector = '.js-editable';

var PreviewView = Backbone.View.extend({
	tagName: 'div',
	template: _.template($('#preview_template').html()),
	initialize: function () {
		_.bindAll(this, 'save', 'dataSyncComplete');
		this.model.bind('save', this.save);
		this.model.bind('sync', this.dataSyncComplete);
		this.model.bind('error', this.dataSyncError);
		this.model.on("invalid", function (model, error) {
			toastr["warning"]("Invalid changes", error);
		});
		this.donationModel = new Donation({
			tcmid: '1111',
			max: '75000',
			defaultAmount: 79,
			name: encodeURIComponent(this.model.get('ownerFirstName') + "'s"),
			campaignId: this.model.get('id'),
			campaignUrl: this.model.get('url'),
			domain: 'localhost',
			sourceCode: this.model.get('sourceCode')
		});
	},
	events: {
		//'mousedown .js-editable': 'editableClick',
		'click .header-profile': 'changePhoto',
		'click .js-campaign-launch': 'showLaunchModal',
		'click .js-cover-photo-edit': 'showCoverModal',
		'click .js-campaign-clone': 'cloneCampaign',
		'click .js-share-email': 'sharebyemail',
	},
	//editableClick: etch.editableInit,
	render: function () {
		var context = this;


		this.$el.html(""); //clear the DOM node

		if (!this.model.isInStatus(this.model.statuses.COMPLETED)) {
			this.$el.html($('#header-actions-template').html());
		}

		this.$el.append(this.template(this.model.toJSON())); // main view template rendered into DOM
		this.$el.removeClass('editing');
		this.$el.find('.js-campaign-save').removeClass('disabled').on('click', function () {
			context.save();
		});


		// ------ Conditional changes based on state ------
		if (this.model.get("profilePhoto") != "") {
			this.$el.find(".header-profile-photo-caption").html("Update Photo");
		}
		if (this.model.isInStatus(this.model.statuses.ACTIVE)) {
			this.$el.find(".js-campaign-launch").hide();
			this.$el.find(".js-campaign-edit").hide();
			this.$el.find("#end-campaign-module").show();
			this.$el.find(".js-donate-now").removeClass("disabled").addClass("standard");
		}
		// ------------------------- 


		//// ------ Goal Editor ------
		//var goalView = new GoalView({
		//	model: this.model
		//});
		//this.$el.find('#goal-module').html(goalView.render().el);
		//goalView.on('goal:changed', function (pGoal) {
		//	context.model.set({
		//		goal: pGoal
		//	});
		//	context.save();
		//});
		//// ------------------------- 


		//// ------ End Campaign UI ------
		//var endCampaignView = new EndCampaignView({ model: this.model });
		//this.$el.find('#end-campaign-module').html(endCampaignView.render().el);
		//// -------------------------


		// ------ Donation Buttons ------
		var donateSideView = new DonateView({ model: this.donationModel });
		var donateHeaderView = new DonateView({ model: this.donationModel });
		this.$el.find('#donate-view-module').html(donateSideView.render().el);
		this.$el.find('#donate-header-module').html(donateHeaderView.render().el);
		this.$el.find('.js-donors-donate').click(function () {
			context.$el.find('#donate-view-module input').focus();
		});
		// -------------------------


		// --- Disable donate/social if preview and not active ----
		if (this.model.isInStatus(this.model.statuses.CREATED)) {
			this.$el.find('.js-donate-field').attr('disabled', 'disabled').addClass('disabled');
			this.$el.find('.js-donate-button').attr('disabled', 'disabled').addClass('disabled').css('cursor', 'default');
			this.$el.find('.js-donate-button').removeClass('js-donate-button');
			this.$el.find('.js-button-social').addClass('disabled').css('cursor', 'default').on("click", false);
			this.$el.find('.js-campaign-hide-tooltip').addClass('has-tip tip-top').attr('title', 'This will be available once the campaign is active.');
			this.$el.foundation('tooltip', 'reflow');
		}
		// -------------------------


		// ----- Completed Campaigns View ------
		if (this.model.isInStatus(this.model.statuses.COMPLETED)) {
			this.$el.find(".campaign-ended").show();
			this.$el.find(".campaign-header-actions").hide();
			this.$el.find(".cover-edit").hide();
			this.$el.find(".header-profile-photo-caption").hide();
			this.$el.find(".edit-date").hide();
			this.$el.find(".edit-goal").hide();
			this.$el.find(".header-profile-photo-active").removeClass("header-profile-photo-active");
			this.$el.find(".edit-in-place").removeClass("js-editable edit-in-place");
			this.$el.find("#donate-header-module").hide();
			this.$el.find("#donate-view-module").hide();
		}
		// -------------------------


		// ----- Campaign Comments View ------
		//var commentCollection = new CommentCollection();
		//commentCollection.campaignId = this.model.get('id');
		//commentCollection.comparator = function (model) {
		//	return -model.get('id'); //this should make comments sort in reverse chronological order
		//};
		//commentCollection.fetch({
		//	success: function (model, response, options) {
		//		var commentView = new CampaignComments({ model: commentCollection });
		//		commentView.showOwnerControls = true;
		//		context.$el.find(".campaign-comments").html(commentView.render().el);
		//	},
		//	error: function (model, response, options) {
		//		toastr["error"]("Problem loading that campaign: " + response.responseJSON.message);
		//	}
		//});
		// -------------------------


		return this;
	},
	save: function () {
		console.log("saving...");
		var title = this.$el.find('#editable-title').text();
		var headline = this.$el.find('#editable-headline').text();
		var description = this.$el.find('#editable-description').html(); //TODO: add XSS sanitization to the API controller
		this.model.set({
			title: title,
			headline: headline,
			description: description
		});

		this.model.save(this.model.attributes, {
			success: function () {
				console.log("done");
				toastr["success"]("Saved", "Campaign Changes Saved to Server");
			}
		});
	},
	dataSyncComplete: function () {
		//this.$el.find('.js-campaign-save').addClass('disabled').off('click');
		//this.model.on("change", this.dataSyncNeeded, this);
		console.log("data sync complete");
		this.render();
	},
	dataSyncError: function (model, response) {
		console.log('sync error: ' + response);
		toastr["error"]("Error while saving", response);
	},
	changePhoto: function () {
		if (this.model.isInStatus(this.model.statuses.COMPLETED)) {
			return;
		}

		var photoModal = new PhotoModal();
		$('#modal-container').html(photoModal.render().el);
		var myModel = this.model;
		var context = this;

		$(".photo-section").hide();
		$(".js-file-upload-button").addClass("disabled");
		$(".js-file-upload-button").attr("disabled", "disabled");

		//User selects a previously used photo
		$(".previous-profile-photo").on("click", function () {
			myModel.save({ profilePhoto: $(this).attr("src") });
			$(".header-profile-photo").attr("src", $(this).attr("src"));

			if (this.model.get("profilePhoto") != "") {
				this.$el.find(".header-profile-photo-caption").html("Update Photo");
			}

			toastr["success"]("Saved", "Campaign Changes Saved to Server");
			$(photoModal.cancelEl).click();
		});

		//User selects a photo to crop and upload
		$(".js-choose-photo").on("click", function () {
			$(".photo-section").show();
			$(".previous-photo-list").hide();
		});

		$('.js-file-upload-button').click(function () {
			$(photoModal.cancelEl).click();
		});

		// Desktop?
		if ($('html').hasClass('full'))
			_fullVersion = true;

		_$imageFrame = $('#js-photo-frame');
		_imageCanvas = document.getElementById('js-photo-full');
		_$imageContainer = $('#js-photo-container');
		_$imageMain = $('.js-photo-main');
		_$imageLoading = $('#js-photo-loading');
		_$imageThirds = $('.js-photo-thirds');

		_dataModel = myModel;

		if (!Modernizr.fileinput)
			UseFileUpload();
		else {
			if (!Modernizr.filereader)
				UseFileUpload();
			else {
				// Desktop browsers that support canvas & files
				UseCanvas();
			}
		}
	},
	showLaunchModal: function () {
		var launchModal = new LaunchModal({
			model: this.model
		});
		launchModal.donationModel = this.donationModel;

		var viewContext = this;

		$.ajax({
			type: 'POST',
			url: '/api/advocatecampaign/launch/' + this.model.get('id'),
			complete: function (source) {
				$('#modal-container').html(launchModal.render().el);
				launchModal.model.fetch();
				_sourceCode = source.responseJSON;
				viewContext.render();
			}
		});

		$(".js-editable").on("input", function () {
			$(".twitter-share-button-content").attr("data-url", $(".js-social-content").text())
				.attr("data-text", $(".js-social-content").text());
		});

		$(".bbm-modal").css("background", "#ddd");
	},
	showCoverModal: function () {
		var context = this;
		var coverModal = new CoverModal();
		coverModal.coverPhotos = _.pairs(this.model.get('causeData').coverPhotos);
		coverModal.currentImage = this.model.get('image');
		coverModal.currentCategory = this.model.get('category');
		coverModal.currentCause = this.model.get('cause');

		$('#modal-container').html(coverModal.render().el);
		coverModal.on('image:selected', function (source) {
			context.updateCoverImage(source);
			coverModal.destroy();
		});
	},
	updateCoverImage: function (source) {
		this.model.set({ image: source });
		this.save();
		this.render();
	},
	cloneCampaign: function () {
		$.ajax({
			type: 'POST',
			url: '/api/advocatecampaign/clone/' + this.model.get('id'),
			success: function (data) {
				if (data && data > 0) {
					window.location.href = "?advcid=" + data;
				}
				else {
					console.log('sync error: ' + response);
					toastr["error"]("Error while saving", response);
				}
			}
		});
	},
	sharebyemail: function () {
		var id = this.model.get('id');
		window.location.href = ShareEmailUrl + id;
	}
});