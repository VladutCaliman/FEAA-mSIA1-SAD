using Code.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Windows.Media;

namespace Code.ViewModel
{
    internal sealed partial class PurchasePatternsViewModel : ObservableObject
    {
        [ObservableProperty] private DataView? _patternsData;
        [ObservableProperty] private SeriesCollection? _chartData;
        [ObservableProperty] private List<string>? _hourLabels;

        public PurchasePatternsViewModel()
        {
            PopulateAdvancedPatterns();
        }

        private void PopulateAdvancedPatterns()
        {
            string query = @"
                SELECT 
                    strftime('%H', order_purchase_timestamp) AS HourOfDay,
                    SUM(CASE WHEN strftime('%w', order_purchase_timestamp) IN ('1', '2', '3', '4', '5') THEN 1 ELSE 0 END) AS WeekdayOrders,
                    SUM(CASE WHEN strftime('%w', order_purchase_timestamp) IN ('0', '6') THEN 1 ELSE 0 END) AS WeekendOrders
                FROM orders
                WHERE order_purchase_timestamp IS NOT NULL
                GROUP BY HourOfDay
                ORDER BY HourOfDay ASC;";

            DataTable dt = new DataTable();
            List<string> labels = new List<string>();
            ChartValues<int> weekdayValues = new ChartValues<int>();
            ChartValues<int> weekendValues = new ChartValues<int>();

            using (var conn = new SqliteConnection(DatabaseConnectionString.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            PatternsData = dt.DefaultView;

            foreach (DataRow row in dt.Rows)
            {
                labels.Add(row["HourOfDay"].ToString()! + ":00");
                weekdayValues.Add(Convert.ToInt32(row["WeekdayOrders"]));
                weekendValues.Add(Convert.ToInt32(row["WeekendOrders"]));
            }

            HourLabels = labels;
            ChartData = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Weekday Volume",
                    Values = weekdayValues,
                    PointGeometrySize = 8,
                    Stroke = Brushes.DodgerBlue
                },
                new LineSeries
                {
                    Title = "Weekend Volume",
                    Values = weekendValues,
                    PointGeometrySize = 8,
                    Stroke = Brushes.Tomato
                }
            };
        }
    }
}
