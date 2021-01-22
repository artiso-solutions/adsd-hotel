using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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
            List<Expression<Func<GuestInformationRecord, bool>>> filters = new();
            if (!string.IsNullOrEmpty(message.FirstName))
            {
                Expression<Func<GuestInformationRecord, bool>> fEx = r => r.GuestInformation.FirstName.Contains(message.FirstName);
                filters.Add(fEx);
            }
            if (!string.IsNullOrEmpty(message.LastName))
            {
                Expression<Func<GuestInformationRecord, bool>> fEx = r => r.GuestInformation.LastName.Contains(message.LastName);
                filters.Add(fEx);
            }
            if (!string.IsNullOrEmpty(message.EMail))
            {
                Expression<Func<GuestInformationRecord, bool>> fEx = r => r.GuestInformation.EMail.Contains(message.EMail);
                filters.Add(fEx);
            }
            var ex = filters switch
            {
                { Count: 0 } => x => true,
                { Count: 1 } => filters.First(),
                _ => CombineAll(filters)
            };

            var result = await _dataStoreClient.GetAllAsync(ex);
            var ids = result?.Select(r => r.OrderId).ToList();
            await context.Reply(new OrderIdRespone(ids));
        }

        private Expression<Func<GuestInformationRecord, bool>> CombineAll(List<Expression<Func<GuestInformationRecord, bool>>> filters)
        {
            if (filters == null || filters.Count <= 1)
                throw new InvalidOperationException("At least two filters are needed to combine.");
            BinaryExpression binary = null!;
            for (var i = 0; i < filters.Count - 1; i++)
            {
                var next = filters[i + 1];
                if (binary == null)
                {
                    var current = filters[i];
                    binary = Expression.OrElse(current.Body, next.Body);
                }
                else
                {
                    binary = Expression.OrElse(binary, next.Body);

                }
            }
            var result = Expression.Lambda<Func<GuestInformationRecord, bool>>(binary, filters.First().Parameters);
            return result;
        }
    }
}
