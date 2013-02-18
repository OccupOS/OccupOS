/// <reference path="../../_references.js" />

//TO-DO: this.resource() for router
OccupOS.Router.map(function () {
    this.route("about", { path: "/about" });
    this.route("contact", { path: "/contact" });
});

OccupOS.HomeRoute = Ember.Route.extend();
OccupOS.AboutRoute = Ember.Route.extend();
OccupOS.ContactRoute = Ember.Route.extend();

