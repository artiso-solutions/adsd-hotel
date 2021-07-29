using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Events;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Purple.Commands;
using artiso.AdsdHotel.Purple.Events;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Events;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace artiso.AdsdHotel.Purple.Api.Sagas
{
    internal class ReservationProcessSaga : Saga<ReservationProcessSagaData>,
        IAmStartedByMessages<CompleteReservation>,
        IHandleMessages<OrderCancellationFeeAuthorizationAcquired>,
        IHandleMessages<SelectedRoomTypeConfirmationFailed>,
        IHandleMessages<SelectedRoomTypeReserved>,
        IHandleMessages<OrderCancellationFeeCharged>,
        IHandleMessages<Response<bool>>
    {
        public const string BlueService = "Blue.Api";
        public const string YellowService = "Yellow.Api";
        private readonly ILogger<ReservationProcessSaga> _logger;

        public ReservationProcessSaga(ILogger<ReservationProcessSaga> _logger)
        {
            this._logger = _logger;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ReservationProcessSagaData> mapper)
        {
            mapper.ConfigureMapping<CompleteReservation>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);

            mapper.ConfigureMapping<OrderCancellationFeeAuthorizationAcquired>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);

            mapper.ConfigureMapping<SelectedRoomTypeConfirmationFailed>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);

            mapper.ConfigureMapping<SelectedRoomTypeReserved>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);

            mapper.ConfigureMapping<OrderCancellationFeeCharged>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);
        }

        public async Task Handle(CompleteReservation message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Handled '{nameof (CompleteReservation)}'.");
            await context.Send(destination: YellowService, new AuthorizeOrderCancellationFeeRequest(Data.OrderId!));
        }

        public async Task Handle(OrderCancellationFeeAuthorizationAcquired message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Handled '{nameof (OrderCancellationFeeAuthorizationAcquired)}'.");
            Data.CancellationFeeAcquired = true;

            // this is problematic, because the handler is replying and we have no handler here which consumes the reply
            await context.Send(destination: BlueService, new ConfirmSelectedRoomType(Data.OrderId!));
        }

        // Handle failure of Blue service
        public Task Handle(SelectedRoomTypeConfirmationFailed message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Handled '{nameof (SelectedRoomTypeConfirmationFailed)}'.");
            // await context.Send(destination: YellowService, new CancelCancellationFeeAuthorization(Data.OrderId!));
            return Task.CompletedTask;
        }

        public async Task Handle(SelectedRoomTypeReserved message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Handled '{nameof (SelectedRoomTypeReserved)}'.");
            Data.RoomTypeConfirmed = true;

            await context.Send(destination: YellowService, new ChargeForOrderCancellationFeeRequest(Data.OrderId!));
        }

        public async Task Handle(OrderCancellationFeeCharged message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Handled '{nameof (OrderCancellationFeeCharged)}'.");
            Data.CancellationFeeCharged = true;

            await context.Publish(new ReservationCompleted(Data.OrderId!));

            MarkAsComplete();
        }

        public Task Handle(Response<bool> message, IMessageHandlerContext context)
        {
            // ToDo this is not nice. Maybe we can remove this when we do a publish instead of a send.
            // If not, then we should remove the replies from the handlers.
            return Task.CompletedTask;
        }
    }
}
