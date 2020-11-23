using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api.Handlers
{
    public class RequestOrderIdHandler : IHandleMessages<OrderIdRequest>
    {
        private readonly IDataStoreClient _dataStoreClient;
        private readonly ILogger<RequestOrderIdHandler> _logger;

        public RequestOrderIdHandler(IDataStoreClient dataStoreClient, ILogger<RequestOrderIdHandler> logger)
        {
            _dataStoreClient = dataStoreClient;
            _logger = logger;
        }

        public async Task Handle(OrderIdRequest message, IMessageHandlerContext context)
        {
            Expression<Func<GuestInformationRecord, bool>> ex = x => false
            || string.IsNullOrEmpty(message.FirstName) ? false : x.GuestInformation.FirstName.Contains(message.FirstName)
            || string.IsNullOrEmpty(message.LastName) ? false : x.GuestInformation.FirstName.Contains(message.LastName)
            || string.IsNullOrEmpty(message.EMail) ? false : x.GuestInformation.FirstName.Contains(message.EMail);
            
            var result = await _dataStoreClient.GetAllAsync(ex);
            var ids = result?.Select(r => r.OrderId).ToList();
            await context.Reply(new OrderIdRespone(ids));
        }
    }
}
