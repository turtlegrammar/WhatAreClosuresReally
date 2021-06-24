This is a presentation I gave at work. It aims to demystify closures and show how widely applicable they are. It's likely
that you use closures all the time and take them for granted! 

First, I show some examples of closures in C#. They're more common than you might think.

Then, I show the equivalence between objects and closures. Any closure can be implemented as an object 
(the variables that the closure captures are passed into the object's constructor) and any object can be implemented as a closure
(methods are replaced by a single dispatch method and dynamic parameters).

Finally, I implement closures by writing an interpreter for Scheme, a simple language in the LISP family.
