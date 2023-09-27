using System.ComponentModel;

namespace Reservoom.ViewModels
{
    public interface IPageViewModel : INotifyPropertyChanged
    {
        bool IsActive { get; set; }
    }
}
