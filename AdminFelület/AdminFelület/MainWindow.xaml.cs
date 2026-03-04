using AdminFelület.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminFelület
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;
        private string _currentView = "users";

        public MainWindow(Services.ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            ViewSelector.SelectionChanged += ViewSelector_SelectionChanged;

            ViewSelector.SelectedIndex = 0;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsersAsync();
        }

        private async void ViewSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewSelector.SelectedIndex == -1) return;

            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            SelectedItemTextBlock.Text = "Nincs kiválasztva";

            switch (ViewSelector.SelectedIndex)
            {
                case 0:
                    _currentView = "users";
                    await LoadUsersAsync();
                    break;

                case 1:
                    _currentView = "receptek";
                    await LoadReceptekAsync();
                    break;

                case 2:
                    _currentView = "kommentek";
                    MessageBox.Show("Válassz egy receptet a receptek nézetben!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                case 3:
                    _currentView = "stats";
                    await LoadStatsAsync();
                    break;
            }
        }

        private async Task LoadUsersAsync()
        {
            var users = await _apiService.GetUsersAsync();

            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Username", Binding = new Binding("Username") });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Role", Binding = new Binding("Role") });

            DataGrid.ItemsSource = users;
        }

        private async Task LoadStatsAsync()
        {
            throw new NotImplementedException();
        }

        private async Task LoadReceptekAsync()
        {
            throw new NotImplementedException();
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}