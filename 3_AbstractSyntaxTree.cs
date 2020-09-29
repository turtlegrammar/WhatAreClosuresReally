
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

    public class StringExp: Exp { public string Value; }

    public class CreateFunction: Exp { public List<string> Parameters; public Exp Body; }

    public class SetVar: Exp { public string Variable; public Exp Value; }

    public class Define: Exp { public string Variable; public Exp Value; }

    public class FunctionCall: Exp { public Exp Function; public List<Exp> Arguments; }

    public class If: Exp { public Exp Condition; public Exp TrueBranch; public Exp FalseBranch; }

    public class Statements: Exp { public List<Exp> Expressions; }

    public class While: Exp { public Exp Condition; public Exp LoopBody; }

    public class ListExp: Exp { public List<Exp> Members; }


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

        public static Exp String(string s) => new StringExp { Value = s};

        public static Exp Function(List<string> parameters, Exp body) => new CreateFunction { Parameters = parameters, Body = body };

        public static Exp Define(string variable, Exp value) => new Define { Variable = variable, Value = value };

        public static Exp SetVar(string variable, Exp value) => new SetVar { Variable = variable, Value = value };

        public static Exp FunctionCall(Exp function, params Exp[] arguments) => new FunctionCall {Function = function, Arguments = arguments.ToList()};

        public static Exp If(Exp condition, Exp trueBranch, Exp falseBranch) => new If { Condition = condition, TrueBranch = trueBranch, FalseBranch = falseBranch };

        public static Exp Statements(params Exp[] exps) => new Statements { Expressions = exps.ToList() };

        public static Exp While(Exp condition, Exp body) => new While { Condition = condition, LoopBody = body };

        public static Exp ListExp(params Exp[] members) => new ListExp { Members = members.ToList() };

        public static Exp Void = new Null();

        /// Internal

        public static List<T> List<T>(params T[] items) => items.ToList();

        public static Exp PrimitiveFunction(Func<List<Exp>, Exp> work) => new PrimitiveFunction { Work = work };
    }

    public static class SmallASTExamples
    {
        public static string LetXEqual5JS = @"
            let x = 5;
        ";
        public static Exp LetXEqual5 = Define(
            "x",
            Number(5)
        );

        public static string LetXEqual5Plus3JS = @"
            let x = 5 + 3;
        ";
        public static Exp LetXEqual5Plus3 = Define(
            "x",
            FunctionCall(Symbol("+"), Number(5), Number(3))
        );

        public static string MaxJS = @"
            let max = function(x, y) {
                return x > y ? x : y;
            };

            max(5, 6);
        ";

        public static Exp Max = Statements(
            Define(
                "max",
                Function(
                    List("x", "y"),
                    If(
                        FunctionCall(Symbol(">"), Symbol("x"), Symbol("y")),
                        Symbol("x"),
                        Symbol("y")
                    ))
            ),
            FunctionCall(Symbol("max"), Number(5), Number(6))
        );

        public static string ContrivedFunctionCallJS = @"
            function(x, y) { x + y + y }(5, 4); // equals 5 + 4 + 4 = 13
        ";

        public static Exp ContrivedFunctionCall = FunctionCall(
            Function(
                List("x", "y"),
                FunctionCall(
                    Symbol("+"),
                    Symbol("x"),
                    FunctionCall(
                        Symbol("+"),
                        Symbol("y"), 
                        Symbol("y")
                    )
                )
            ),
            Number(5), 
            Number(4)
        );
    }

    // https://repl.it/languages/scheme
    // https://repl.it/languages/javascript
    public static class ASTExamples
    {
        public static string LoopSumToCodeJS = @"
        let loopSumTo = function(x) {
           let sum = 0;
           while (x > 0) {
              sum = sum + x;
              x = x - 1;
           }
           return sum;
        }
        loopSumTo(5); 
        ";

        public static Exp LoopSumTo = Statements(
            Define(
                "loopSumTo", 
                Function(
                    List("x"),
                    Statements(
                        Define("sum", Number(0)),
                        While(
                            FunctionCall(Symbol(">"), Symbol("x"), Number(0)),
                            Statements(
                                SetVar("sum", FunctionCall(Symbol("+"), Symbol("x"), Symbol("sum"))),
                                SetVar("x", FunctionCall(Symbol("-"), Symbol("x"), Number(1)))
                            )
                        ),
                        Symbol("sum")
                    )
                )
            ),
            FunctionCall(Symbol("loopSumTo"), Number(5))
        );

        public static string LoopSumToCodeScheme = @"
          (define loopSumTo 
                  (lambda (x) 
                     (define sum 0)
                     (while (> x 0)
                        (set! sum (+ sum x))
                        (set! x (- x 1)))
                      sum))
          (loopSumTo 5)
        ";

        public static string RecursiveSumToCodeJS = @"
         let recSumTo = x => 1 > x ? 0 : x + recSumTo(x - 1);
         recSumTo(5);
        ";

        public static Exp RecursiveSumTo = Statements(
            Define(
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

        public static string RecursiveSumToCodeScheme = @"
          (define recSumTo
                  (lambda (x) 
                     (if (> 1 x) 
                         0 
                         (+ x (recSumTo (- x 1))))))
          (recSumTo 5)
        ";

        public static string CounterCodeJS = @"
         let makeCounter = function() { let x = 0; return function() { x = x + 1; return x; }; }
         let counter = makeCounter();
         counter();
         counter();
        ";

        public static Exp Counter = Statements(
            Define(
                "makeCounter",
                Function(
                    List<string>(),
                    Statements(
                        Define("x", Number(0)),
                        Function(
                            List<string>(),
                            Statements(
                                SetVar("x", FunctionCall(Symbol("+"), Symbol("x"), Number(1))),
                                Symbol("x")
                            )
                        )
                    )
                )
            ),
            Define("counter", FunctionCall(Symbol("makeCounter"))),
            FunctionCall(Symbol("counter")),
            FunctionCall(Symbol("counter"))
        );

        public static string CounterCodeScheme = @"
          (define makeCounter
                  (lambda ()
                     (define x 0)
                     (lambda ()
                        (set! x (+ x 1))
                        x)))
           (define counter (makeCounter))
           (counter)
           (counter)
        ";

        public static string ComplicatedCounterCodeJS = @"
        let globalCount = 0;
        let makeCounter = function() { let x = 0; return function() { x++; globalCount++; return x; }; }
        let counter1 = makeCounter();
        let counter2 = makeCounter();
        counter1();
        counter1();
        let counter2Value = counter2();
        let counter1Value = counter1();
        [globalCount, counter1Value, counter2Value]
        ";

        public static string ComplicatedCounterCodeScheme = @"
        (define globalCount 0)
        (define makeCounter
                (lambda ()
                   (define x 0)
                   (lambda ()
                      (set! x (+ x 1))
                      (set! globalCount (+ globalCount 1))
                      x)))
        (define counter1 (makeCounter))
        (define counter2 (makeCounter))
        (counter1)
        (counter1)
        (define counter2Value (counter2))
        (define counter1Value (counter1))
        (list globalCount counter1Value counter2Value)
        ";

        public static string BankAccountCodeJS = @"
        let makeBankAccount = function() {
            let balance = 0;
            let withdraw = amt => balance -= amt;
            let deposit = amt => balance += amt;
            let getBalance = () => balance;
            let unknownMethod  = () => null;

            return method =>
                // would use strings, but makes copy pasting into repl harder
                method == 'w' ? withdraw
                : method == 'd' ? deposit
                : method == 'b' ? getBalance
                : unknownMethod;
        }

        let alice = makeBankAccount();
        let bob = makeBankAccount();

        alice('d')(100); // deposit 100
        alice('w')(50);  // withdraw 50
        bob('d')(200);  // deposit 200

        [alice('b')(), bob('b')()] // get balances
        ";

        public static string BankAccountCodeScheme = @"
        (define makeBankAccount
            (lambda ()
                (define balance 0)
                (define withdraw (lambda (amt) (set! balance (- balance amt))))
                (define deposit (lambda (amt) (set! balance (+ balance amt))))
                (define getBalance (lambda () balance))
                (define unknownMethod (lambda () 'ShouldThrowException))

                (lambda (method)
                  (if (eq? method 'withdraw)   withdraw
                  (if (eq? method 'deposit)    deposit
                  (if (eq? method 'getBalance) getBalance 
                      unknownMethod))))))

        (define alice (makeBankAccount))
        (define bob (makeBankAccount))

        ((alice 'deposit) 100)
        ((alice 'withdraw) 50)
        ((bob 'deposit) 200)

        (list ((alice 'getBalance)) ((bob 'getBalance)))
        ";
    }
}