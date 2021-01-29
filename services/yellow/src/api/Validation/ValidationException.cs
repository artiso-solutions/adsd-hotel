using System;

namespace artiso.AdsdHotel.Yellow.Api.Validation
{
    internal class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
            //
        }
        
        public ValidationException(IValidateModelResult validateModelResult) : base(validateModelResult.GetErrors())
        {
            //
        }
    }
}
