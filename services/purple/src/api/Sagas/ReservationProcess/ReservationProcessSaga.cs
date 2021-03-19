﻿using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Events;
using artiso.AdsdHotel.Purple.Commands;
using artiso.AdsdHotel.Purple.Events;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Events;
using NServiceBus;

namespace artiso.AdsdHotel.Purple.Api.Sagas
{
    internal class ReservationProcessSaga : Saga<ReservationProcessSagaData>,
        IAmStartedByMessages<CompleteReservation>,
        IHandleMessages<OrderCancellationFeeAuthorizationAcquired>,
        IHandleMessages<SelectedRoomTypeConfirmationFailed>,
        IHandleMessages<SelectedRooomTypeReserved>,
        IHandleMessages<OrderCancellationFeeCharged>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ReservationProcessSagaData> mapper)
        {
            mapper.ConfigureMapping<CompleteReservation>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);
        }

        public async Task Handle(CompleteReservation message, IMessageHandlerContext context)
        {
            await context.Publish(new AuthorizeOrderCancellationFeeRequest(Data.OrderId!));
        }

        public async Task Handle(OrderCancellationFeeAuthorizationAcquired message, IMessageHandlerContext context)
        {
            Data.CancellationFeeAcquired = true;

            await context.Publish(new ConfirmSelectedRoomType(Data.OrderId!));
        }

        public Task Handle(SelectedRoomTypeConfirmationFailed message, IMessageHandlerContext context)
        {
            // await context.Publish(new CancelCancellationFeeAuthorization(Data.OrderId!));
            return Task.CompletedTask;
        }

        public async Task Handle(SelectedRooomTypeReserved message, IMessageHandlerContext context)
        {
            Data.RoomTypeConfirmed = true;

            await context.Publish(new ChargeForOrderCancellationFeeRequest(Data.OrderId!));
        }

        public async Task Handle(OrderCancellationFeeCharged message, IMessageHandlerContext context)
        {
            Data.CancellationFeeCharged = true;

            await context.Publish(new ReservationCompleted(Data.OrderId!));

            MarkAsComplete();
        }
    }
}
