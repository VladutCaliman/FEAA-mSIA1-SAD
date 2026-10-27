using Code.Model.Utility;
using Code.View.Utility;
using Code.ViewModel.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using System.Data;

namespace Code.ViewModel
{
    internal sealed partial class CustomerStatsViewModel : ObservableObject
    {
        [ObservableProperty] private DataView _queryData;
        [ObservableProperty] private SeriesCollection _pieChartData;
        [ObservableProperty] private DataRowView _selectedData;

        private CustomerBoxResult _selectedCriteria;
        private double _totalRevenue;

        public CustomerStatsViewModel()
        {
            CustomerBoxView optionsBox = new CustomerBoxView();
            if (optionsBox.ShowDialog() == true)
            {
                _selectedCriteria = new(
                    CustomerCriteria: (optionsBox.DataContext as CustomerBoxViewModel)!.CustomerCriteria,
                    QueryRange: (optionsBox.DataContext as CustomerBoxViewModel)!.QueryRange);
            }
            else throw new Exception("[UNREACHABLE] Cannot get data from user.");

            HandleChoice(_selectedCriteria);
        }

        private void HandleChoice(CustomerBoxResult boxResult)
        {
            switch (boxResult.CustomerCriteria)
            {
                case CustomerCriteriaEnum.MinRevenue:
                    break;
                case CustomerCriteriaEnum.MaxRevenue:
                    break;
                case CustomerCriteriaEnum.None:
                default:
                    //User closed the box wihot choosing an option (the X button)
                    return;
            }
        }

        partial void OnSelectedDataChanged(DataRowView value)
        {
            throw new NotImplementedException();
        }
    }
}
