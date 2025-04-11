using System;

namespace Charmaran.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);
        }
        
        public static DateTime ConvertToUtc(this DateTime date)
        {
            return date.ToUniversalTime();
        }
        
        public static DateTime ConvertToLocalTime(this DateTime date)
        {
            return date.ToLocalTime();
        }
    }
}