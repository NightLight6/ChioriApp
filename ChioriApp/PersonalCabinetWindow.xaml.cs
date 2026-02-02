using ChioriApp.Models;
using ChioriApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace ChioriApp
{
    public partial class PersonalCabinetWindow : Window
    {
        private readonly ChioriContext _context = new();

        public PersonalCabinetWindow()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            var currentUser = AppUser.Current;
            if (currentUser == null)
            {
                MessageBox.Show("Сессия истекла.");
                Close();
                return;
            }

            var customer = _context.Customers
                .FirstOrDefault(c => c.UserId == currentUser.UserId);

            if (customer != null)
            {
                lblFullName.Text = $"{customer.FirstName} {customer.LastName}";
                lblLogin.Text = $"Логин: {currentUser.Username}";
                lblEmail.Text = $"Email: {currentUser.Email}";
                lblPhone.Text = $"Телефон: {currentUser.Phone ?? "Не указан"}";
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}