using CommunityToolkit.Mvvm.ComponentModel;

namespace Code.ViewModel.Utility
{
    public enum CustomerCriteriaEnum
    {
        None,
        MaxRevenue,
        MinRevenue,
        MaxOrders,
        MinOrders,
    }

    internal sealed partial class CustomerBoxViewModel : ObservableObject
    {
        [ObservableProperty] private CustomerCriteriaEnum _customerCriteria = CustomerCriteriaEnum.None;
        [ObservableProperty] private int _queryRange = 50;

        public bool IsCriteriaSelected => CustomerCriteria != CustomerCriteriaEnum.None;

        partial void OnCustomerCriteriaChanged(CustomerCriteriaEnum value)
        {
            OnPropertyChanged(nameof(IsCriteriaSelected));
        }
    }
}
