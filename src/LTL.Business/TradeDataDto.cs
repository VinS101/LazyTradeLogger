using System;

namespace LTL.Business
{
    public class TradeDataDto
    {
        public string CommentsAtClose { get; set; }
        public string Ticker { get; set; }
        public DateTime? OpenDate { get; set; }
        public OptionsTradingStrategy Strategy { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal Delta { get; set; }
        public decimal Price { get; set; }
        public decimal Underlying { get; set; }
        public decimal? ShortPutStrike { get; set; }
        public decimal? ShortCallStrike { get; set; }
        public decimal? LongCallStrike { get; set; }
        public decimal? LongPutStrike { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal MaxRisk { get; set; }
        public int DTE { get; set; }
        public decimal? RiskRewardRatio { get; set; }
        public int DaysInTrade { get; set; }
        public TradeStatus TradeStatus { get; set; }
        public string CommentsAtOpen { get; set; }
        public decimal? ProbablityOfProfit {get;set;}
        public decimal? MaxProfit { get; set; }
    }
}