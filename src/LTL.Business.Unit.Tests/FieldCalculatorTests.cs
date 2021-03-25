using System;
using LTL.Common;
using Xunit;
using Moq;
using Xunit.Abstractions;
using SemanticComparison;
using static LTL.Business.Unit.Tests.FieldCalculatorCommonTestData;

namespace LTL.Business.Unit.Tests
{
    public class FieldCalculatorTests
    {
        private readonly ITestOutputHelper logger;

        public FieldCalculatorTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Theory]
        [ClassData(typeof(FieldCalculatorValidTestData))]
        public void ComputeCalculatedFields_WhenValidFieldsArePresent_ShouldCalculateFields(TradeDataDto tradeDataDto, TradeDataDto expected)
        {
            // ARRANGE
            var dateTimeProvider = new Mock<ITimeProvider>();
            dateTimeProvider.Setup(c => c.Now).Returns(FrozenTime);
            var fieldCalculator = new FieldCalculator(dateTimeProvider.Object, new Mock<NLog.ILogger>().Object);

            // ACT
            TradeDataDto tradeDataWithComputedFields = fieldCalculator.ComputeCalculatedFields(tradeDataDto);

            // ASSERT
            // For debugging:
            var expectedObject = Newtonsoft.Json.JsonConvert.SerializeObject(expected);
            var computed = Newtonsoft.Json.JsonConvert.SerializeObject(tradeDataWithComputedFields);
            logger.WriteLine("Expecting:");
            logger.WriteLine(expectedObject);
            
            
            logger.WriteLine("Actual:");
            logger.WriteLine(computed);

            var expectedLikeness = new Likeness<TradeDataDto, TradeDataDto>(expected);
            
            expectedLikeness.ShouldEqual(tradeDataWithComputedFields);
        }

        [Theory]
        [ClassData(typeof(FieldCalculatorInvalidTestData))]
        public static void ComputeCalculatedFields_WhenInValidFieldsArePresent_ShouldThrowException(TradeDataDto tradeDataDto, Func<Action, Exception> assertion)
        {
            // ARRANGE
            var dateTimeProvider = new Mock<ITimeProvider>();
            dateTimeProvider.Setup(c => c.Now).Returns(FrozenTime);
            var fieldCalculator = new FieldCalculator(dateTimeProvider.Object, new Mock<NLog.ILogger>().Object);

            // ACT
            // Assert
            assertion(() => fieldCalculator.ComputeCalculatedFields(tradeDataDto));
        }
    }
}