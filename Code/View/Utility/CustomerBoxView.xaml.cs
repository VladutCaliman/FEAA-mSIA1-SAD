using System.Windows;

namespace Code.View.Utility
{
    /// <summary>
    /// Interaction logic for CustomerBoxView.xaml
    /// </summary>
    public partial class CustomerBoxView : Window
    {
        public CustomerBoxView()
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
