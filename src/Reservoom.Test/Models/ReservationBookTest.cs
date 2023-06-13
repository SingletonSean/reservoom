using Moq;
using NUnit.Framework;
using Reservoom.Exceptions;
using Reservoom.Models;
using Reservoom.Services.ReservationConflictValidators;
using Reservoom.Services.ReservationCreators;
using Reservoom.Services.ReservationProviders;

namespace Reservoom.Test.Models
{
    public class ReservationBookTest
    {
        private ReservationBook _reservationBook;

        private Mock<IReservationConflictValidator> _mockReservationConflictValidator;

        [SetUp]
        public void SetUp()
        {
            _mockReservationConflictValidator = new Mock<IReservationConflictValidator>();

            _reservationBook = new ReservationBook(
                new Mock<IReservationProvider>().Object,
                new Mock<IReservationCreator>().Object,
                _mockReservationConflictValidator.Object
            );
        }

        [Test]
        public void AddReservation_WithConflictingReservation_ThrowsReservationConflictException()
        {
            // arrange & mock
            Reservation reservation = new Reservation(
                new RoomID(1, 1), 
                "New", 
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 2));
            _mockReservationConflictValidator
                .Setup((t) => t.GetConflictingReservation(reservation))
                .ReturnsAsync(new Reservation(
                    new RoomID(1, 1),
                    "Other",
                    new DateTime(2000, 1, 1),
                    new DateTime(2000, 1, 2))
                );

            // act & assert
            Assert.ThrowsAsync<ReservationConflictException>(() => _reservationBook.AddReservation(reservation));
        }
    }
}
