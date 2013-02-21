Ember.Handlebars.registerBoundHelper('control-group', function (options) {
    //var escaped = Handlebars.Utils.escapeExpression(value).capitalize();
    var name = options.hash.name.capitalize(),
        prependId = "",
        display = name,
        type = "text";

    if (!(typeof options.hash.prependId === 'undefined')) {
        prependId = options.hash.prependId;
    }
    if (!(typeof options.hash.display === 'undefined')) {
        display = options.hash.display;
    }
    
    if (!(typeof options.hash.type === 'undefined')) {
        type = options.hash.type;
    }
    
    //{{view Ember.TextField type="' + type + '" placeholder="' + display + '" valueBinding="view.' + options.hash.name + '" id="input' + prependId + name + '"}} \
    return new Handlebars.SafeString('<div class="control-group"> \
                <label class="control-label" for="input' + prependId + name + '">' + display + '</label> \
                <div class="controls"> \
                    <input class="ember-view ember-text-field" type="text" id="input' + prependId + name + '" placeholder="' + display + '" /> \
                </div> \
            </div>');
});