using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Contracts;
using artiso.AdsdHotel.Red.Contracts.Grpc;
using Grpc.Net.Client;
using CancellationFee = artiso.AdsdHotel.Red.Contracts.CancellationFee;
using RoomRate = artiso.AdsdHotel.Red.Contracts.RoomRate;

namespace artiso.AdsdHotel.Red.Ambassador
{
    public class RedAmbassador
    {
        private readonly Rates.RatesClient _ratesClient;

        internal RedAmbassador(string uri)
        {
            var channel = GrpcChannel.ForAddress(uri);
            _ratesClient = new Rates.RatesClient(channel);
        }

        public async Task<IReadOnlyList<RoomRate>> GetRoomRatesByRoomTypeAsync(
            string roomType,
            DateTime startDate,
            DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(roomType))
                throw new ArgumentNullException(nameof(roomType));

            var request = new GetRoomRatesByRoomTypeRequest { RoomType = roomType, StartDate = new(startDate), EndDate = new(endDate) };
            var response = await _ratesClient.GetRoomRatesByRoomTypeAsync(request);

            var roomRates = response.RoomRates.Select(rate => new RoomRate
            {
                Id = rate.Id,
                RateItems = rate.RateItems.Select(item => new RateItem { Id = item.Id, Price = item.Price, }).ToArray(),
                TotalPrice = rate.TotalPrice,
                CancellationFee = new CancellationFee
                {
                    DeadLine = rate.CancellationFee.DeadLine.ToDateTime(),
                    FeeInPercentage = rate.CancellationFee.FeeInPercentage,
                }
            }).ToArray();

            return roomRates;
        }

        public async Task<bool> InputRoomRatesAsync(string roomRateId, string orderId, DateTime startDate, DateTime endDate)
        {
            var request = new InputRoomRatesRequest
                {StartDate = new Date(startDate), EndDate = new Date(endDate), OrderId = orderId, RoomRateId = roomRateId};

            var reply = await _ratesClient.InputRoomRatesAsync(request);
            return reply.Success;
        }
    }
}
