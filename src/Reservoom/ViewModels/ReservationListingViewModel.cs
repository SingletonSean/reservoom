using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Reservoom.Models;
using Reservoom.Services;
using Reservoom.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reservoom.ViewModels
{
    public partial class ReservationListingViewModel : ObservableRecipient, IRecipient<ReservationMadeMessage>, IPageViewModel
    {
        private readonly HotelStore _hotelStore;
        private readonly NavigationService<MakeReservationViewModel> _makeReservationNavigationService;
        private readonly ObservableCollection<ReservationViewModel> _reservations;

        public IEnumerable<ReservationViewModel> Reservations => _reservations;

        public bool HasReservations => _reservations.Any();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        private string _errorMessage;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        [ObservableProperty]
        private bool _isLoading;

        [RelayCommand]
        private void MakeReservation()
        {
            _makeReservationNavigationService.Navigate();
        }

        [RelayCommand]
        private async Task LoadReservations()
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            try
            {
                await _hotelStore.Load();

                UpdateReservations(_hotelStore.Reservations);
            }
            catch (Exception)
            {
                ErrorMessage = "Failed to load reservations.";
            }

            IsLoading = false;
        }

        public ReservationListingViewModel(HotelStore hotelStore, NavigationService<MakeReservationViewModel> makeReservationNavigationService)
        {
            _hotelStore = hotelStore;
            _makeReservationNavigationService = makeReservationNavigationService;
            _reservations = new ObservableCollection<ReservationViewModel>();

            _reservations.CollectionChanged += OnReservationsChanged;
        }

        protected override void OnActivated()
        {
            StrongReferenceMessenger.Default.RegisterAll(this);

            base.OnActivated();
        }

        protected override void OnDeactivated()
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);

            base.OnDeactivated();
        }

        public void Receive(ReservationMadeMessage message)
        {
            ReservationViewModel reservationViewModel = new ReservationViewModel(message.Value);
            _reservations.Add(reservationViewModel);
        }

        public static ReservationListingViewModel LoadViewModel(HotelStore hotelStore, NavigationService<MakeReservationViewModel> makeReservationNavigationService)
        {
            ReservationListingViewModel viewModel = new ReservationListingViewModel(hotelStore, makeReservationNavigationService);

            viewModel.LoadReservationsCommand.Execute(null);

            return viewModel;
        }

        public void UpdateReservations(IEnumerable<Reservation> reservations)
        {
            _reservations.Clear();

            foreach (Reservation reservation in reservations)
            {
                ReservationViewModel reservationViewModel = new ReservationViewModel(reservation);
                _reservations.Add(reservationViewModel);
            }
        }

        private void OnReservationsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasReservations));
        }
    }
}
