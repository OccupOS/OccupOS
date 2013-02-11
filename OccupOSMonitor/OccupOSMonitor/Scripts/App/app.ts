class HelloWorld {
    
    element: HTMLElement;

    constructor(e: HTMLElement) {
        this.element = e;
    }

    helloWorld(notify: (feedback: string)=>any) {
        this.element.innerHTML = "Hello World";
        notify("completed")
    }
}

window.onload = () => {
    var e = document.getElementById('content')

    var hello = new HelloWorld(e);
    hello.helloWorld(function (feedback) {
        console.log(feedback);
    });
};

/*
  Creates a new instance of an Ember application and
  specifies what HTML element inside index.html Ember
  should manage for you.
*/
window.Todos = Ember.Application.create({
    rootElement: window.TESTING ? '#qunit-fixture' : '#todoapp'
});