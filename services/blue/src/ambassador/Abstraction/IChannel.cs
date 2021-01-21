using System;
using System.Threading;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public interface IChannel : IAsyncDisposable
    {
        Task Send(object command);

        Task<TResponse> Request<TResponse>(object request, CancellationToken cancellationToken = default);

        Task Publish(object evt);
    }
}
