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
            //calculatedTradeDto.ProbablityOfProfit = CalculateDaysProbablityOfProfit(calculatedTradeDto);

            logger.Debug("Finished calculating fields!");
            return calculatedTradeDto;
        }


        /// <summary>
        /// Calculates the probablity of profit based on strategy.
        /// </summary>
        /// <param name="calculatedTradeDto"></param>
        /// <returns>The probality of profit</returns>
        private decimal CalculateDaysProbablityOfProfit(TradeDataDto calculatedTradeDto)
        {
            // TODO: Implement and unit test
            throw new NotImplementedException("Need to calculate probablity of profit.");
        }

        /// <summary>
        /// Calculate the risk/reward ratio.
        /// </summary>
        /// <param name="fields">Trade dto</param>
        /// <returns>the risk reward ratio</returns>
        /// <remarks>It is usually recommended to have a 1 to 3 ratio for short positions</remarks>
        private decimal CalculateRiskRewardRatio(TradeDataDto fields)
        {
            decimal ratio = 0;
            if (IsStrategyCredit(fields.Strategy))
            {
                ratio = fields.TotalCredit / fields.MaxRisk;
            }
            else
            {
                if (fields.Strategy == OptionsTradingStrategy.SS)
                    throw new NotImplementedException("Need to figure out unlimited loss ratios. Might need to make this fields nullable.");
                
                ratio = fields.TotalDebit / fields.MaxRisk; // TODO: What about unlimited risk???
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

        private decimal CalculateMaxRisk(TradeDataDto fields)
        {
            decimal maxRisk = 0;
            if (!IsStrategyCredit(fields.Strategy))
            {
                // Max risk is the total debit
                maxRisk = fields.TotalCredit;
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
                        if(!fields.ShortCallStrike.HasValue)
                            throw new NotSupportedException($"Strategy is {fields.Strategy.GetEnumName()}, but no short call strike is specified.");
                        maxRisk = (fields.ShortCallStrike.Value - fields.Price) * OptionsMultipliyer;
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
    }
}