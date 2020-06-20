function my_function(message) {
    console.log("from utilities: " + message);
}

function dotNetStaticInvocation() {
    DotNet.invokeMethodAsync("BlazorMovies.Client", "GetCurrentCount") // ritorna una promise per questo serve il then...
        .then(result => {
            console.log("Counter from javascript: " + result);
        });
}

// dotnetObject rappresenta l'istanza dell'oggetto c# che espone il metodo da invocare
function dotNeInstanceInvocation(dotnetObject) {
    // se il metodo .net ritorna un tipo, non è void, va gestito come una promise, col then...
    dotnetObject.invokeMethodAsync("IncrementCount"); 
        //.then(result => {
        //    console.log("Counter from javascript: " + result);
        //});
}
