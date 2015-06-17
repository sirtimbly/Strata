var AdvocateCampaignCollection = Backbone.Collection.extend({
	model: AdvocateCampaign,
	url: '/api/advocatecampaign'
});