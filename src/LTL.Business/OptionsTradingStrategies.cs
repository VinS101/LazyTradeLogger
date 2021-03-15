using System.ComponentModel;

namespace LTL.Business
{
    public enum OptionsTradingStrategy
    {
        /// <summary>
        /// Short put
        /// </summary>
        [Description("short put")]
        SP,

        /// <summary>
        /// Short call
        /// </summary>
        [Description("short call")]
        SC,

        /// <summary>
        /// Long call
        /// </summary>
        [Description("long call")]
        LC,

        /// <summary>
        /// Long put
        /// </summary>
        [Description("long put")]
        LP,

        /// <summary>
        /// Put credit spread
        /// </summary>
        [Description("put credit spread")]
        PCS,

        /// <summary>
        /// Call credit spread
        /// </summary>
        [Description("call credit spread")]
        CCS,

        /// <summary>
        /// Put debit spread
        /// </summary>
        [Description("put debit spread")]
        PDS,

        /// <summary>
        /// Call debit spread
        /// </summary>
        [Description("call debit spread")]
        CDS,

        /// <summary>
        /// Iron condor
        /// </summary>
        [Description("iron condor")]
        IC,

        /// <summary>
        /// Long Strangle
        /// </summary>
        [Description("long strangle")]
        LS,

        /// <summary>
        /// Short strangle
        /// </summary>
        [Description("short strangle")]
        SS
    }
}
