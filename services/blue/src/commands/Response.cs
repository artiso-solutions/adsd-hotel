using System;
using System.Diagnostics.CodeAnalysis;

namespace artiso.AdsdHotel.Blue.Commands
{
    public record Response<TValue>
    {
        public bool IsSuccessful => Exception is null;

        [MaybeNull]
        [AllowNull]
        public TValue Value { get; }

        public Exception? Exception { get; }

        public Response([AllowNull] TValue value) =>
            Value = value;

        public Response(Exception exception) =>
            Exception = exception;
    }
}
