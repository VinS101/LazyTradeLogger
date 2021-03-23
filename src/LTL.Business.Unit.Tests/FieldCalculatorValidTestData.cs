using System.Collections;
using System.Collections.Generic;
using static LTL.Business.Unit.Tests.FieldCalculatorCommonTestData;

namespace LTL.Business.Unit.Tests
{
    public class FieldCalculatorValidTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return When.StrategyIsLowerCaseShortPut.ShouldCalculateExpectedFields;
            yield return When.StrategyIsLowerCaseShortPut.ShouldCalculateExpectedFields;
            yield return When.StrategyIsShortCall.ShouldCalculateExpectedFields;
        }

        private static class When
        {
            public static class StrategyIsShortPut
            {
                public static object[] ShouldCalculateExpectedFields
                {
                    get
                    {
                        return new object[]
                        {
                            new TradeDataDto
                            {
                                Ticker = "MSFT",
                                ExpiryDate = FrozenExpiratyDate,
                                Delta = new decimal(0.3),
                                Price = new decimal(1.72),
                                Underlying = new decimal(243),
                                Strategy = OptionsTradingStrategy.SP,
                                ShortPutStrike = 220,
                                CommentsAtOpen = "Naked put for MSFT!"
                            },
                            new TradeDataDto
                            {
                                Ticker = "MSFT",
                                OpenDate = FrozenTime,
                                ExpiryDate = FrozenExpiratyDate,
                                Quantity = 1,
                                Delta = new decimal(0.3),
                                Price = new decimal(1.72),
                                Underlying = new decimal(243),
                                Strategy = OptionsTradingStrategy.SP,
                                ShortPutStrike = 220,
                                CommentsAtOpen = "Naked put for MSFT!",
                                TotalCredit = new decimal(172.00),
                                MaxRisk = new decimal(21828),
                                DTE = 7, // Expiration date - DateTime.Now
                                RiskRewardRatio = new decimal(0.0079), // Max profit / max risk
                                DaysInTrade = 0,
                                TradeStatus = TradeStatus.OPEN
                            },
                        };
                    }
                }
            }
            public static class StrategyIsLowerCaseShortPut
            {
                public static object[] ShouldCalculateExpectedFields
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
                                Strategy = OptionsTradingStrategy.SP,
                                ShortPutStrike = 220,
                                CommentsAtOpen = "Naked put for MSFT!"
                            },
                            new TradeDataDto
                            {
                                Ticker = "MSFT",
                                OpenDate = FrozenTime,
                                ExpiryDate = FrozenExpiratyDate,
                                Quantity = 1,
                                Delta = new decimal(0.3),
                                Price = new decimal(1.72),
                                Underlying = new decimal(243),
                                Strategy = OptionsTradingStrategy.SP,
                                ShortPutStrike = 220,
                                CommentsAtOpen = "Naked put for MSFT!",
                                TotalCredit = new decimal(172.00),
                                MaxRisk = new decimal(21828),
                                DTE = 7, // Expiration date - DateTime.Now
                                RiskRewardRatio = new decimal(0.0079), // Max profit / max risk
                                DaysInTrade = 0,
                                TradeStatus = TradeStatus.OPEN
                            },
                        };
                    }
                }
            }

            public static class StrategyIsShortCall
            {
                public static object[] ShouldCalculateExpectedFields
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
                                ShortCallStrike = 250,
                                CommentsAtOpen = "Naked call option for MSFT!"
                            },
                            new TradeDataDto
                            {
                                Ticker = "MSFT",
                                OpenDate = FrozenTime,
                                ExpiryDate = FrozenExpiratyDate,
                                Quantity = 1,
                                Delta = new decimal(0.3),
                                Price = new decimal(1.72),
                                Underlying = new decimal(243),
                                Strategy = OptionsTradingStrategy.SC,
                                ShortCallStrike = 250,
                                CommentsAtOpen = "Naked call option for MSFT!",
                                TotalCredit = new decimal(172.00),
                                MaxRisk = new decimal(24828),
                                DTE = 7, // Expiration date - DateTime.Now
                                RiskRewardRatio = new decimal(0.0069), // Max profit / max risk
                                DaysInTrade = 0,
                                TradeStatus = TradeStatus.OPEN
                            },
                        };
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}