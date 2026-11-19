using Code.View;
using System.Windows;

namespace Code
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new DefaultView());
        }

        private void Button_CustomerStats_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new CustomerStatsView());

        private void Button_CashflowEstim_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new CashflowEstimView());

        private void Button_RegionDistrib_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new RegionDistribView());

        private void Button_PaymentMethod_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new PaymentMethodView());

        private void Button_DeliSatisCor_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new DeliSatisCorView());

        private void Button_PurchasePatterns_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new PurchasePatternsView());
    }
}