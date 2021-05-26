using Reservoom.Exceptions;
using Reservoom.Services.ReservationConflictValidators;
using Reservoom.Services.ReservationCreators;
using Reservoom.Services.ReservationProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservoom.Models
{
    public class ReservationBook
    {
        private readonly IReservationProvider _reservationProvider;
        private readonly IReservationCreator _reservationCreator;
        private readonly IReservationConflictValidator _reservationConflictValidator;

        public ReservationBook(IReservationProvider reservationProvider, IReservationCreator reservationCreator, IReservationConflictValidator reservationConflictValidator)
        {
            _reservationProvider = reservationProvider;
            _reservationCreator = reservationCreator;
            _reservationConflictValidator = reservationConflictValidator;
        }

        /// <summary>
        /// Get all reservations.
        /// </summary>
        /// <returns>All reservations in the reservation book.</returns>
        public async Task<IEnumerable<Reservation>> GetAllReservations()
        {
            return await _reservationProvider.GetAllReservations();
        }

        /// <summary>
        /// Add a reservation to the reservation book.
        /// </summary>
        /// <param name="reservation">The incoming reservation.</param>
        /// <exception cref="InvalidReservationTimeRangeException">Thrown if reservation start time is after end time.</exception>
        /// <exception cref="ReservationConflictException">Thrown if incoming reservation conflicts with existing reservation.</exception>
        public async Task AddReservation(Reservation reservation)
        {
            if(reservation.StartTime > reservation.EndTime)
            {
                throw new InvalidReservationTimeRangeException(reservation);
            }

            Reservation conflictingReservation = await _reservationConflictValidator.GetConflictingReservation(reservation);

            if(conflictingReservation != null)
            {
                throw new ReservationConflictException(conflictingReservation, reservation);
            }

            await _reservationCreator.CreateReservation(reservation);
        }
    }
}
