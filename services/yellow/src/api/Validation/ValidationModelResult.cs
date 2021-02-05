using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace artiso.AdsdHotel.Yellow.Api.Validation
{
    public class ValidationModelResult<T> : IValidateModelResult
    {
        private readonly List<string> _errors;
        
        public T Instance { get; }

        internal ValidationModelResult(T model)
        {
            _errors = new List<string>();
            Instance = model;
        }

        public bool IsValid()
        {
            return !_errors.Any();
        }

        internal ImmutableList<string> Errors => _errors.ToImmutableList();

        internal void SetError(string value)
        {
            _errors.Add(value);
        }

        public string GetErrors()
        {
            var strBuilder = new StringBuilder();
            
            foreach (var error in Errors)
                strBuilder.AppendLine($"{error}");

            return strBuilder.ToString().TrimEnd();
        }
    }
}
