using AdminFelület.Models;
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
            await LoadStatsAsync();
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
            }
            await LoadStatsAsync();
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

        private async Task LoadReceptekAsync()
        {
            var receptek = await _apiService.GetReceptekAsync();

            DataGrid.Columns.Clear();
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Név", Binding = new System.Windows.Data.Binding("Nev"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Feltöltő", Binding = new System.Windows.Data.Binding("FeltoltoUsername"), Width = new DataGridLength(150) });   
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Likes", Binding = new System.Windows.Data.Binding("Likes"), Width = new DataGridLength(70) });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Nehézség", Binding = new System.Windows.Data.Binding("NehezsegiSzint"), Width = new DataGridLength(100) });
            DataGrid.Columns.Add(new DataGridTextColumn { Header = "Idő (perc)", Binding = new System.Windows.Data.Binding("ElkeszitesiIdo"), Width = new DataGridLength(100) });

            DataGrid.ItemsSource = receptek;
        }

        private async Task LoadStatsAsync()
        {
            var stats = await _apiService.GetStatisztikaAsync();

            if (stats != null)
            {
                StatsText.Text = $"Felhasználók: {stats.OsszFelhasznalo}\n" +
                                $"Receptek: {stats.OsszRecept}\n" +
                                $"Kommentek: {stats.OsszKomment}\n" +
                                $"Like-ok: {stats.OsszLike}\n" +
                                $"Képes receptek: {stats.ReceptekKeppel}\n" +
                                $"Kép nélküli: {stats.ReceptekKepNelkul}";
            }
            else
            {
                StatsText.Text = "Hiba a statisztikák betöltésekor";
            }
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedItem == null)
            {
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                SelectedItemTextBlock.Text = "Nincs kiválasztva";
                return;
            }

            EditButton.IsEnabled = true;
            DeleteButton.IsEnabled = true;

            if (_currentView == "users" && DataGrid.SelectedItem is UserModel user)
            {
                SelectedItemTextBlock.Text = $"User: {user.Username}\nRole: {user.Role}\nID: {user.Id}";
            }
            else if (_currentView == "receptek" && DataGrid.SelectedItem is ReceptModel recept)
            {
                SelectedItemTextBlock.Text = $"Recept: {recept.Nev}\nFeltöltő: {recept.FeltoltoUsername}\nLikes: {recept.Likes}\n{recept.ElkeszitesiIdo} perc\nNehezségi szint: {recept.NehezsegiSzint}";
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentView)
            {
                case "users":
                    await LoadUsersAsync();
                    break;
                case "receptek":
                    await LoadReceptekAsync();
                    break;
                case "stats":
                    await LoadStatsAsync();
                    break;
            }
            MessageBox.Show("Nézet frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}