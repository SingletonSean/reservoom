using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Reservoom.Exceptions;
using Reservoom.Models;
using Reservoom.Services;
using Reservoom.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reservoom.ViewModels
{
    [ObservableRecipient]
    public partial class MakeReservationViewModel : ObservableValidator, IPageViewModel
    {
        private readonly HotelStore _hotelStore;
        private readonly NavigationService<ReservationListingViewModel> _reservationViewNavigationService;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Username cannot be empty.")]
        [NotifyPropertyChangedFor(nameof(CanCreateReservation))]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string _username;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, double.MaxValue, ErrorMessage = "Floor number must be greater than zero.")]
        [NotifyPropertyChangedFor(nameof(CanCreateReservation))]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private int _floorNumber = 1;
        
        [ObservableProperty]
        private int _roomNumber;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(MakeReservationViewModel), nameof(ValidateStartDateBeforeEndDate), ErrorMessage = "The start date cannot be after the end date.")]
        [NotifyPropertyChangedFor(nameof(CanCreateReservation))]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private DateTime _startDate = new DateTime(2023, 9, 1);
        partial void OnStartDateChanged(DateTime value)
        {
            ValidateProperty(EndDate, nameof(EndDate));
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(MakeReservationViewModel), nameof(ValidateStartDateBeforeEndDate), ErrorMessage = "The end date cannot be before the start date.")]
        [NotifyPropertyChangedFor(nameof(CanCreateReservation))]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private DateTime _endDate = new DateTime(2023, 9, 3);
        partial void OnEndDateChanged(DateTime value)
        {
            ValidateProperty(StartDate, nameof(StartDate));
        }

        public static ValidationResult ValidateStartDateBeforeEndDate(string name, ValidationContext context)
        {
            MakeReservationViewModel viewModel = (MakeReservationViewModel)context.ObjectInstance;

            if (viewModel.StartDate < viewModel.EndDate)
            {
                return ValidationResult.Success;
            }

            return new("Start date is not before end date.");
        }

        public bool CanCreateReservation =>
            HasUsername &&
            HasFloorNumberGreaterThanZero &&
            HasStartDateBeforeEndDate &&
            !HasErrors;

        private bool HasUsername => !string.IsNullOrEmpty(Username);
        private bool HasFloorNumberGreaterThanZero => FloorNumber > 0;
        private bool HasStartDateBeforeEndDate => StartDate < EndDate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSubmitErrorMessage))]
        private string _submitErrorMessage;

        public bool HasSubmitErrorMessage => !string.IsNullOrEmpty(SubmitErrorMessage);

        [ObservableProperty]
        private bool _isSubmitting;

        public MakeReservationViewModel(HotelStore hotelStore, NavigationService<ReservationListingViewModel> reservationViewNavigationService)
        {
            _hotelStore = hotelStore;
            _reservationViewNavigationService = reservationViewNavigationService;

            Messenger = StrongReferenceMessenger.Default;
        }

        [RelayCommand(CanExecute = nameof(CanCreateReservation))]
        private async Task Submit()
        {
            SubmitErrorMessage = string.Empty;
            IsSubmitting = true;

            Reservation reservation = new Reservation(
                new RoomID(FloorNumber, RoomNumber),
                Username,
                StartDate,
                EndDate);

            try
            {
                await _hotelStore.MakeReservation(reservation);

                _reservationViewNavigationService.Navigate();
            }
            catch (ReservationConflictException)
            {
                SubmitErrorMessage = "This room is already taken on those dates.";
            }
            catch (InvalidReservationTimeRangeException)
            {
                SubmitErrorMessage = "Start date must be before end date.";
            }
            catch (Exception)
            {
                SubmitErrorMessage = "Failed to make reservation.";
            }

            IsSubmitting = false;
        }

        [RelayCommand]
        private void Cancel()
        {
            _reservationViewNavigationService.Navigate();
        }
    }
}
