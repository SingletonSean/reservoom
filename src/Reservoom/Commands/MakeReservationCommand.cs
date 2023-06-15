using Reservoom.Exceptions;
using Reservoom.Models;
using Reservoom.Services;
using Reservoom.Stores;
using Reservoom.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reservoom.Commands
{
    public class MakeReservationCommand : AsyncCommandBase
    {
        private readonly MakeReservationViewModel _makeReservationViewModel;
        private readonly HotelStore _hotelStore;
        private readonly NavigationService<ReservationListingViewModel> _reservationViewNavigationService;

        public MakeReservationCommand(MakeReservationViewModel makeReservationViewModel, 
            HotelStore hotelStore,
            NavigationService<ReservationListingViewModel> reservationViewNavigationService)
        {
            _makeReservationViewModel = makeReservationViewModel;
            _hotelStore = hotelStore;
            _reservationViewNavigationService = reservationViewNavigationService;
            
            _makeReservationViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return _makeReservationViewModel.CanCreateReservation && base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _makeReservationViewModel.SubmitErrorMessage = string.Empty;
            _makeReservationViewModel.IsSubmitting = true;

            Reservation reservation = new Reservation(
                new RoomID(_makeReservationViewModel.FloorNumber, _makeReservationViewModel.RoomNumber),
                _makeReservationViewModel.Username,
                _makeReservationViewModel.StartDate,
                _makeReservationViewModel.EndDate);

            try
            {
                await _hotelStore.MakeReservation(reservation);
                
                // TODO: Abstract this for testing! 
                //MessageBox.Show("Successfully reserved room.", "Success",
                //    MessageBoxButton.OK, MessageBoxImage.Information);

                _reservationViewNavigationService.Navigate();
            }
            catch (ReservationConflictException)
            {
                _makeReservationViewModel.SubmitErrorMessage = "This room is already taken on those dates.";
            }
            catch (InvalidReservationTimeRangeException)
            {
                _makeReservationViewModel.SubmitErrorMessage = "Start date must be before end date.";
            }
            catch (Exception)
            {
                _makeReservationViewModel.SubmitErrorMessage = "Failed to make reservation.";
            }

            _makeReservationViewModel.IsSubmitting = false;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(MakeReservationViewModel.CanCreateReservation))
            {
                OnCanExecutedChanged();
            }
        }
    }
}
