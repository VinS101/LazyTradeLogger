using System.Collections.Generic;
using System.Collections;
using Xunit;
using System;

namespace LTL.Business.Unit.Tests
{
    public class StrategyParserTestCases
    {
        /// <summary>
        /// Positive test cases for the parser validator.
        /// </summary>
        public class StrategyParserValidTestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "SHORT PUT", OptionsTradingStrategy.SP };
                yield return new object[] { "short put", OptionsTradingStrategy.SP };
                yield return new object[] { "SP", OptionsTradingStrategy.SP };
                yield return new object[] { "sp", OptionsTradingStrategy.SP };

                yield return new object[] { "SHORT CALL", OptionsTradingStrategy.SC };
                yield return new object[] { "short call", OptionsTradingStrategy.SC };
                yield return new object[] { "sc", OptionsTradingStrategy.SC };
                yield return new object[] { "SC", OptionsTradingStrategy.SC };


                yield return new object[] { "LONG CALL", OptionsTradingStrategy.LC };
                yield return new object[] { "long call", OptionsTradingStrategy.LC };
                yield return new object[] { "lc", OptionsTradingStrategy.LC };
                yield return new object[] { "LC", OptionsTradingStrategy.LC };

                yield return new object[] { "PUT CREDIT SPREAD", OptionsTradingStrategy.PCS };
                yield return new object[] { "put credit spread", OptionsTradingStrategy.PCS };
                yield return new object[] { "pcs", OptionsTradingStrategy.PCS };
                yield return new object[] { "PCS", OptionsTradingStrategy.PCS };

                yield return new object[] { "CALL CREDIT SPREAD", OptionsTradingStrategy.CCS };
                yield return new object[] { "call credit spread", OptionsTradingStrategy.CCS };
                yield return new object[] { "ccs", OptionsTradingStrategy.CCS };
                yield return new object[] { "CCS", OptionsTradingStrategy.CCS };

                yield return new object[] { "PUT DEBIT SPREAD", OptionsTradingStrategy.PDS };
                yield return new object[] { "put debit spread", OptionsTradingStrategy.PDS };
                yield return new object[] { "PDS", OptionsTradingStrategy.PDS };
                yield return new object[] { "pds", OptionsTradingStrategy.PDS };

                yield return new object[] { "CALL DEBIT SPREAD", OptionsTradingStrategy.CDS };
                yield return new object[] { "call debit spread", OptionsTradingStrategy.CDS };
                yield return new object[] { "CDS", OptionsTradingStrategy.CDS };
                yield return new object[] { "cds", OptionsTradingStrategy.CDS };
                
                yield return new object[] { "IRON CONDOR", OptionsTradingStrategy.IC };
                yield return new object[] { "iron condor", OptionsTradingStrategy.IC };
                yield return new object[] { "IC", OptionsTradingStrategy.IC };
                yield return new object[] { "ic", OptionsTradingStrategy.IC };

                yield return new object[] { "LONG STRANGLE", OptionsTradingStrategy.LS };
                yield return new object[] { "long strangle", OptionsTradingStrategy.LS };
                yield return new object[] { "LS", OptionsTradingStrategy.LS };
                yield return new object[] { "ls", OptionsTradingStrategy.LS };

                yield return new object[] { "SHORT STRANGLE", OptionsTradingStrategy.SS };
                yield return new object[] { "short strangle", OptionsTradingStrategy.SS };
                yield return new object[] { "SS", OptionsTradingStrategy.SS };
                yield return new object[] { "ss", OptionsTradingStrategy.SS };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        }

        /// <summary>
        /// Negative test cases for Parser validator
        /// </summary>
        public class StrategyParserInvalidTestCases : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                Func<Action, Exception> throwsArgumentException = (a) => Assert.Throws<ArgumentException>(a);
                yield return new object[] { "GGGG", throwsArgumentException };
                yield return new object[] { "SPc", throwsArgumentException };
                yield return new object[] { "LPb", throwsArgumentException };
                yield return new object[] { "LCf", throwsArgumentException };
                yield return new object[] { "SCa", throwsArgumentException };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}