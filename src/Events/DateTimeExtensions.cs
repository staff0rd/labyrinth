using System;

namespace Events
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return dateTime.Ticks / 10000L - 62135596800000L;
        }
    }
}