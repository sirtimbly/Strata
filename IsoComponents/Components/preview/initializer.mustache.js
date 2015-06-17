
var _singletonInitialized = false;


$(function () {
	'use strict';

	var local{{modelName}} = new {{modelName}}( 
		{{{modelData}}}
	);
	
	if (!_singletonInitialized) {
		var local{{componentName}} = new PreviewView({ model: local{{modelName}} });
		local{{componentName}}.template = _.template($('#{{templateId}}').html());
		$('#{{containerId}}').html(local{{componentName}}.render().el);
		_singletonInitialized = true;
	}

});