using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using static LTL.Business.Unit.Tests.FieldCalculatorCommonTestData;

namespace LTL.Business.Unit.Tests
{
    public class FieldCalculatorInvalidTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {

            yield return When.Null;
            //yield return TestCases.ShortPut.StrikePriceIsHigherThanUnderlying;
            yield return When.ShortCallHasNoStrike.ShouldThrowNotSuppportedException;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static class When
        {
            private static Func<Action, Exception> throwsArgumentException = (a) => Assert.Throws<ArgumentException>(a);
            private static Func<Action, NotSupportedException> NotSuppertedException = (a) => Assert.Throws<NotSupportedException>(a);
            public static object[] Null => new object[] { null, throwsArgumentException };

            public class ShortPut
            {
                // TODO: To validate or not validate? Should this test case target the Parser Validator? It feels like that it doesnt' belong here...
                public static object[] StrikePriceIsHigherThanUnderlying => new object[]
                {
                    new TradeDataDto
                    {
                        Ticker = "MSFT",
                        ExpiryDate = FrozenExpiratyDate,
                        Delta = new decimal(0.3),
                        Price = new decimal(1.72),
                        Underlying = new decimal(243),
                        Strategy = OptionsTradingStrategy.SP,
                        ShortPutStrike = 250,
                        CommentsAtOpen = "Naked put for MSFT!"
                    },
                    NotSuppertedException
                };
            }

            public static class ShortCallHasNoStrike
            {
                public static object[] ShouldThrowNotSuppportedException
                {
                    get
                    {
                        return new object[]
                        {
                            new TradeDataDto
                            {
                                Ticker = "msft",
                                ExpiryDate = FrozenExpiratyDate,
                                Delta = new decimal(0.3),
                                Price = new decimal(1.72),
                                Underlying = new decimal(243),
                                Strategy = OptionsTradingStrategy.SC,
                                CommentsAtOpen = "Naked put for MSFT!"
                            },
                            NotSuppertedException
                        };
                    }
                }
            }
        }
    }
}