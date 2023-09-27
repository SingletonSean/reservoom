using CommunityToolkit.Mvvm.Messaging.Messages;
using Reservoom.Models;

namespace Reservoom.Stores
{
    public class ReservationMadeMessage : ValueChangedMessage<Reservation>
    {
        public ReservationMadeMessage(Reservation value) : base(value) { }
    }
}
