using CommunityToolkit.Mvvm.ComponentModel;
using Reservoom.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservoom.Stores
{
    public class NavigationStore
    {
        private IPageViewModel _currentViewModel;
        public IPageViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != null)
                {
                    _currentViewModel.IsActive = false;
                }

                _currentViewModel = value;

                if (_currentViewModel != null)
                {
                    _currentViewModel.IsActive = true;
                }

                OnCurrentViewModelChanged();
            }
        }

        public event Action CurrentViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
