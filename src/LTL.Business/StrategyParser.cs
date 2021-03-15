using System;
using static LTL.Common.EnumExtensions;

namespace LTL.Business
{
    public static class StrategyParser
    {
        public static OptionsTradingStrategy ParseStrategy(string strategy)
        {
            if (string.IsNullOrWhiteSpace(strategy))
                throw new ArgumentException(nameof(strategy));
                
            if (!Enum.TryParse(value: strategy, ignoreCase: true, result: out OptionsTradingStrategy parsedStrategy))
            {
                if (!TryGetEnumValueFromDescription<OptionsTradingStrategy>(strategy?.ToLower(), out OptionsTradingStrategy parsedDescription))
                {
                    // Nither enum or the friendly name were specified.
                    throw new ArgumentException($"Error: Strategy: '{strategy}' is unknown. Please choose from the following list: {string.Join(",", GetEnumDescriptions(typeof(OptionsTradingStrategy)))}");
                }
                return parsedDescription;
            }
            return parsedStrategy;
        }
    }
}