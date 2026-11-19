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

            double averageTrend = 0;
            int count = concreteCashflow.Count;

            if (count >= 3)
            {
                averageTrend = (concreteCashflow[count - 1] - concreteCashflow[count - 3]) / 2.0;
            }

            ChartValues<double> predictedCashflow = new ChartValues<double>();
            List<string> allMonths = new List<string>(concreteMonths);

            for (int i = 0; i < count - 1; i++)
            {
                predictedCashflow.Add(double.NaN);
            }

            predictedCashflow.Add(concreteCashflow.Last());

            DateTime lastMonth = DateTime.Parse(concreteMonths.Last() + "-01");
            double currentEstimation = concreteCashflow.Last();

            for (int i = 1; i <= PredictionRange; i++)
            {
                DateTime nextMonth = lastMonth.AddMonths(i);
                allMonths.Add(nextMonth.ToString("yyyy-MM"));

                currentEstimation += averageTrend;
                predictedCashflow.Add(Math.Round(currentEstimation, 2));
            }

            MonthLables = allMonths;

            LineChartData = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Cashflow",
                    Values = concreteCashflow,
                    PointGeometrySize = 10
                },
                new LineSeries
                {
                    Title = "Estimated",
                    Values = predictedCashflow,
                    PointGeometrySize = 10,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection { 2, 2 }
                }
            };
        }
    }
}
