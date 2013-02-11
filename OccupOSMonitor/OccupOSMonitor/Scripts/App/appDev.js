//Creating and instantiating a new app
window.OccupOS = Ember.Application.create({
    VERSION: '0.1.0',
    rootElement: '#occupOSMonitor'
    //If we want to do unit testing
    //rootElement: window.TESTING ? '#qunit-fixture' : '#todoapp'
});