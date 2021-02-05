using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Api;
using artiso.AdsdHotel.Red.Data;
using artiso.AdsdHotel.Red.Data.Entities;
using artiso.AdsdHotel.Red.Service.Service;
using Grpc.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace artiso.AdsdHotel.Red.Tests
{
    [TestClass]
    public class RoomRateTests
    {
        [TestMethod]
        public async Task TotalPriceSumTest()
        {
            var dbMock = new Mock<IRoomPriceService>();
            dbMock.Setup(priceService => priceService.GetRoomRatesByRoomType(It.IsAny<string>())).Returns(() => Task.FromResult(new List<Rate>(){
                new Rate(Guid.NewGuid(), 50),
                new Rate(Guid.NewGuid(), 15)
            }));
            var contextMock = new Mock<ServerCallContext>();
            var service = new RatesService(dbMock.Object);
            var getRoomRatesByRoomTypeReply = await service.GetRoomRatesByRoomType(new GetRoomRatesByRoomTypeRequest() { RoomType = "test" }, contextMock.Object);
            Assert.AreEqual(65, getRoomRatesByRoomTypeReply.TotalPrice);
        }

        [TestMethod]
        public void EmptyTypeTest()
        {
            var dbMock = new Mock<IRoomPriceService>();
            var contextMock = new Mock<ServerCallContext>();
            var service = new RatesService(dbMock.Object);
            Assert.ThrowsExceptionAsync<ArgumentException>(() => service.GetRoomRatesByRoomType(new GetRoomRatesByRoomTypeRequest() {RoomType = ""},
                contextMock.Object));
        }
    }
}
