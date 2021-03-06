﻿using System;
using System.Threading.Tasks;
using NServiceBus;

namespace artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus
{
    public class EndpointHolder : IAsyncDisposable
    {
        public Task EndpointReady { get; }

        public IEndpointInstance Endpoint
        {
            get
            {
                if (!EndpointReady.IsCompletedSuccessfully)
                    throw new Exception("Endpoint not ready.");

                return _endpoint!;
            }
        }

        private IEndpointInstance? _endpoint;

        public EndpointHolder(Task<IEndpointInstance> endpointPromise)
        {
            var tcs = new TaskCompletionSource<bool>();

            endpointPromise.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    _endpoint = t.Result;
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetException(t.Exception ?? new Exception("Unable to create the endpoint"));
                }
            });

            EndpointReady = tcs.Task;
        }

        public async ValueTask DisposeAsync()
        {
            if (EndpointReady.IsCompletedSuccessfully)
                await Endpoint.Stop().ConfigureAwait(false);
        }
    }
}
