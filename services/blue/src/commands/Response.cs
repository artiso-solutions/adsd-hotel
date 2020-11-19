﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace artiso.AdsdHotel.Blue.Commands
{
    public class Response<TValue>
    {
        public bool IsSuccessful => Exception is null;

        [MaybeNull]
        [AllowNull]
        public TValue Value { get; }

        public Exception? Exception { get; }

        public Response(TValue value) =>
            Value = value;

        public Response(Exception exception) =>
            Exception = exception;

        public void Deconstruct(
            [MaybeNull] out TValue value,
            out Exception? exception)
        {
            value = Value;
            exception = Exception;
        }
    }
}