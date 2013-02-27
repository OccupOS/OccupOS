OccupOS.MonitorRoute = Ember.Route.extend({
    setupController: function (controller, model) {
        controller.set('content', OccupOS.Sensordata.find());
    }
});