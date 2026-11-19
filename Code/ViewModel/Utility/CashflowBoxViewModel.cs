using CommunityToolkit.Mvvm.ComponentModel;

namespace Code.ViewModel.Utility
{
    internal sealed partial class CashflowBoxViewModel : ObservableObject
    {
        [ObservableProperty] private int _predictionRange = 3;
    }
}
