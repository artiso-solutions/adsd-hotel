using System;

namespace artiso.AdsdHotel.Red.Contracts.Grpc
{
    public sealed partial class Date
    {
        public Date(int day, int month, int year)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        public Date(DateTime date)
        {
            Day = date.Day;
            Month = date.Month;
            Year = date.Year;
        }

        public DateTime ToDateTime() => new(Year, Month, Day);
    }
}
