using System.Diagnostics.CodeAnalysis;
using artiso.AdsdHotel.Yellow.Api.Validation;

namespace artiso.AdsdHotel.Yellow.Api
{
    public static class HandlerHelper
    {
        /// <summary>
        /// it throws a <see cref="artiso.AdsdHotel.Yellow.Api.Validation.ValidationException"/> if the given is null 
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="artiso.AdsdHotel.Yellow.Api.Validation.ValidationException"></exception>
        [return: NotNull]
        public static T Ensure<T>(T item)
        {
            var result = item;
            if (result is null)
                throw new ValidationException($"{typeof(T).Name} should not be null");

            return result;
        }
    }
}
