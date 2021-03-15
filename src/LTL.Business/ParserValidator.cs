using System;
using System.ComponentModel.DataAnnotations;

namespace LTL.Business
{
    /// <summary>
    /// Responsible for validatig the parsed values coming from the consumer calling project
    /// </summary>
    public class ParserValidator : IParserValidator
    {
        /// <summary>
        /// Validates the trade data object using custom business logic.
        /// </summary>
        /// <param name="tradeData">The DTO object that encapsulates the logged trade data</param>
        public void Validate(TradeDataDto tradeData)
        {
            // TODO: VALIDATE srike price and underlying
            ValidateStrategy(tradeData, tradeData.Strategy);
        }

        /// <summary>
        /// Validates the specified options strategy that was passed in the input
        /// </summary>
        /// <param name="fields">The parsed fields</param>
        /// <param name="strategy">The parsed strategy</param>
        private static void ValidateStrategy(TradeDataDto fields, OptionsTradingStrategy strategy)
        {
            Action<object> throwStrikeMissingException = (a) => throw new ValidationException($"Error: {a} is missing.");

            switch (strategy)
            {
                case OptionsTradingStrategy.SP:
                    if (!fields.ShortPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortPutStrike));
                    break;
                case OptionsTradingStrategy.SC:
                    if (!fields.ShortCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortCallStrike));
                    break;
                case OptionsTradingStrategy.LC:
                    if (!fields.LongCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongCallStrike));
                    break;
                case OptionsTradingStrategy.LP:
                    if (!fields.LongPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongPutStrike));
                    break;
                case OptionsTradingStrategy.PCS:
                    if (!fields.ShortPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortPutStrike));
                    if (!fields.LongPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongPutStrike));
                    break;
                case OptionsTradingStrategy.CCS:
                    if (!fields.ShortCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortCallStrike));
                    if (!fields.LongCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongCallStrike));
                    break;
                case OptionsTradingStrategy.PDS:
                    if (!fields.LongPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongPutStrike));
                    if (!fields.ShortPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortPutStrike));
                    break;
                case OptionsTradingStrategy.CDS:
                    if (!fields.LongCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongCallStrike));
                    if (!fields.ShortCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortCallStrike));
                    break;
                case OptionsTradingStrategy.IC:
                    if (!fields.LongCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongCallStrike));
                    if (!fields.ShortCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortCallStrike));
                    if (!fields.ShortPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortPutStrike));
                    if (!fields.LongPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongPutStrike));
                    break;
                case OptionsTradingStrategy.LS:
                    if (!fields.LongCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.LongCallStrike));
                    if (!fields.LongPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortPutStrike));
                    break;
                case OptionsTradingStrategy.SS:
                    if (!fields.ShortCallStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortCallStrike));
                    if (!fields.ShortPutStrike.HasValue)
                        throwStrikeMissingException(nameof(fields.ShortPutStrike));
                    break;
                default:
                    throw new ValidationException($"Failed to validate. Unknown strategy specified: {strategy}");
            }
        }
    }
}