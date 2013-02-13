/// <reference path="../App.js" />
/// <reference path="../../vendor/ember.js" />

OccupOS.Router.map(function () {
    this.route("about", { path: "/about" });
    this.route("contact", { path: "/contact" });
});

OccupOS.HomeRoute = Ember.Route.extend();
OccupOS.AboutRoute = Ember.Route.extend();
OccupOS.ContactRoute = Ember.Route.extend();

