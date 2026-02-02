using ChioriApp.Models;
using ChioriApp.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ChioriApp
{
    public partial class UserEditWindow : Window
    {
        private class AccountStatusOption
        {
            public string Display { get; set; } = string.Empty;
            public int Value { get; set; }
        }

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

            var customer = _context.Customers.FirstOrDefault(c => c.UserId == _user.UserId);
            if (customer != null)
            {
                txtFirstName.Text = customer.FirstName;
                txtLastName.Text = customer.LastName;
                txtMiddleName.Text = customer.Patronymic ?? "";
            }

            var adminRoleId = _context.Roles.First(r => r.RoleName == "Администратор").RoleId;
            var roles = _context.Roles.Where(r => r.RoleId != adminRoleId).ToList();
            cmbRole.ItemsSource = roles;
            cmbRole.SelectedValue = _user.RoleId;

            var statusOptions = new[]
            {
                new AccountStatusOption { Display = "Активен", Value = 1 },
                new AccountStatusOption { Display = "Заблокирован", Value = 2 }
            };
            cmbAccountStatus.ItemsSource = statusOptions;
            cmbAccountStatus.DisplayMemberPath = "Display";
            cmbAccountStatus.SelectedValuePath = "Value";

            var selectedStatus = statusOptions.FirstOrDefault(s => s.Value == _user.AccountStatusId);
            if (selectedStatus != null)
            {
                cmbAccountStatus.SelectedItem = selectedStatus;
            }

            lblRegistrationDate.Text = _user.Created_at.ToString("dd.MM.yyyy");
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

            var accountStatusId = cmbAccountStatus.SelectedValue as int?;
            if (!accountStatusId.HasValue)
            {
                lblError.Text = "Выберите статус аккаунта.";
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
                _user.AccountStatusId = accountStatusId.Value;

                var customer = _context.Customers.FirstOrDefault(c => c.UserId == _user.UserId);
                if (customer == null)
                {
                    if (_user.UserId <= 0)
                    {
                        lblError.Text = "Невозможно создать профиль: UserId недопустим.";
                        return;
                    }

                    customer = new Customer
                    {
                        UserId = _user.UserId,
                        FirstName = txtFirstName.Text.Trim(),
                        LastName = txtLastName.Text.Trim(),
                        Patronymic = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? null : txtMiddleName.Text.Trim(),
                    };
                    _context.Customers.Add(customer);
                }
                else
                {
                    customer.FirstName = txtFirstName.Text.Trim();
                    customer.LastName = txtLastName.Text.Trim();
                    customer.Patronymic = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? null : txtMiddleName.Text.Trim();
                }

                _context.SaveChanges();
                MessageBox.Show("Данные пользователя успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                var errorMsg = $"Ошибка:\n{ex.Message}\n\nПодробности:\n{ex.InnerException?.Message}";
                lblError.Text = errorMsg;
                MessageBox.Show(errorMsg, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}