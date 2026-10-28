using Code.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.Sqlite;

namespace Code.ViewModel
{
    internal sealed partial class PaymentMethodViewModel : ObservableObject
    {
        [ObservableProperty] private SeriesCollection? _pieChartData;

        public PaymentMethodViewModel()
        {
            PopulatePieChart();
        }

        private void PopulatePieChart()
        {
            string query = @"
                SELECT 
                    payment_type AS PaymentType, 
                    COUNT(order_id) AS UsageCount
                FROM order_payments
                GROUP BY payment_type
                ORDER BY UsageCount DESC;";

            SeriesCollection series = new SeriesCollection();

            using (var conn = new SqliteConnection(DatabaseConnectionString.ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string paymentType = reader["PaymentType"].ToString()!;
                            int usageCount = Convert.ToInt32(reader["UsageCount"]);

                            series.Add(new PieSeries
                            {
                                Title = paymentType.ToUpper(),
                                Values = new ChartValues<int> { usageCount },
                                DataLabels = true
                            });
                        }
                    }
                }
            }

            PieChartData = series;
        }
    }
}
