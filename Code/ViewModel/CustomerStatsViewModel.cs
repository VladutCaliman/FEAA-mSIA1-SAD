using Code.Model.Utility;
using Code.Utility;
using Code.View.Utility;
using Code.ViewModel.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Windows;

namespace Code.ViewModel
{
    internal sealed partial class CustomerStatsViewModel : ObservableObject
    {
        [ObservableProperty] private DataView? _queryData;
        [ObservableProperty] private SeriesCollection? _pieChartData;
        [ObservableProperty] private DataRowView? _selectedData;

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
            else
            {
                MessageBox.Show("Couldn't get selection criteria.", "Error.", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                throw new Exception("Couldn't get selection criteria");
            }

            HandleChoice(_selectedCriteria);
        }

        private void HandleChoice(CustomerBoxResult boxResult)
        {
            switch (boxResult.CustomerCriteria)
            {
                case CustomerCriteriaEnum.MinRevenue:
                    string queryMinRev = @$"
                        SELECT
                            customers.first_name AS FirstName,
                            customers.last_name AS LastName,
	                        ROUND(SUM(order_payments.payment_value),2) AS Revenue
                        FROM customers
                        INNER JOIN orders USING (customer_id)
                        INNER JOIN order_payments USING (order_id)
                        GROUP BY customers.customer_unique_id
                        HAVING Revenue > 0.0
                        ORDER BY Revenue ASC
                        LIMIT {_selectedCriteria.QueryRange};";
                    QueryDatabase(queryMinRev);
                    break;
                case CustomerCriteriaEnum.MaxRevenue:
                    string queryMaxRev = @$"
                        SELECT 
                            customers.first_name AS FirstName,
                            customers.last_name AS LastName,
	                        ROUND(SUM(order_payments.payment_value),2) AS Revenue
                        FROM customers
                        INNER JOIN orders USING (customer_id)
                        INNER JOIN order_payments USING (order_id)
                        GROUP BY customers.customer_unique_id
                        HAVING Revenue > 0.0
                        ORDER BY Revenue DESC
                        LIMIT {_selectedCriteria.QueryRange};";
                    QueryDatabase(queryMaxRev);
                    break;
                case CustomerCriteriaEnum.MinOrders:
                    string queryMinOrd = $@"
                        SELECT
                            customers.first_name AS FirstName,
                            customers.last_name AS LastName,
                            ROUND(SUM(order_payments.payment_value),2) AS Revenue,
                            COUNT(order_id) AS NoOfOrders
                        FROM customers
                        INNER JOIN orders USING (customer_id)
                        INNER JOIN order_payments USING (order_id)
                        GROUP BY customers.customer_unique_id
                        ORDER BY NoOfOrders ASC
                        LIMIT {_selectedCriteria.QueryRange};";
                    QueryDatabase(queryMinOrd);
                    break;
                case CustomerCriteriaEnum.MaxOrders:
                    string queryMaxOrd = $@"
                        SELECT
                            customers.first_name AS FirstName,
                            customers.last_name AS LastName,
                            ROUND(SUM(order_payments.payment_value),2) AS Revenue,
                            COUNT(order_id) AS NoOfOrders
                        FROM customers
                        INNER JOIN orders USING (customer_id)
                        INNER JOIN order_payments USING (order_id)
                        GROUP BY customers.customer_unique_id
                        ORDER BY NoOfOrders DESC
                        LIMIT {_selectedCriteria.QueryRange};";
                    QueryDatabase(queryMaxOrd);
                    break;
                case CustomerCriteriaEnum.None:
                default:
                    return;
            }
        }

        private void QueryDatabase(string query)
        {
            using (var conn = new SqliteConnection(DatabaseConnectionString.ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqliteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        QueryData = dt.DefaultView;

                        foreach (DataRow row in dt.Rows)
                        {
                            _totalRevenue += Convert.ToDouble(row["Revenue"]);
                        }
                    }
                }
            }
        }

        partial void OnSelectedDataChanged(DataRowView? value)
        {
            SeriesCollection pieChartData = new SeriesCollection();
            DataRow currentRow = value!.Row;

            pieChartData.Add(new PieSeries
            {
                Title = $"{currentRow["FirstName"]} {currentRow["LastName"]}",
                Values = new ChartValues<double> { Convert.ToDouble(currentRow["Revenue"]) },
                DataLabels = true,
            });

            pieChartData.Add(new PieSeries
            {
                Title = "Total batch revenue",
                Values = new ChartValues<double> { Math.Round(_totalRevenue - Convert.ToDouble(currentRow["Revenue"]), 2) },
                DataLabels = true
            });

            PieChartData = pieChartData;
        }
    }
}
