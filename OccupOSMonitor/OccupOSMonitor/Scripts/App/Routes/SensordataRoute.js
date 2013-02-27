OccupOS.SensordataRoute = Ember.Route.extend({
    setupController: function (controller, model) {
        controller.set('content', OccupOS.Sensordata.find());
    }
    /*model: function() {
        return OccupOS.Sensordata.find();
    }*/
});