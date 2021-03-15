namespace LTL.Business
{
    /// <summary>
    /// Responsible for calculating computed fields
    /// </summary>
    public interface IFieldCalculator
    {
        TradeDataDto ComputeCalculatedFields (TradeDataDto fields);
    }
}
