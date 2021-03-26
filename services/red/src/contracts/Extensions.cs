using System;

namespace artiso.AdsdHotel.Red.Contracts
{
    public static class Extensions
    {
        public static DateTime ToDateTime(this Date date)
        {
            return new(date.Year, date.Month, date.Day);
        }
    }
}
