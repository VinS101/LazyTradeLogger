using System;

namespace LTL.Common
{
    /// <summary>
    /// Responsible for providing time.
    /// </summary>
    public interface ITimeProvider
    {
        DateTime Now { get; }
        
        string ToShortDateString();
    }
}