using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Events;
using MongoDB.Bson;
using MongoDB.Driver;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api
{
    public class SetGuestInformationHandler : IHandleMessages<SetGuestInformation>
    {
        private readonly MongoClient mongoClient;

        public SetGuestInformationHandler(MongoClient mongoClient)
        {
            this.mongoClient = mongoClient;
        }

        public async Task Handle(SetGuestInformation message, IMessageHandlerContext context)
        {
            var db = mongoClient.GetDatabase("blackDatabase");
            var collection = db.GetCollection<GuestInformationRecord>("GuestInformation");
            var record = new GuestInformationRecord { OrderId = message.OrderId, GuestInformation = message.GuestInformation };
            await collection.InsertOneAsync(record).ConfigureAwait(false);
            await context.Publish(new GuestInformationSet(message.OrderId, message.GuestInformation)).ConfigureAwait(false);
        }
    }
}
