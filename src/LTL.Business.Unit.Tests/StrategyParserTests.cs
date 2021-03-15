using Xunit;
using System;
using static LTL.Business.StrategyParser;
using static LTL.Business.Unit.Tests.StrategyParserTestCases;

namespace LTL.Business.Unit.Tests
{
    public static class StrategyParserTests
    {

        [Theory]
        [ClassData(typeof(StrategyParserValidTestCases))]
        public static void ParseStrategy_WhenValidStrategyAndStikesProvided_ShouldNotThrowException(string strategy, OptionsTradingStrategy expectedStrategy)
        {
            // ACT
            Assert.Equal(expectedStrategy, ParseStrategy(strategy));
        }

        [Theory]
        [ClassData(typeof(StrategyParserInvalidTestCases))]
        public static void Validate_WhenInvalidStrategyAndStikesProvided_ShouldThrowException(string strategy, Func<Action, Exception> assertion)
        {
            // ACT
            assertion(() => ParseStrategy(strategy));
        }
    }
}
