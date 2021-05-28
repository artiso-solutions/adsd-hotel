using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace artiso.AdsdHotel.ITOps.Communication
{
    public class RabbitMqReadinessProbe
    {
        private readonly RabbitMqConfig? _rabbitMqConfig;
        private readonly IOptions<RabbitMqConfig>? _rabbitMqConfigOptions;

        public RabbitMqReadinessProbe(RabbitMqConfig rabbitMqConfig)
        {
            _rabbitMqConfig = rabbitMqConfig;
        }

        public RabbitMqReadinessProbe(IOptions<RabbitMqConfig> rabbitMqConfigOptions)
        {
            _rabbitMqConfigOptions = rabbitMqConfigOptions;
        }

        public async Task<bool> IsServiceAliveAsync(CancellationToken cancellationToken = default)
        {
            do
            {
                try
                {
                    var config = GetConfig();
                    using var tcpClientB = new TcpClient();
                    await tcpClientB.ConnectAsync(config.Host, 5672, cancellationToken).ConfigureAwait(false);
                    return true;
                }
                catch
                {
                    if (!cancellationToken.IsCancellationRequested)
                        await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                }
            }
            while (!cancellationToken.IsCancellationRequested);

            return false;
        }

        private RabbitMqConfig GetConfig() => _rabbitMqConfig ?? _rabbitMqConfigOptions!.Value;
    }
}
