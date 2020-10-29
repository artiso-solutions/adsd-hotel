using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using MongoDB.Driver;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api
{
    public class RequestGuestInformationHandler : IHandleMessages<RequestGuestInformation>
    {
        //private readonly MongoClient mongoClient;

        //public RequestGuestInformationHandler(MongoClient mongoClient)
        //{
        //    this.mongoClient = mongoClient;
        //}

        public async Task Handle(RequestGuestInformation message, IMessageHandlerContext context)
        {
            //var db = mongoClient.GetDatabase("blackDatabase");
            //var collection = db.GetCollection<GuestInformationRecord>("GuestInformation");
            //var resultCollection = await collection.FindAsync<GuestInformationRecord>(x => x.OrderId == message.OrderId).ConfigureAwait(false);
            //var result = await resultCollection.SingleOrDefaultAsync().ConfigureAwait(false);
            await context.Reply(new GuestInformationResponse ()/*{GuestInformation = result.GuestInformation}*/);
        }
    }
}
