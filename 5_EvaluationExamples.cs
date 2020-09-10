using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static Closures.ExpHelpers;
using Xunit;
using FluentAssertions;
using static Closures.Evaluator;

namespace Closures
{
    public class EvaluationExamples
    {
        [Fact]
        public void LoopSumToExample()
        {
            var env =  Environment.InitialEnvironment();

            var parsed = Parser.Parse(ASTExamples.LoopSumToCode);
            parsed.Should().BeEquivalentTo(ASTExamples.LoopSumTo, options => options.RespectingRuntimeTypes());

            var resultFromAST = Eval(ASTExamples.LoopSumTo, env);
            var resultFromParsed = Eval(parsed, env);

            (resultFromAST as Number).Value.Should().Be(15);
            (resultFromParsed as Number).Value.Should().Be(15);
        }

        [Fact]
        public void RecursiveSumToExample()
        {
            var env =  Environment.InitialEnvironment();

            var parsed = Parser.Parse(ASTExamples.RecursiveSumToCode);
            parsed.Should().BeEquivalentTo(ASTExamples.RecursiveSumTo, options => options.RespectingRuntimeTypes());

            var resultFromAST = Eval(ASTExamples.RecursiveSumTo, env);
            var resultFromParsed = Eval(parsed, env);

            (resultFromAST as Number).Value.Should().Be(15);
            (resultFromParsed as Number).Value.Should().Be(15);
        }

        [Fact]
        public void CounterExample()
        {
            var env =  Environment.InitialEnvironment();

            var parsed = Parser.Parse(ASTExamples.CounterCode);
            parsed.Should().BeEquivalentTo(ASTExamples.Counter, options => options.RespectingRuntimeTypes());

            var resultFromAST = Eval(ASTExamples.Counter, env);
            var resultFromParsed = Eval(parsed, env);

            (resultFromAST as Number).Value.Should().Be(2);
            (resultFromParsed as Number).Value.Should().Be(2);
        }
    }
}