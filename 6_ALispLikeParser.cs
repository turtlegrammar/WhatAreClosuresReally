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
            from ids in  many(between(spaces, spaces, Identifier))
            from close in ch(')')
            select ids.ToList();

        public static Parser<Exp> ParseCreateFunction =
            from fnKeyword in str("lambda")
            from argList in between(spaces, spaces, ParseArgsList)
            from body in many1(ParseExp)
            select Function(argList, AsSeq(body.ToArray()));

        public static Parser<Exp> ParseSetVar =
            from assignKeyword in str("set!")
            from lvalue in between(spaces, spaces, Identifier)
            from rvalue in ParseExp 
            select SetVar(lvalue, rvalue);

        public static Parser<Exp> ParseDefine =
            from assignKeyword in str("define")
            from lvalue in between(spaces, spaces, Identifier)
            from rvalue in ParseExp 
            select SetVar(lvalue, rvalue);

        public static Parser<Exp> ParseIf = 
            from ifKeyword in str("if")
            from condition in between(spaces, spaces, ParseExp)
            from trueBranch in between(spaces, spaces, ParseExp)
            from falseBranch in ParseExp
            select If(condition, trueBranch, falseBranch);

        public static Parser<Exp> ParseWhile = 
            from whileKeyword in str("while")
            from condition in between(spaces, spaces, ParseExp)
            from body in many1(ParseExp)
            select While(condition, AsSeq(body.ToArray()));

        public static Parser<Exp> ParseSpecialFormOrFnApplication = 
            from open in ch('(')
            from result in between(spaces, spaces, choice(
                attempt(ParseCreateFunction),
                attempt(ParseSetVar),
                attempt(ParseDefine),
                attempt(ParseIf),
                attempt(ParseWhile),
                from list in many1(ParseExp) select FunctionCall(list.First(), list.Skip(1).ToArray())
            ))
            from close in ch(')')
            select result;


        public static Parser<Exp> ParseExp = 
            from result in between(spaces, spaces, choice(
                attempt(ParseNumber),
                attempt(ParseSymbol),
                ParseSpecialFormOrFnApplication
            ))
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