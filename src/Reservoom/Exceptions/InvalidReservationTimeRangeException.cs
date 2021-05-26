using Reservoom.Models;
using System;

namespace Reservoom.Exceptions
{
    public class InvalidReservationTimeRangeException : Exception
    {
        public Reservation Reservation { get; }

        public InvalidReservationTimeRangeException(Reservation reservation)
        {
            Reservation = reservation;
        }
    }
}
