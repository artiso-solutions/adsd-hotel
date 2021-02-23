using System;

namespace artiso.AdsdHotel.Red.Api
{
    public static class Extensions
    {
        public static DateTime ToDateTime(this Date date)
        {
            return new(date.Year, date.Month, date.Day);
        }
    }
}
