using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace artiso.AdsdHotel.ITOps.Validation
{
    public class MultiValidationException : ValidationException
    {
        public Dictionary<string, List<string>> Errors { get; } = new();

        public void Add(string topic, string description)
        {
            if (Errors.ContainsKey(topic))
            {
                Errors[topic].Add(description);
            }
            else
            {
                Errors.Add(topic, new() { description });
            }
        }
    }
}
