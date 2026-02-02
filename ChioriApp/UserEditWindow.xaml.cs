using ChioriApp.Models;
using ChioriApp.Services;
using System.Linq;
using System.Windows;

namespace ChioriApp
{
    public partial class UserEditWindow : Window
    {
        private readonly ChioriContext _context = new();
        private readonly User _user;

        public UserEditWindow(User user)
        {
            InitializeComponent();
            _user = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            txtLogin.Text = _user.Username;
            txtEmail.Text = _user.Email;
            txtPhone.Text = _user.Phone ?? "";

            var adminRoleId = _context.Roles.First(r => r.RoleName == "Администратор").RoleId;
            var roles = _context.Roles.Where(r => r.RoleId != adminRoleId).ToList();
            cmbRole.ItemsSource = roles;
            cmbRole.SelectedValue = _user.RoleId;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email))
            {
                lblError.Text = "Логин и email обязательны.";
                return;
            }

            var selectedRoleId = cmbRole.SelectedValue as int?;
            if (!selectedRoleId.HasValue)
            {
                lblError.Text = "Выберите роль.";
                return;
            }

            if (_context.Users.Any(u => u.Username == login && u.UserId != _user.UserId))
            {
                lblError.Text = "Пользователь с таким логином уже существует.";
                return;
            }
            if (_context.Users.Any(u => u.Email == email && u.UserId != _user.UserId))
            {
                lblError.Text = "Email уже зарегистрирован другим пользователем.";
                return;
            }

            try
            {
                _user.Username = login;
                _user.Email = email;
                _user.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone;
                _user.RoleId = selectedRoleId.Value;

                _context.SaveChanges();
                MessageBox.Show("Данные пользователя успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Hide();
                new AdminWindow().Show();
            }
            catch (System.Exception ex)
            {
                lblError.Text = $"Ошибка при сохранении: {ex.Message}";
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new AdminWindow().Show();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}