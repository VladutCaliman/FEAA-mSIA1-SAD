using System.Windows;

namespace Code.View.Utility
{
    /// <summary>
    /// Interaction logic for CashflowBoxView.xaml
    /// </summary>
    public partial class CashflowBoxView : Window
    {
        public CashflowBoxView()
        {
            InitializeComponent();
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
