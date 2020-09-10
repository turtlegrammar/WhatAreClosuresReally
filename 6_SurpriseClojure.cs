using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static Closures.ExpHelpers;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Closures
{

    public static class Parser 
    {
        public static Exp AsSeq(Exp[] exps) =>
            exps.Length == 1 ? exps[0] : Sequence(exps);

        public static Parser<string> Identifier = asString(many1(noneOf(" ()\r\n")));

        public static Parser<Exp> ParseNumber =
            from n in asString(many1(digit)) select Number(Int32.Parse(n));

        public static Parser<Exp> ParseSymbol =
            from s in Identifier select Symbol(s);

        public static Parser<List<string>> ParseArgsList =
            from open in ch('(')
            from spaces1 in spaces 
            from ids in  many(between(spaces, spaces, Identifier))
            from close in ch(')')
            select ids.ToList();

        public static Parser<Exp> ParseCreateFunction =
            from fnKeyword in str("function")
            from spaces1 in spaces 
            from argList in ParseArgsList
            from spaces2 in spaces
            from body in many1(ParseExp)
            select Function(argList, AsSeq(body.ToArray()));

        public static Parser<Exp> ParseAssign =
            from assignKeyword in str("assign")
            from spaces1 in spaces
            from lvalue in Identifier
            from spaces2 in spaces
            from rvalue in ParseExp 
            select Assign(lvalue, rvalue);

        public static Parser<Exp> ParseIf = 
            from ifKeyword in str("if")
            from spaces1 in spaces 
            from condition in ParseExp
            from spaces2 in spaces 
            from trueBranch in ParseExp 
            from spaces3 in spaces 
            from falseBranch in ParseExp
            select If(condition, trueBranch, falseBranch);

        public static Parser<Exp> ParseWhile = 
            from whileKeyword in str("while")
            from spaces1 in spaces 
            from condition in ParseExp
            from spaces2 in spaces 
            from body in many1(ParseExp)
            from spaces3 in spaces 
            select While(condition, AsSeq(body.ToArray()));

        public static Parser<Exp> ParseSpecialFormOrFnApplication = 
            from open in ch('(')
            from spaces2 in spaces 
            from result in choice(
                attempt(ParseCreateFunction),
                attempt(ParseAssign),
                attempt(ParseIf),
                attempt(ParseWhile),
                from list in many1(ParseExp) select FunctionCall(list.First(), list.Skip(1).ToArray())
            )
            from spaces3 in spaces
            from close in ch(')')
            select result;


        public static Parser<Exp> ParseExp = 
            from s1 in spaces
            from result in choice(
                attempt(ParseNumber),
                attempt(ParseSymbol),
                ParseSpecialFormOrFnApplication
            )
            from s2 in spaces 
            select result;

        public static Exp Parse(string code)
        {
            var parseResult = parse(many1(ParseExp), code);
            if (parseResult.IsFaulted)
            {
                throw new Exception(parseResult.ToString());
            }
            else
            {
                return AsSeq(parseResult.Reply.Result.ToArray());
            }
        } 
    }
}