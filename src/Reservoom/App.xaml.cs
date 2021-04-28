using Reservoom.Exceptions;
using Reservoom.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Reservoom
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Hotel hotel = new Hotel("SingletonSean Suites");

            try
            {
                hotel.MakeReservation(new Reservation(
                    new RoomID(1, 3),
                    "SingletonSean",
                    new DateTime(1995, 1, 2),
                    new DateTime(1999, 1, 3)));
                hotel.MakeReservation(new Reservation(
                    new RoomID(1, 3),
                    "SingletonSean",
                    new DateTime(2000, 1, 1),
                    new DateTime(2000, 1, 2)));
            }
            catch (ReservationConflictException ex)
            {

            }
            
            IEnumerable<Reservation> reservations = hotel.GetAllReservations();

            base.OnStartup(e);
        }
    }
}
