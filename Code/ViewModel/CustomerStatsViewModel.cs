using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Data;

namespace Code.ViewModel
{
    internal sealed partial class CustomerStatsViewModel : ObservableObject
    {
        [ObservableProperty] private DataView _queryData;
        [ObservableProperty] private SeriesCollection _pieChartData;
        [ObservableProperty] private DataRowView _selectedData;

        public CustomerStatsViewModel()
        {
            var table = new DataTable();

            table.Columns.Add("CustomerId", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Orders", typeof(int));
            table.Columns.Add("TotalSpent", typeof(decimal));

            table.Rows.Add(1, "John Smith", "john@example.com", 12, 1540.50m);
            table.Rows.Add(2, "Jane Doe", "jane@example.com", 8, 920.75m);
            table.Rows.Add(3, "Michael Brown", "michael@example.com", 24, 3870.00m);
            table.Rows.Add(4, "Emily Wilson", "emily@example.com", 5, 410.25m);
            table.Rows.Add(5, "David Taylor", "david@example.com", 17, 2150.80m);

            _queryData = table.DefaultView;

            _pieChartData = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "John Smith",
                    Values = new ChartValues<int> { 12 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Jane Doe",
                    Values = new ChartValues<int> { 8 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Michael Brown",
                    Values = new ChartValues<int> { 24 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Emily Wilson",
                    Values = new ChartValues<int> { 5 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "David Taylor",
                    Values = new ChartValues<int> { 17 },
                    DataLabels = true
                }
            };
        }


        partial void OnSelectedDataChanged(DataRowView value)
        {
            throw new NotImplementedException();
        }
    }
}
