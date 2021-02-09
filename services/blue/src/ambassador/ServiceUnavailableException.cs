using System;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException()
        {
        }

        public ServiceUnavailableException(string message)
            : base(message)
        {
        }
    }
}
