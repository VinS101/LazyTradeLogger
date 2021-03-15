using System.Collections.Generic;
using System.Collections;
using Xunit;
using System;
using System.ComponentModel.DataAnnotations;

namespace LTL.Business.Unit.Tests
{
    public class ParserValidatorTestCases
    {
        /// <summary>
        /// Positive test cases for the parser validator.
        /// </summary>
        public class ParseValidatorValidTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.SP, ShortPutStrike = 25 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.SC, ShortCallStrike = 25 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.LC, LongCallStrike = 25 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.LP, LongPutStrike = 25 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.PCS, ShortPutStrike = 45, LongPutStrike = 40 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.CCS, ShortCallStrike = 35, LongCallStrike = 40 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.PDS, LongPutStrike = 50, ShortPutStrike = 45 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.CDS, LongCallStrike = 40, ShortCallStrike = 35 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.IC, LongCallStrike = 85, ShortCallStrike = 80, LongPutStrike = 35, ShortPutStrike = 40 } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.LS, LongCallStrike = 45, LongPutStrike = 45  } };
                yield return new object[] { new TradeDataDto { Strategy = OptionsTradingStrategy.SS, ShortCallStrike = 85, ShortPutStrike = 35  } };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        }

        /// <summary>
        /// Negative test cases for Parser validator
        /// </summary>
        public class ParserValidatorInvalidTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                Func<Action, Exception> throwsArgumentException = (a) => Assert.Throws<ArgumentException>(a);
                Func<Action, Exception> throwsValidationException = (a) => Assert.Throws<ValidationException>(a);
                yield return new object[] { new TradeDataDto { Strategy = (OptionsTradingStrategy)15 }, throwsValidationException };
                yield return new object[] { new TradeDataDto { Strategy = (OptionsTradingStrategy)15 }, throwsValidationException };
                // yield return new object[] { new TradeDataDto { Strategy = "SP" }, throwsValidationException };
                // yield return new object[] { new TradeDataDto { Strategy = "LP" }, throwsValidationException };
                // yield return new object[] { new TradeDataDto { Strategy = "LC" }, throwsValidationException };
                // yield return new object[] { new TradeDataDto { Strategy = "SC" }, throwsValidationException };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}