using System;

namespace LTL.Common
{
    public class DateTimeProvider : ITimeProvider
    {

        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public string ToShortDateString()
        {
            return Now.ToShortDateString();
        }
    }
}