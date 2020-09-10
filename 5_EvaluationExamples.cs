using Xunit;
using FluentAssertions;
using static Closures.Evaluator;
using static Closures.ExpHelpers;

namespace Closures
{
    public class EvaluationExamples
    {
        [Fact]
        public void LoopSumToExample()
        {
            var env =  Environment.InitialEnvironment();

            var parsed = Parser.Parse(ASTExamples.LoopSumToCodeScheme);
            parsed.Should().BeEquivalentTo(ASTExamples.LoopSumTo, options => options.RespectingRuntimeTypes());

            var resultFromAST = Eval(ASTExamples.LoopSumTo, env);
            var resultFromParsed = Eval(parsed, env);

            resultFromAST.Should().BeEquivalentTo(Number(15), options => options.RespectingRuntimeTypes());
            resultFromParsed.Should().BeEquivalentTo(Number(15), options => options.RespectingRuntimeTypes());
        }

        [Fact]
        public void RecursiveSumToExample()
        {
            var env =  Environment.InitialEnvironment();

            var parsed = Parser.Parse(ASTExamples.RecursiveSumToCodeScheme);
            parsed.Should().BeEquivalentTo(ASTExamples.RecursiveSumTo, options => options.RespectingRuntimeTypes());

            var resultFromAST = Eval(ASTExamples.RecursiveSumTo, env);
            var resultFromParsed = Eval(parsed, env);

            resultFromAST.Should().BeEquivalentTo(Number(15), options => options.RespectingRuntimeTypes());
            resultFromParsed.Should().BeEquivalentTo(Number(15), options => options.RespectingRuntimeTypes());
        }

        [Fact]
        public void CounterExample()
        {
            var env =  Environment.InitialEnvironment();

            var parsed = Parser.Parse(ASTExamples.CounterCodeScheme);
            parsed.Should().BeEquivalentTo(ASTExamples.Counter, options => options.RespectingRuntimeTypes());

            var resultFromAST = Eval(ASTExamples.Counter, env);
            var resultFromParsed = Eval(parsed, env);

            resultFromAST.Should().BeEquivalentTo(Number(2), options => options.RespectingRuntimeTypes());
            resultFromParsed.Should().BeEquivalentTo(Number(2), options => options.RespectingRuntimeTypes());
        }

        [Fact]
        public void ComplicatedCounterExample()
        {
            var env =  Environment.InitialEnvironment();

            var parsed = Parser.Parse(ASTExamples.ComplicatedCounterCodeScheme);

            var result = Eval(parsed, env);

            result.Should().BeEquivalentTo(
                ListExp(Number(4), Number(3), Number(1)),
                options => options.RespectingRuntimeTypes()
            );
        }

        [Fact]
        public void BankAccountExample()
        {
            var env =  Environment.InitialEnvironment();

            var parsed = Parser.Parse(ASTExamples.BankAccountCodeScheme);

            var result = Eval(parsed, env);

            result.Should().BeEquivalentTo(
                ListExp(Number(50), Number(200)),
                options => options.RespectingRuntimeTypes()
            );
        }
    }
}