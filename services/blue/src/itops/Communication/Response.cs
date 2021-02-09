using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace artiso.AdsdHotel.ITOps.Communication
{
    public class Response<TValue>
    {
        public bool IsSuccessful => Exception is null;

        [MaybeNull]
        public TValue Value { get; }

        public Exception? Exception { get; }

        public Response(TValue value) =>
            Value = value;

        public Response(Exception exception) =>
            Exception = exception;

        [JsonConstructor]
        internal Response(TValue value, Exception exception) =>
            (Value, Exception) = (value, exception);

        public void Deconstruct(
            [MaybeNull] out TValue value,
            out Exception? exception)
        {
            value = Value;
            exception = Exception;
        }
    }
}
