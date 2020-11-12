using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Black.Api
{
    public class ProceedIfRabbitMqIsAlive : IHostedService
    {
        private readonly string _host;
        private readonly int _port;

        public ProceedIfRabbitMqIsAlive(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            do
            {
                try
                {
                    using var tcpClientB = new TcpClient();
                    await tcpClientB.ConnectAsync(_host, _port).ConfigureAwait(false);
                    return;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
