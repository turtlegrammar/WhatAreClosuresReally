
using System;
using System.Collections.Generic;
using System.Linq;
using static Closures.ExpHelpers;

namespace Closures
{
    public abstract class Exp {}

    public class Number: Exp { public long Value; }

    public class Symbol: Exp { public string Value; }

    public class Bool: Exp { public bool Value; }

    public class CreateFunction: Exp { public List<string> Parameters; public Exp Body; }

    public class Assign: Exp { public string Variable; public Exp Value; }

    public class FunctionCall: Exp { public Exp Function; public List<Exp> Arguments; }

    public class If: Exp { public Exp Condition; public Exp TrueBranch; public Exp FalseBranch; }

    public class Sequence: Exp { public List<Exp> Expressions; }

    public class While: Exp { public Exp Condition; public Exp LoopBody; }


    // Internal

    public class FunctionObject: Exp { public List<string> Parameters; public Exp Body; public Environment Environment; }

    public class Null: Exp {}

    public class PrimitiveFunction: Exp { public Func<List<Exp>, Exp> Work; }


    // Helpers

    public static class ExpHelpers
    {
        public static Exp Number(long n) => new Number { Value = n };

        public static Exp Symbol(string s) => new Symbol{ Value = s };

        public static Exp Bool(bool b) => new Bool { Value = b };

        public static Exp Function(List<string> parameters, Exp body) => new CreateFunction { Parameters = parameters, Body = body };

        public static Exp Assign(string variable, Exp value) => new Assign { Variable = variable, Value = value };

        public static Exp FunctionCall(Exp function, params Exp[] arguments) => new FunctionCall {Function = function, Arguments = arguments.ToList()};

        public static Exp If(Exp condition, Exp trueBranch, Exp falseBranch) => new If { Condition = condition, TrueBranch = trueBranch, FalseBranch = falseBranch };

        public static Exp Sequence(params Exp[] exps) => new Sequence { Expressions = exps.ToList() };

        public static Exp While(Exp condition, Exp body) => new While { Condition = condition, LoopBody = body };

        public static Exp Void = new Null();

        /// Internal

        public static List<T> List<T>(params T[] items) => items.ToList();

        public static Exp PrimitiveFunction(Func<List<Exp>, Exp> work) => new PrimitiveFunction { Work = work };
    }

    public static class ASTExamples
    {
        // let loopSumTo = function(x) {
        //    let sum = 0;
        //    while (x > 0) {
        //       sum = sum + x;
        //       x = x - 1;
        //    }
        //    return sum;
        // }
        // loopSumTo(5); // 0 + 1 + 2 + 3 + 4 + 5 = 15
        public static string LoopSumToCode = @"
          (assign loopSumTo 
                  (function (x) 
                     (assign sum 0)
                     (while (> x 0)
                        (assign sum (+ sum x))
                        (assign x (- x 1)))
                      sum))
          (loopSumTo 5)
        ";
        public static Exp LoopSumTo = Sequence(
            Assign(
                "loopSumTo", 
                Function(
                    List("x"),
                    Sequence(
                        Assign("sum", Number(0)),
                        While(
                            FunctionCall(Symbol(">"), Symbol("x"), Number(0)),
                            Sequence(
                                Assign("sum", FunctionCall(Symbol("+"), Symbol("x"), Symbol("sum"))),
                                Assign("x", FunctionCall(Symbol("-"), Symbol("x"), Number(1)))
                            )
                        ),
                        Symbol("sum")
                    )
                )
            ),
            FunctionCall(Symbol("loopSumTo"), Number(5))
        );

        // let recSumTo = x => 1 > x ? 0 : x + recSumTo(x - 1);
        // recSumTo(5);
        public static string RecursiveSumToCode = @"
          (assign recSumTo
                  (function (x) 
                     (if (> 1 x) 
                         0 
                         (+ x (recSumTo (- x 1))))))
          (recSumTo 5)
        ";
        public static Exp RecursiveSumTo = Sequence(
            Assign(
                "recSumTo", 
                Function(
                    List("x"),
                    If(
                        FunctionCall(Symbol(">"), Number(1), Symbol("x")),
                        Number(0),
                        FunctionCall(
                            Symbol("+"),
                            Symbol("x"),
                            FunctionCall(
                                Symbol("recSumTo"),
                                FunctionCall(Symbol("-"), Symbol("x"), Number(1))
                            )
                        )
                    )
                )
            ),
            FunctionCall(Symbol("recSumTo"), Number(5))
        );

        // let makeCounter = function() { let x = 0; return function() { x = x + 1; return x; }; }
        // let counter = makeCounter();
        // counter();
        // counter();
        public static string CounterCode = @"
          (assign makeCounter
                  (function ()
                     (assign x 0)
                     (function ()
                        (assign x (+ x 1))
                        x)))
           (assign counter (makeCounter))
           (counter)
           (counter)
        ";
        public static Exp Counter = Sequence(
            Assign(
                "makeCounter",
                Function(
                    List<string>(),
                    Sequence(
                        Assign("x", Number(0)),
                        Function(
                            List<string>(),
                            Sequence(
                                Assign("x", FunctionCall(Symbol("+"), Symbol("x"), Number(1))),
                                Symbol("x")
                            )
                        )
                    )
                )
            ),
            Assign("counter", FunctionCall(Symbol("makeCounter"))),
            FunctionCall(Symbol("counter")),
            FunctionCall(Symbol("counter"))
        );
    }
}