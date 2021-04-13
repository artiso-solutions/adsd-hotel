using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Contracts;
using Grpc.Net.Client;

namespace artiso.AdsdHotel.Red.Ambassador
{
    public class RedAmbassador
    {
        private readonly Contracts.Grpc.Rates.RatesClient _ratesClient;

        internal RedAmbassador(string uri)
        {
            var channel = GrpcChannel.ForAddress(uri);
            _ratesClient = new Contracts.Grpc.Rates.RatesClient(channel);
        }

        public async Task<IReadOnlyList<RoomRate>> GetRoomRatesByRoomTypeAsync(
            string roomType,
            DateTime startDate,
            DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(roomType))
                throw new ArgumentNullException(nameof(roomType));

            var request = new Contracts.Grpc.GetRoomRatesByRoomTypeRequest { RoomType = roomType, StartDate = new(startDate), EndDate = new(endDate) };
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

        //public async Task<InputRoomRatesReply> InputRoomRatesAsync(InputRoomRatesRequest inputRoomRatesRequest)
        //{
        //    return await _ratesClient.InputRoomRatesAsync(inputRoomRatesRequest);
        //}
    }
}
