using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Api.Handlers;
using artiso.AdsdHotel.Red.Persistence;
using artiso.AdsdHotel.Red.Persistence.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NServiceBus.Testing;

namespace artiso.AdsdHotel.Red.Tests
{
    [TestClass]
    public class RoomSelectedHandlerTests
    {
        //[TestMethod]
        //public async Task Test1()
        //{
        //    var busChannelMock = new Mock<IChannel>();
        //    var repMock = new Mock<IRoomRepository>();

        //    var rate = new RateItemEntity(Guid.NewGuid(), 10);
        //    var rates = new List<RateItemEntity> { rate };
        //    var details = new ConfirmationDetailsEntity() {CancellationFeeEntity = new CancellationFeeEntity { FeeInPercentage = 10f} };
        //    Persistence.Entities.RoomTypeEntity rt = new Persistence.Entities.RoomTypeEntity (Guid.NewGuid(), "test", rates, details );
        //    repMock.Setup<Task<Persistence.Entities.RoomTypeEntity>>(r => r.GetRoomTypeById(It.IsAny<string>()))
        //        .Returns(Task.FromResult(rt));


        //    var messageSession = new TestableMessageSession();
        //    var channel = new TestChannel(messageSession);
        //    var testee = new RoomSelectedHandler(busChannelMock.Object, repMock.Object, channel);
        //    await testee.Handle(new Contracts.Grpc.InputRoomRatesRequest { OrderId = "1", StartDate = new Contracts.Grpc.Date( DateTime.Today), EndDate = new Contracts.Grpc.Date(DateTime.Today.AddDays(1)), RoomRateId = "rate2" });
        //    Assert.AreEqual(1, messageSession.PublishedMessages.Length);
        //}
    }
}
