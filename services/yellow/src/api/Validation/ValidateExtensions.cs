using System;
using System.Linq;

namespace artiso.AdsdHotel.Yellow.Api.Validation
{
    public static class ValidateExtensions
    {
        public static ValidationModelResult<T> Validate<T>(this T model)
        {
            return new(model);
        }

        public static ValidationModelResult<T> That<T>(this ValidationModelResult<T> v, Func<T, bool> rule, string errorMessage = "")
        {
            if (v.Errors.Any())
                return v;

            if (!rule(v.Instance)) 
                v.SetError(errorMessage);

            return v;
        }
        
        public static ValidationModelResult<T> HasData<T>(this ValidationModelResult<T> v, Func<T, string> rule, string errorMessage)
        {
            var provValue = rule(v.Instance);

            return v.That(_ => !string.IsNullOrWhiteSpace(provValue));
        }
        
        public static ValidationModelResult<T> NotNull<T>(this ValidationModelResult<T> v, Func<T, object?> rule, string errorMessage)
        {
            var provValue = rule(v.Instance);

            return v.That(_ => provValue is not null);
        }
    }
}
