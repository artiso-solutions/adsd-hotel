using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace artiso.AdsdHotel.Yellow.Api
{
    public static class HandlerHelper
    {
        /// <summary>
        /// it throws a <see cref="System.ComponentModel.DataAnnotations.ValidationException"/> if the given is null 
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException"></exception>
        public static T Ensure<T>(T item)
        {
            var result = item;
            if (result is null)
                throw new ValidationException($"{typeof(T).Name} should not be null");

            return result;
        }
    }
}
