var HelloWorld = (function () {
    function HelloWorld(e) {
        this.element = e;
    }
    HelloWorld.prototype.helloWorld = function (notify) {
        this.element.innerHTML = "Hello World";
        notify("completed");
    };
    return HelloWorld;
})();
window.onload = function () {
    var e = document.getElementById('content');
    var hello = new HelloWorld(e);
    hello.helloWorld(function (feedback) {
        console.log(feedback);
    });
};
window.Todos = Ember.Application.create({
    rootElement: window.TESTING ? '#qunit-fixture' : '#todoapp'
});
