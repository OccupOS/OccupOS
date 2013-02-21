/// <reference path="../../vendor/ember.js" />
/// <reference path="../App.js" />

OccupOS.AuthView = Ember.View.extend({
    templateName: 'Auth',
    email: '',
    password: '',
    fullName: '',
    
    submit: function(event) {
        var self = this;
        console.log(event.target);
        console.log($(event.target));
        console.log($(event.target).serialize());
        console.log('Email: ' + this.get('email'));
        console.log('Password: ' + this.get('password'));
        console.log('Full Name: ' + this.get('fullName'));

        event.preventDefault();

        /*contact.saveResource()
          .fail( function(e) {
              App.displayError(e);
          })
          .done(function() {
              App.contactsController.pushObject(contact);
              self.get("parentView").hideNew();
          }); */
    }
})