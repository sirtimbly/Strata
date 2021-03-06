# Strata

A UI Component System for isomorphic rendering in ASP.Net.

---

## Project Strata's Goals

0. Organize markup, styles, and scripts together in a single directory, per user interface component.
0. Share rendering code between browser and server.
0. Provide tools for progressively loading scripts and styles.
0. Allow components to nest and depend on each other.
0. Bundle and cache intelligently.
0. Convention over configuration. 
0. Be flexible enough for implementing other template syntax and JS frameworks in the future. (Backbone+Mustache is the reference implementation, though.)

## Purpose

To organize related code files together in a 'components' directory for discrete sections of user interface functionality. This will make code organization easier across a large javascript drive application. Additionally I want to include (render) these components in a web page with all necessary dependencies resolved and compiled. And to enable progressive enhancement as well as faster performance we have usable HTML delivered to the client without needing to run client-side templating logic before any usable UI is rendered.

There is no public package repository. This is primarily aimed at users who have an internal collection of UI components. Or have built a UI Component API of thier own but want an easy way to handle dependencies and rendering.

## Isomorphic

In this case it means that the data templating in the UI components happens first on the server and then in JS on the client. So that all usable markup is delivered first, without waiting for an AJAX service call and JS templating engine.


## Templating

Currently Strata uses the Mustache template language because of it's wide adoption and many cross platform implementations.

Also, it's convenient for me because my default use case for Strata is with Backbone and Underscore components using the mustache data templating syntax.

## Definition of A Stratum

It's my name for a user interface component. 

A stratum consists of 

- Markup
- JS Code
- JS Dependencies
- Stylesheets
- View Initialization Logic



### Component.json file example

```
{

	"name": "banner",
	"version": "1.0.0",
	"components": [ ],
	"scripts": [
		"/bower_components/underscore/underscore.js",
		"/bower_components/backbone/backbone.js"
	],
	"styles": [ 
		
	]
}
```



## TODO:

- Unit Tests
- Create a Nuget package.
- Create Nuget package for MVC helper for rendering
- Create Nuget package for WebForms usercontrols.
- finish implementing component dependencies


