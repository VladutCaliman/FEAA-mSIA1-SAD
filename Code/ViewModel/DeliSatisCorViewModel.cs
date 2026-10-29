using Code.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Code.ViewModel
{
    internal sealed partial class DeliSatisCorViewModel : ObservableObject
    {
        [ObservableProperty] private DataView? _deliveryData;
        [ObservableProperty] private SeriesCollection? _chartData;
        [ObservableProperty] private List<string>? _intervalLabels;

        public DeliSatisCorViewModel()
        {
            PopulateAdvancedAnalysis();
        }

        private void PopulateAdvancedAnalysis()
        {
            string query = @"
                SELECT
                    CASE
                        WHEN DeliveryTime <= 5 THEN '0-5 Days'
                        WHEN DeliveryTime <= 10 THEN '6-10 Days'
                        WHEN DeliveryTime <= 15 THEN '11-15 Days'
                        WHEN DeliveryTime <= 20 THEN '16-20 Days'
                        ELSE 'Peste 20 Days'
                    END AS DeliveryInterval,
                    COUNT(order_id) AS NoOfOrders,
                    ROUND(AVG(review_score), 2) AS MeanScore
                FROM (
                    SELECT
                        o.order_id,
                        r.review_score,
                        CAST(julianday(o.order_delivered_customer_date) - julianday(o.order_purchase_timestamp) AS INTEGER) AS DeliveryTime
                    FROM orders o
                    JOIN order_reviews r ON o.order_id = r.order_id
                    WHERE o.order_status = 'delivered'
                      AND o.order_delivered_customer_date IS NOT NULL
                      AND o.order_purchase_timestamp IS NOT NULL
                      AND r.review_score IS NOT NULL
                ) AS DeliveryData
                WHERE DeliveryTime >= 0
                GROUP BY DeliveryInterval
                ORDER BY
                    CASE DeliveryInterval
                        WHEN '0-5 Days' THEN 1
                        WHEN '6-10 Days' THEN 2
                        WHEN '11-15 Days' THEN 3
                        WHEN '16-20 Days' THEN 4
                        ELSE 5
                    END;";

            DataTable dt = new DataTable();
            List<string> labels = new List<string>();
            ChartValues<double> scores = new ChartValues<double>();

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

            DeliveryData = dt.DefaultView;

            foreach (DataRow row in dt.Rows)
            {
                labels.Add(row["DeliveryInterval"].ToString()!);
                scores.Add(Convert.ToDouble(row["MeanScore"]));
            }

            IntervalLabels = labels;
            ChartData = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Mean Score",
                    Values = scores,
                    DataLabels = true
                }
            };
        }

    }
}
