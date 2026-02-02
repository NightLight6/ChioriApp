// MainWindow.xaml.cs
using ChioriApp.Helpers;
using ChioriApp.Models;
using ChioriApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace ChioriApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Заполните все поля.";
                return;
            }

            string hashed = PasswordHasher.ComputeSha256Hash(password);

            using var context = new ChioriContext();
            var user = context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == login && u.PasswordHash == hashed);

            if (user == null)
            {
                lblError.Text = "Неверный логин или пароль.";
                return;
            }

            AppUser.Current = user;

            this.Hide();

            Window nextWindow = user.Role.RoleName switch
            {
                "Администратор" => new AdminWindow(),
                "Менеджер" => new ManagerWindow(),
                "Клиент" => new CatalogWindow(),
                _ => new MainWindow()
            };

            nextWindow.Closed += (s, args) => Application.Current.Shutdown();
            nextWindow.Show();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var registerWindow = new RegisterWindow();
            registerWindow.Closed += (s, args) => this.Show();
            registerWindow.Show();
        }
    }
}