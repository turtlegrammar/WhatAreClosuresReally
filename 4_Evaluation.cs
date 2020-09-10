using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static Closures.ExpHelpers;

namespace Closures
{
    public class Environment
    {

        private readonly ImmutableList<Dictionary<string, Exp>> _scopes;

        public Environment(Dictionary<string, Exp> globalScope)
        {
            _scopes = new List<Dictionary<string, Exp>> { globalScope }.ToImmutableList();
        }

        public Environment(ImmutableList<Dictionary<string, Exp>> scopes)
        {
            _scopes = scopes;
        }

        private bool ScopeForVariable(string variable, out Dictionary<string, Exp> scope)
        {
            for (var i = _scopes.Count - 1; i >= 0; i--)
            {
                if (_scopes[i].ContainsKey(variable))
                {
                    scope = _scopes[i];
                    return true;
                }
            }

            scope = null;
            return false;
        }

        public void Bind(string variable, Exp value)
        {
            if (ScopeForVariable(variable, out var scope))
            {
                scope[variable] = value;
            }
            else 
            {
                _scopes.Last()[variable] = value;
            }
        }

        public Exp ValueOf(string variable) => ScopeForVariable(variable, out var scope)
            ? scope[variable]
            : throw new Exception($"Unbound variable: {variable}");

        public Environment NewScope(IEnumerable<(string varName, Exp value)> bindings)
        {
            return new Environment(_scopes.Add(bindings.ToDictionary(tup => tup.varName, tup => tup.value)));
        }

        ///
        ///
        ///

        public static Environment Empty = new Environment(new List<Dictionary<string, Exp>>().ToImmutableList());

        public static Environment InitialEnvironment() =>
            new Environment(
                new Dictionary<string, Exp>
                {
                    {"+", PrimitiveFunction(args => Number( (args[0] as Number).Value + (args[1] as Number).Value))},
                    {"-", PrimitiveFunction(args => Number( (args[0] as Number).Value - (args[1] as Number).Value))},
                    {">", PrimitiveFunction(args => Bool( (args[0] as Number).Value > (args[1] as Number).Value))},
                }
            );
    }


    public static class Evaluator
    {
        public static Exp Eval(Exp exp, Environment env)
        {
            return exp switch
            {
                Number n => n,
                Symbol s => env.ValueOf(s.Value),
                Null n => n,
                Bool b => b,

                Assign a => EvalAssign(a),
                If i => EvalIf(i),
                Sequence s => EvalSequence(s),
                While w => EvalWhile(w),

                CreateFunction cf => new FunctionObject { Parameters = cf.Parameters, Body = cf.Body, Environment = env },
                FunctionObject f => f,
                PrimitiveFunction pf => pf,

                FunctionCall fc => EvalFunctionCall(fc),

                _ => throw new Exception($"Unknown type: {exp.GetType()}")
            };

            Exp EvalAssign(Assign assign)
            {
                var evaluatedValue = Eval(assign.Value, env);
                env.Bind(assign.Variable, evaluatedValue);
                return evaluatedValue;
            }

            Exp EvalIf(If ifExp)
            {
                var condition = Eval(ifExp.Condition, env);
                return (condition is Bool b && b.Value)
                    ? Eval(ifExp.TrueBranch, env)
                    : Eval(ifExp.FalseBranch, env);
            }

            Exp EvalSequence(Sequence sequence)
            {
                Exp resultExp = ExpHelpers.Void; 
                foreach (var exp in sequence.Expressions)
                {
                    resultExp = Eval(exp, env);
                }
                return resultExp;
            }

            Exp EvalWhile(While w)
            {
                var resultExp = ExpHelpers.Void;

                while (true)
                {
                    var evalCondition = Eval(w.Condition, env);
                    if (evalCondition is Bool b && b.Value)
                    {
                        resultExp = Eval(w.LoopBody, env);
                    }
                    else
                    {
                        return resultExp;
                    }
                }
            }

            Exp EvalFunctionCall(FunctionCall fc)
            {
                var evaluatedFunction = Eval(fc.Function, env);
                var evaluatedArgs = fc.Arguments.Select(arg => Eval(arg, env)).ToList();

                if (evaluatedFunction is FunctionObject functionObject)
                {
                    var extendedFunctionEnvironment = functionObject.Environment.NewScope(
                        functionObject.Parameters.Zip(evaluatedArgs)
                    );

                    return Eval(functionObject.Body, extendedFunctionEnvironment);
                }

                else if (evaluatedFunction is PrimitiveFunction primitive)
                {
                    return primitive.Work(evaluatedArgs);
                }

                else 
                {
                    throw new Exception($"Can't apply non-function of type {evaluatedFunction.GetType()}");
                }
            }
        }
    }
}