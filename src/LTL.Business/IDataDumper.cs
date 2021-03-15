using System.Collections.Generic;
using LTL.Business;

namespace LTL
{
    /// <summary>
    /// Responsible for dumping data into a destination
    /// </summary>
    public interface IDataDumper
    {
        void Dump(TradeDataDto fields, Dictionary<string, object> parameters);
    }
}