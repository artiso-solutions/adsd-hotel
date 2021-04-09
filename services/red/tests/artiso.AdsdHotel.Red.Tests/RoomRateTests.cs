//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using artiso.AdsdHotel.Red.Persistence;
//using artiso.AdsdHotel.Red.Persistence.Entities;
//using artiso.AdsdHotel.Red.Api.Service;
//using artiso.AdsdHotel.Red.Contracts;
//using Grpc.Core;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace artiso.AdsdHotel.Red.Tests
//{
//    [TestClass]
//    public class RoomRateTests
//    {
//        [TestMethod]
//        public async Task TotalPriceSumTest()
//        {
//            var dbMock = new Mock<IRoomPriceRepository>();
//            dbMock.Setup(priceService => priceService.GetRoomRatesByRoomType(It.IsAny<string>())).Returns(() => Task.FromResult(new List<RateItem>(){
//                new RateItem(Guid.NewGuid(), 50),
//                new RateItem(Guid.NewGuid(), 15)
//            }));
//            var contextMock = new Mock<ServerCallContext>();
//            var service = new RatesService(dbMock.Object);
//            var getRoomRatesByRoomTypeReply = await service.GetRoomRatesByRoomType(new GetRoomRatesByRoomTypeRequest() { RoomType = "test" }, contextMock.Object);
//            Assert.AreEqual(65, getRoomRatesByRoomTypeReply.TotalPrice);
//        }

//        [TestMethod]
//        public void EmptyTypeTest()
//        {
//            var dbMock = new Mock<IRoomPriceRepository>();
//            var contextMock = new Mock<ServerCallContext>();
//            var service = new RatesService(dbMock.Object);
//            Assert.ThrowsExceptionAsync<ArgumentException>(() => service.GetRoomRatesByRoomType(new GetRoomRatesByRoomTypeRequest() {RoomType = ""},
//                contextMock.Object));
//        }
//    }
//}
