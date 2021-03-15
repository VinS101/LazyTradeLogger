using System;
using LTL.Business;

namespace LTL.UI.Console
{
    public class TradeDataDtoMapper
    {
        private readonly Func<string, OptionsTradingStrategy> _parseStrategy;

        public TradeDataDtoMapper(Func<string, OptionsTradingStrategy> parseStrategy)
        {
            _parseStrategy = parseStrategy;
        }

        public void Map(RequiredFields source, TradeDataDto target)
        {
            target.Ticker = source.Ticker;
            target.Strategy = _parseStrategy(source.Strategy);
            target.OpenDate = source.OpenDate;
            target.ExpiryDate = source.ExpiryDate;
            target.Quantity = source.Quantity;
            target.Delta = source.Delta;
            target.Price = source.Price;
            target.Underlying = source.Underlying; // TODO: Rename to UnderlyingPrice
            target.ShortPutStrike = source.ShortPutStrike;
            target.LongPutStrike = source.LongPutStrike;
            target.ShortCallStrike = source.ShortCallStrike;
            target.LongCallStrike = source.LongCallStrike;
            target.TradeStatus = source.TradeStatus;
            target.CommentsAtOpen = source.CommentsAtOpen;
            target.CommentsAtClose = source.CommentsAtClose;
        }        
    }
}