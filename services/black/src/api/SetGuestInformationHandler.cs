using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Events;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NServiceBus;
using NServiceBus.Extensibility;

namespace artiso.AdsdHotel.Black.Api
{
    public class SetGuestInformationHandler : IHandleMessages<SetGuestInformation>
    {
        private readonly ILogger<SetGuestInformationHandler> logger;

        //private readonly IDataStoreClient dataStoreClient;

        //public SetGuestInformationHandler(IDataStoreClient dataStoreClient)
        //{
        //    this.dataStoreClient = dataStoreClient;
        //}

        public SetGuestInformationHandler(ILogger<SetGuestInformationHandler> logger)
        {
            this.logger = logger;
        }

        public async Task Handle(SetGuestInformation message, IMessageHandlerContext context)
        {
            //var db = mongoClient.GetDatabase("blackDatabase");
            //var collection = db.GetCollection<GuestInformationRecord>("GuestInformation");
            //var record = new GuestInformationRecord { OrderId = message.OrderId, GuestInformation = message.GuestInformation };
            //await collection.InsertOneAsync(record).ConfigureAwait(false);
            //var filter = Builders<GuestInformationRecord>.Filter.Eq(x => x.OrderId, message.OrderId);
            //var update = Builders<GuestInformationRecord>.Update.Set(x => x.GuestInformation, message.GuestInformation);
            //var options = new FindOneAndUpdateOptions<GuestInformationRecord>();
            //options.IsUpsert = true;
            //var proj = await collection.FindOneAndUpdateAsync(filter, update, options).ConfigureAwait(false);
            this.logger.LogInformation($"Handled command for order {message.OrderId}");
            await context.Publish(new GuestInformationSet(message.OrderId, message.GuestInformation)).ConfigureAwait(false);
        }
    }
}
