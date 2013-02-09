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