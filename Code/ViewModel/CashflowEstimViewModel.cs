using Code.Utility;
using Code.View.Utility;
using Code.ViewModel.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.Sqlite;
using System.Windows;

namespace Code.ViewModel
{
    internal sealed partial class CashflowEstimViewModel : ObservableObject
    {
        [ObservableProperty] private SeriesCollection? _lineChartData;
        [ObservableProperty] private List<string>? _monthLables;

        private int _predictionRange;

        public CashflowEstimViewModel()
        {
            CashflowBoxView optionsBox = new CashflowBoxView();
            if (optionsBox.ShowDialog() == true)
            {
                _predictionRange = (optionsBox.DataContext as CashflowBoxViewModel)!.PredictionRange;
            }
            else
            {
                MessageBox.Show("Couldn't get selection criteria.", "Error.", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                throw new Exception("Couldn't get selection criteria");
            }

            PopulateLineChart(_predictionRange);
        }

        private void PopulateLineChart(int PredictionRange)
        {
            string query = @"
                SELECT 
                    strftime('%Y-%m', o.order_purchase_timestamp) AS Month, 
                    SUM(p.payment_value) AS Cashflow
                FROM orders o
                JOIN order_payments p USING (order_id)
                WHERE o.order_status = 'delivered' 
                  AND o.order_purchase_timestamp IS NOT NULL
                GROUP BY Month
                ORDER BY Month ASC;";

            List<string> concreteMonths = new List<string>();
            ChartValues<double> concreteCashflow = new ChartValues<double>();

            using (var conn = new SqliteConnection(DatabaseConnectionString.ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            concreteMonths.Add(reader["Month"].ToString()!);
                            concreteCashflow.Add(Convert.ToDouble(reader["Cashflow"]));
                        }
                    }
                }
            }

            //todo: estimation

            MonthLables = concreteMonths;
            LineChartData = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Incasari Istorice",
                    Values = concreteCashflow,
                    PointGeometrySize = 10
                }
            };
        }
    }
}
