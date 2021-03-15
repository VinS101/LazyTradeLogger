using System;
using CommandLine;
using LTL.Business;

namespace LTL.UI.Console
{
    public class RequiredFields
    {
        [Option('t', "ticker", Required = true, HelpText = "The stock ticker (Required)")]
        public string Ticker { get; set; }

        [Option('s', "strategy", Required = true, HelpText = "The strategy (Required)")]
        public string Strategy { get; set; }

        [Option('o', "opendate", Required = false, HelpText = "The date when the position was opened")]
        public DateTime? OpenDate { get; set; }

        [Option('e', "expiration", Required = true, HelpText = "The date when this position will expire (Required)")]
        public DateTime ExpiryDate { get; set; }

        [Option('q', "quantity", Required = false, HelpText = "The quantity of the position (position size)")]
        public int Quantity { get; set; }

        [Option('d', "delta", Required = true, HelpText = "The delta or net delta of the position. (Required)")]
        public decimal Delta { get; set; }

        [Option("price", Required = true, HelpText = "Price of the options contract (Required)")]
        public decimal Price { get; set; }

        [Option("underlying", Required = true, HelpText = "Price of the underlying (Required)")]
        public decimal Underlying { get; set; }

        [Option("shortputstrike", Required = false, HelpText = "The strike Price of the short put")]
        public decimal? ShortPutStrike { get; set; }

        [Option("longputstrike", Required = false, HelpText = "The strike Price of the long put")]
        public decimal? LongPutStrike { get; set; }

        [Option("shortcallstrike", Required = false, HelpText = "The strike Price of the short call")]
        public decimal? ShortCallStrike { get; set; }

        [Option("longcallstrike", Required = false, HelpText = "The strike Price of the long call")]
        public decimal? LongCallStrike { get; set; }

        [Option("status", Required = false, HelpText = "The current trade status")]
        public TradeStatus TradeStatus { get; set; }

        [Option("commentopen", Required = false, HelpText = "Comments for opening the trade")]
        public string CommentsAtOpen { get; set; }
        
        [Option("commentclose", Required = false, HelpText = "Comments at the closing of the trade")]
        public string CommentsAtClose { get; set; }
    }
}
