using AdminFelület.Models;
using AdminFelület.Services;
using System.Text;
using System.Threading.Tasks;
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
                case 0: _currentView = "users"; await LoadUsersAsync(); break;
                case 1: _currentView = "receptek"; await LoadReceptekAsync(); break;
                case 2: _currentView = "cimkek"; await LoadCimkekAsync(); break;
                case 3: _currentView = "kommentek"; await LoadKommentekAsync(); break;
                case 4: _currentView = "mentett"; await LoadMentettAsync(); break;
                case 5: _currentView = "likes"; await LoadLikesAsync(); break;
                case 6: _currentView = "recept-cimkek"; await LoadReceptCimkekAsync(); break;
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

        private async Task LoadCimkekAsync()
        {
            try
            {
                var cimkek = await _apiService.GetCimkekAsync();

                if (cimkek == null)
                {
                    MessageBox.Show("Hiba: Nincs adat vagy a kérés sikertelen.", "Figyelmeztetés");
                    return;
                }

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Címke neve", Binding = new Binding("CimkeNev"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Receptek száma", Binding = new Binding("ReceptekSzama"), Width = new DataGridLength(150) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), Width = new DataGridLength(80) });
                DataGrid.ItemsSource = cimkek;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a címkék betöltésekor: {ex.Message}", "Hiba");
            }
        }

        private async Task LoadKommentekAsync()
        {
            try
            {
                var kommentek = await _apiService.GetAllKommentekAsync();

                if (kommentek == null)
                {
                    MessageBox.Show("Hiba: Nincs adat vagy a kérés sikertelen.", "Figyelmeztetés");
                    return;
                }

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Szöveg", Binding = new Binding("Szoveg"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "User", Binding = new Binding("Username"), Width = new DataGridLength(120) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Recept", Binding = new Binding("ReceptNev"), Width = new DataGridLength(150) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Dátum", Binding = new Binding("IrtaEkkor") { StringFormat = "yyyy-MM-dd HH:mm" }, Width = new DataGridLength(140) });
                DataGrid.ItemsSource = kommentek;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a kommentek betöltésekor: {ex.Message}", "Hiba");
            }
        }

        private async Task LoadMentettAsync()
        {
            try
            {
                var mentett = await _apiService.GetMentettReceptekAsync();

                if (mentett == null)
                {
                    MessageBox.Show("Hiba: Nincs adat vagy a kérés sikertelen.", "Figyelmeztetés");
                    return;
                }

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "User", Binding = new Binding("Username"), Width = new DataGridLength(150) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Recept", Binding = new Binding("ReceptNev"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Mentve", Binding = new Binding("MentveEkkor") { StringFormat = "yyyy-MM-dd HH:mm" }, Width = new DataGridLength(140) });
                DataGrid.ItemsSource = mentett;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a mentett receptek betöltésekor: {ex.Message}", "Hiba");
            }
        }

        private async Task LoadLikesAsync()
        {
            try
            {
                var likes = await _apiService.GetLikesAsync();

                if (likes == null)
                {
                    MessageBox.Show("Hiba: Nincs adat vagy a kérés sikertelen.", "Figyelmeztetés");
                    return;
                }

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "User", Binding = new Binding("Username"), Width = new DataGridLength(150) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Recept", Binding = new Binding("ReceptNev"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Likeolta", Binding = new Binding("LikeoltaEkkor") { StringFormat = "yyyy-MM-dd HH:mm" }, Width = new DataGridLength(140) });
                DataGrid.ItemsSource = likes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a likek betöltésekor: {ex.Message}", "Hiba");
            }
        }

        private async Task LoadReceptCimkekAsync()
        {
            try
            {
                var kapcsolatok = await _apiService.GetReceptCimkekAsync();

                if (kapcsolatok == null)
                {
                    MessageBox.Show("Hiba: Nincs adat vagy a kérés sikertelen.", "Figyelmeztetés");
                    return;
                }

                DataGrid.Columns.Clear();
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Recept", Binding = new Binding("ReceptNev"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "Címke", Binding = new Binding("CimkeNev"), Width = new DataGridLength(150) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "ReceptID", Binding = new Binding("ReceptId"), Width = new DataGridLength(100) });
                DataGrid.Columns.Add(new DataGridTextColumn { Header = "CimkeID", Binding = new Binding("CimkeId"), Width = new DataGridLength(80) });
                DataGrid.ItemsSource = kapcsolatok;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a recept-címkék betöltésekor: {ex.Message}", "Hiba");
            }
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
                DeleteButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                SelectedItemTextBlock.Text = "Nincs kiválasztva";
                return;
            }

            bool canEdit = _currentView == "users" || _currentView == "receptek" ||
                          _currentView == "cimkek" || _currentView == "kommentek";
            bool canDelete = _currentView != "likes" && _currentView != "recept-cimkek";

            EditButton.IsEnabled = canEdit;
            DeleteButton.IsEnabled = canDelete;

            switch (_currentView)
            {
                case "users" when DataGrid.SelectedItem is UserModel user:
                    SelectedItemTextBlock.Text = $"{user.Username}\nRole: {user.Role}";
                    break;

                case "receptek" when DataGrid.SelectedItem is ReceptModel recept:
                    SelectedItemTextBlock.Text = $"{recept.Nev}\nFeltöltő: {recept.FeltoltoUsername}\n {recept.Likes} likes";
                    break;

                case "cimkek" when DataGrid.SelectedItem is CimkeModel cimke:
                    SelectedItemTextBlock.Text = $"{cimke.CimkeNev}\nReceptek: {cimke.ReceptekSzama} db";
                    break;

                case "kommentek" when DataGrid.SelectedItem is KommentModel komment:
                    SelectedItemTextBlock.Text = $"{komment.Username}\nRecept: {komment.ReceptNev}\n{komment.Szoveg.Substring(0, Math.Min(50, komment.Szoveg.Length))}...";
                    break;

                case "mentett" when DataGrid.SelectedItem is MentettReceptModel mentett:
                    SelectedItemTextBlock.Text = $"{mentett.Username}\nRecept: {mentett.ReceptNev}\nMentve: {mentett.MentveEkkor:yyyy-MM-dd}";
                    break;

                case "likes" when DataGrid.SelectedItem is LikeModel like:
                    SelectedItemTextBlock.Text = $"{like.Username}\nRecept: {like.ReceptNev}\nLikeolta: {like.LikeoltaEkkor:yyyy-MM-dd}";
                    break;

                case "recept-cimkek" when DataGrid.SelectedItem is ReceptCimkeModel rc:
                    SelectedItemTextBlock.Text = $"{rc.ReceptNev}\n {rc.CimkeNev}";
                    break;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            switch (_currentView)
            {
                case "users": await LoadUsersAsync(); break;
                case "receptek": await LoadReceptekAsync(); break;
                case "cimkek": await LoadCimkekAsync(); break;
                case "kommentek": await LoadKommentekAsync(); break;
                case "mentett": await LoadMentettAsync(); break;
                case "likes": await LoadLikesAsync(); break;
                case "recept-cimkek": await LoadReceptCimkekAsync(); break;
            }

            await LoadStatsAsync();
            IsEnabled = true;
            MessageBox.Show("Frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem == null) return;

            var result = MessageBox.Show("Szeretné szerkeszteni a kiválasztott elemet?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No) return;

            IsEnabled = false;
            bool success = false;

            try
            {
                switch (_currentView)
                {
                    case "users" when DataGrid.SelectedItem is UserModel user:
                        success = await _apiService.UpdateUserAsync(user);
                        if (success) await LoadUsersAsync();
                        break;
                    case "receptek" when DataGrid.SelectedItem is ReceptModel recept:
                        success = await _apiService.UpdateReceptAsync(recept);
                        if (success) await LoadReceptekAsync();
                        break;
                    case "cimkek" when DataGrid.SelectedItem is CimkeModel cimke:
                        success = await _apiService.UpdateCimkeAsync(cimke);
                        if (success) await LoadStatsAsync();
                        break;
                    case "kommentek" when DataGrid.SelectedItem is KommentModel komment:
                        success = await _apiService.UpdateKommentAsync(komment);
                        if (success) await LoadKommentekAsync();
                        break;
                }

                IsEnabled = true;
                await LoadStatsAsync();

                if (success)
                    MessageBox.Show("Sikeres mentés!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Hiba történt a mentés során.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            catch (Exception ex)
            {
                IsEnabled = true;
                MessageBox.Show($"Hiba a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem == null) return;

            var result = MessageBox.Show("Biztosan törölni szeretné a kiválasztott elemet?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;

            IsEnabled = false;
            bool success = false;

            try
            {
                switch (_currentView)
                {
                    case "users" when DataGrid.SelectedItem is UserModel user:
                        success = await _apiService.DeleteUserAsync(user.Id);
                        if (success) await LoadUsersAsync();
                        break;
                    case "receptek" when DataGrid.SelectedItem is ReceptModel recept:
                        success = await _apiService.DeleteReceptAsync(recept.Id);
                        if (success) await LoadReceptekAsync();
                        break;
                    case "cimkek" when DataGrid.SelectedItem is CimkeModel cimke:
                        success = await _apiService.DeleteCimkeAsync(cimke.Id);
                        if (success) await LoadCimkekAsync();
                        break;
                    case "kommentek" when DataGrid.SelectedItem is KommentModel komment:
                        success = await _apiService.DeleteKommentAsync(komment.Id);
                        if (success) await LoadKommentekAsync();
                        break;
                }

                await LoadStatsAsync();

                if (success)
                    MessageBox.Show("Sikeres törlés!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Hiba történt a törlés során.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsEnabled = true;
            }
        }
    }
}