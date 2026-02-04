using ChioriApp.Helpers;
using ChioriApp.Models;
using ChioriApp.Services;
using ChioriApp.Validators;
using System;
using System.Linq;
using System.Windows;

namespace ChioriApp
{
    public partial class RegisterWindow : Window
    {
        private readonly ChioriContext _context = new();

        public RegisterWindow()
        {
            InitializeComponent();
            LoadRoles();
        }

        private void LoadRoles()
        {
            try
            {
                var allRoles = _context.Roles.ToList();

                if (!allRoles.Any())
                {
                    lblError.Text = "Ошибка: в системе нет ролей. Обратитесь к администратору.";
                    return;
                }

                cmbRole.ItemsSource = allRoles;

                var clientRole = allRoles.FirstOrDefault(r =>
                    r.RoleName?.Equals("Клиент", StringComparison.OrdinalIgnoreCase) == true);

                if (clientRole != null)
                {
                    cmbRole.SelectedItem = clientRole;
                }
                else
                {
                    cmbRole.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ошибка загрузки ролей: {ex.Message}";
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string patronymic = txtpatronymic.Text.Trim();
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                lblError.Text = "Имя и фамилия обязательны.";
                return;
            }

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            {
                lblError.Text = "Логин, пароль и email обязательны.";
                return;
            }

            var selectedRole = cmbRole.SelectedItem as Role;
            if (selectedRole == null)
            {
                lblError.Text = "Выберите роль.";
                return;
            }

            if (_context.Users.Any(u => u.Username == login))
            {
                lblError.Text = "Пользователь с таким логином уже существует.";
                return;
            }
            if (_context.Users.Any(u => u.Email == email))
            {
                lblError.Text = "Email уже зарегистрирован.";
                return;
            }

            var newUser = new User
            {
                Username = login,
                Email = email,
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                RoleId = selectedRole.RoleId,
                AccountStatusId = 1,
                Created_at = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
            };

            newUser.PasswordHash = PasswordHasher.ComputeSha256Hash(password);

            var validator = new UserValidator();
            var errors = validator.Validate(newUser);
            if (errors.Count > 0)
            {
                lblError.Text = string.Join("\n", errors.Select(er => er.ErrorMessage));
                return;
            }

            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();

                var customer = new Customer
                {
                    UserId = newUser.UserId,
                    FirstName = firstName,
                    LastName = lastName,
                    Patronymic = string.IsNullOrWhiteSpace(patronymic) ? null : patronymic,
                    BirthDate = dpBirthDate.SelectedDate?.Date,
                    RegistrationDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Hide();
                new MainWindow().Show();
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ошибка при сохранении: {ex.Message}\n\n{ex.InnerException?.Message}";
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new MainWindow().Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}