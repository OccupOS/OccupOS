/*OccupOS.SensordataController = Ember.Controller.extend({
    test: 'Hello World'
});*/

OccupOS.SensordataController = Em.ArrayController.extend({
    sortProperties: ['measuredAt']
});

