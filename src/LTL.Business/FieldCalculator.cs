using System;
using LTL.Common;
using NLog;

namespace LTL.Business
{
    public class FieldCalculator : IFieldCalculator
    {
        private ITimeProvider dateTimeProvider;
        private readonly ILogger logger;
        private const decimal OptionsMultipliyer = 100;
        private const int roundingDecimals = 4;

        public FieldCalculator(
            ITimeProvider dateTimeProvider,
            ILogger logger)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.logger = logger;
        }

        public TradeDataDto ComputeCalculatedFields(TradeDataDto incomingFields)
        {
            logger.Debug("Computing calculated fields...");

            if (incomingFields is null)
                throw new ArgumentException("Error: Fields is null", nameof(incomingFields));

            var calculatedTradeDto = new TradeDataDto
            {
                Ticker = incomingFields.Ticker.ToUpper(),
                Strategy = incomingFields.Strategy,
                OpenDate = incomingFields.OpenDate ?? dateTimeProvider.Now,
                ExpiryDate = incomingFields.ExpiryDate,
                Quantity = incomingFields.Quantity == 0 ? 1 : incomingFields.Quantity,
                Delta = incomingFields.Delta,
                Price = incomingFields.Price,
                Underlying = incomingFields.Underlying, // TODO: Get a live quote?
                ShortPutStrike = incomingFields.ShortPutStrike,
                ShortCallStrike = incomingFields.ShortCallStrike,
                LongCallStrike = incomingFields.LongCallStrike,
                LongPutStrike = incomingFields.LongPutStrike,
                CommentsAtOpen = incomingFields.CommentsAtOpen,
                CommentsAtClose = incomingFields.CommentsAtClose,
            };

            calculatedTradeDto.TotalCredit = CalculateTotalCredit(incomingFields);
            calculatedTradeDto.TotalDebit = CalculateTotalDebit(calculatedTradeDto);
            calculatedTradeDto.MaxRisk = CalculateMaxRisk(calculatedTradeDto);
            calculatedTradeDto.DTE = CalculateDateUntilExpiration(calculatedTradeDto);
            calculatedTradeDto.RiskRewardRatio = CalculateRiskRewardRatio(calculatedTradeDto);
            calculatedTradeDto.DaysInTrade = CalculateDaysInTrade(calculatedTradeDto);
            calculatedTradeDto.MaxProfit = CalculateMaxProfit(calculatedTradeDto);
            calculatedTradeDto.ProbablityOfProfit = CalculateProbablityOfProfit(calculatedTradeDto); // TODO: To be implemented soon

            logger.Debug("Finished calculating fields!");
            return calculatedTradeDto;
        }

        /// <summary>
        /// Calculates the probablity of profit based on strategy.
        /// </summary>
        /// <param name="calculatedTradeDto"></param>
        /// <returns>The probality of profit</returns>
        private decimal? CalculateProbablityOfProfit(TradeDataDto calculatedTradeDto)
        {
            // TODO: Implement and unit test
            decimal? probablityOfProfit = 0;
            switch (calculatedTradeDto.Strategy)
            {
                case OptionsTradingStrategy.SP:
                    probablityOfProfit = CalculateShortPutProbablityOfProfit(calculatedTradeDto);
                    break;
                case OptionsTradingStrategy.PCS:
                    probablityOfProfit = CalculatePutCreditSpreadProbablityOfProfit(calculatedTradeDto);
                    break;
                default:
                    throw new NotImplementedException("Need to calculate probablity of profit.");
            }
            throw new NotImplementedException("Need to calculate probablity of profit.");
        }

        private decimal? CalculatePutCreditSpreadProbablityOfProfit(TradeDataDto calculatedTradeDto)
        {
            if (!calculatedTradeDto.ShortPutStrike.HasValue)
                throw new InvalidOperationException("Short put strike is not specified");
            if (!calculatedTradeDto.LongPutStrike.HasValue)
                throw new InvalidOperationException("Long put strike is not specified");

            return 100 - (calculatedTradeDto.Price / ((calculatedTradeDto.ShortPutStrike - calculatedTradeDto.LongPutStrike) * 100));
        }

        private decimal CalculateShortPutProbablityOfProfit(TradeDataDto calculatedTradeDto)
        {
            if (!calculatedTradeDto.ShortCallStrike.HasValue)
                throw new InvalidOperationException("Short put is not specified");

            // 1.Breakeven = Strike price - Premium collected
            decimal breakeven = calculatedTradeDto.ShortPutStrike.Value - calculatedTradeDto.Price;
            // 2. Calculate the probablity of ITM for the breakeven --> How do we do this exactly? Need the formula.
            //      - Calculate the delta of the breakeven strike
            //      -
            // Source: https://www.optionmatters.ca/delta-assessing-probabilities-based-break-even-price/ 
            //var probablityOfInTheMoney = ???;
            // 3. Substract from 100
            throw new NotImplementedException("Need to obtain the delta of the break-even of the short put from a finance API to proceed further and calculate the PoP for short puts.");
        }

        /// <summary>
        /// Calculate the risk/reward ratio.
        /// </summary>
        /// <param name="fields">Trade dto</param>
        /// <returns>the risk reward ratio</returns>
        /// <remarks>It is usually recommended to have a 1 to 3 ratio for short positions</remarks>
        private decimal? CalculateRiskRewardRatio(TradeDataDto fields)
        {
            decimal ratio = 0;
            if (IsStrategyCredit(fields.Strategy))
            {
                if (fields.MaxRisk.HasValue)
                    ratio = fields.TotalCredit / fields.MaxRisk.Value;
            }
            else
            {
                if (fields.Strategy == OptionsTradingStrategy.SS)
                    throw new NotImplementedException("Need to figure out unlimited loss ratios. Might need to make this fields nullable.");

                if (!IsStrategyNakedLong(fields.Strategy))
                {
                    if (fields.MaxRisk.HasValue)
                        ratio = fields.TotalDebit / fields.MaxRisk.Value; // TODO: What about unlimited risk???
                }
                else
                {
                    return null; // No risk to reward ratio for naked long options
                }
            }
            return Decimal.Round(ratio, roundingDecimals);
        }

        /// <summary>
        /// Current time - start open date
        /// </summary>
        /// <param name="fields"></param>
        /// <returns>an integer that represents the days in trade</returns>
        private int CalculateDaysInTrade(TradeDataDto fields)
        {
            if (!fields.OpenDate.HasValue)
                throw new ArgumentException(nameof(fields.OpenDate));

            return (dateTimeProvider.Now - fields.OpenDate.Value).Days;
        }

        private int CalculateDateUntilExpiration(TradeDataDto fields)
        {
            return (fields.ExpiryDate - dateTimeProvider.Now).Days;
        }

        private decimal? CalculateMaxRisk(TradeDataDto fields)
        {
            decimal? maxRisk = 0;
            if (!IsStrategyCredit(fields.Strategy))
            {
                // Max risk is the total debit
                maxRisk = fields.TotalDebit;
            }
            else
            {
                switch (fields.Strategy)
                {
                    case OptionsTradingStrategy.SP:
                        if (!fields.ShortPutStrike.HasValue)
                            throw new NotSupportedException("Strategy is short put but no short put strike is specified.");
                        maxRisk = (fields.ShortPutStrike.Value - fields.Price) * OptionsMultipliyer;
                        break;
                    case OptionsTradingStrategy.SC:
                        if (!fields.ShortCallStrike.HasValue)
                            throw new NotSupportedException($"Strategy is {fields.Strategy.GetEnumName()}, but no short call strike is specified.");
                        maxRisk = (fields.ShortCallStrike.Value - fields.Price) * OptionsMultipliyer;
                        break;
                    case OptionsTradingStrategy.PCS:
                        if (!fields.ShortPutStrike.HasValue)
                            throw new NotSupportedException($"Strategy is {fields.Strategy.GetEnumName()}, but no short put strike is specified.");
                        if (!fields.LongPutStrike.HasValue)
                            throw new NotSupportedException($"Strategy is {fields.Strategy.GetEnumName()}, but no long put strike is specified.");
                        maxRisk = (fields.ShortPutStrike - fields.LongPutStrike - fields.Price) * 100;
                        break;
                    default:
                        throw new NotSupportedException($"Max risk cannot be determined for strategy: {fields.Strategy.GetEnumName()}");
                }

            }

            return maxRisk;
        }

        /// <summary>
        /// Calculates the total debit paid for the options strategy
        /// </summary>
        /// <param name="fields">Trade Data</param>
        /// <returns>a decimal that represents the total debit paid</returns>
        private decimal CalculateTotalDebit(TradeDataDto fields)
        {
            decimal totalDebit = 0;
            if (!IsStrategyCredit(fields.Strategy))
            {
                totalDebit = fields.Price * OptionsMultipliyer;
            }
            return totalDebit;
        }

        /// <summary>
        /// Calculates the total credit recieved If the strategy short
        /// </summary>
        /// <param name="fields">Trade data</param>
        /// <returns>a decimal that represents the total credit recieved.</returns>
        private decimal CalculateTotalCredit(TradeDataDto fields)
        {
            decimal totalCredit = 0;
            if (IsStrategyCredit(fields.Strategy))
            {
                totalCredit = fields.Price * OptionsMultipliyer;
            }
            return Decimal.Round(totalCredit, roundingDecimals);
        }

        /// <summary>
        /// Calculates the max profit for the trade
        /// </summary>
        /// <param name="calculatedTradeDto">The object that represents the trade</param>
        /// <returns>The max profit. If there is unlimited loss, it returns null</returns>
        private decimal? CalculateMaxProfit(TradeDataDto calculatedTradeDto)
        {
            decimal? maxProfit = default(decimal?);
            if (IsStrategyCredit(calculatedTradeDto.Strategy))
            {
                maxProfit = calculatedTradeDto.TotalCredit;
            }
            else
            {
                switch (calculatedTradeDto.Strategy)
                {
                    case OptionsTradingStrategy.CDS:
                        break;
                    case OptionsTradingStrategy.PDS:
                        break;
                    default:
                        break;
                }
            }
            return maxProfit;
        }

        /// <summary>
        /// Determines whether the strategy is a short or long strategy
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns><c>true</c> if the strategy is a short strategy. Otherwise false.</returns>
        /// TODO: Extract into a standalone class later?
        private bool IsStrategyCredit(OptionsTradingStrategy strategy) =>
            strategy == OptionsTradingStrategy.SP ||
            strategy == OptionsTradingStrategy.SC ||
            strategy == OptionsTradingStrategy.PCS ||
            strategy == OptionsTradingStrategy.IC ||
            strategy == OptionsTradingStrategy.SS;

        /// <summary>
        /// Determines wheter a strategy is naked or not.
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns><c>True if the strategy is naked. Otherwise, false.<c></returns>
        private bool IsStrategyNakedLong(OptionsTradingStrategy strategy) =>
            strategy == OptionsTradingStrategy.LC ||
            strategy == OptionsTradingStrategy.LP ||
            strategy == OptionsTradingStrategy.LS;

        // {
        //     var IsStrategyNaked = false;
        //     switch (strategy)
        //     {
        //         case OptionsTradingStrategy.LC:
        //             IsStrategyNaked = true;
        //             break;
        //         case OptionsTradingStrategy.LP:
        //             IsStrategyNaked = true;
        //             break;
        //         case OptionsTradingStrategy.LS:
        //             IsStrategyNaked = true;
        //             break;
        //         default:
        //             break;
        //     }
        //     return IsStrategyNaked;
        // }
    }
}
