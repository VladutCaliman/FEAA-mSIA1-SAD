using Code.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.Sqlite;

namespace Code.ViewModel
{
    internal sealed partial class RegionDistribViewModel : ObservableObject
    {
        [ObservableProperty] private SeriesCollection? _columnChartData;
        [ObservableProperty] private List<string>? _stateLabels;


        public RegionDistribViewModel()
        {
            PopulateColumnChart();
        }

        private void PopulateColumnChart()
        {
            string query = @"
                SELECT 
                    customer_state AS State, 
                    COUNT(customer_id) AS NoOfCustomers
                FROM customers
                GROUP BY customer_state
                ORDER BY NoOfCustomers DESC
                LIMIT 15;";

            List<string> state = new List<string>();
            ChartValues<int> noOfCustomers = new ChartValues<int>();

            using (var conn = new SqliteConnection(DatabaseConnectionString.ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            state.Add(reader["State"].ToString()!);
                            noOfCustomers.Add(Convert.ToInt32(reader["NoOfCustomers"]));
                        }
                    }
                }
            }

            StateLabels = state;
            ColumnChartData = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "No.:",
                    Values = noOfCustomers,
                    DataLabels = true
                }
            };
        }
    }
}
