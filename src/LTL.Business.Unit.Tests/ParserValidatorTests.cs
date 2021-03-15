using Xunit;
using System;
using static LTL.Business.Unit.Tests.ParserValidatorTestCases;

namespace LTL.Business.Unit.Tests
{
    public static class ParserValidatorTests
    {

        [Theory]
        [ClassData(typeof(ParseValidatorValidTestData))]
        public static void Validate_WhenValidStrategyAndStikesProvided_ShouldNotThrowException(TradeDataDto fields)
        {
            // ARRANGE
            var validator = new ParserValidator();

            // ACT
            validator.Validate(fields);
        }

        [Theory]
        [ClassData(typeof(ParserValidatorInvalidTestData))]
        public static void Validate_WhenInvalidStrategyAndStikesProvided_ShouldThrowException(TradeDataDto fields, Func<Action, Exception> assertion)
        {
            // ARRANGE
            var validator = new ParserValidator();

            // ACT
            assertion(() => validator.Validate(fields));
        }
    }
}
