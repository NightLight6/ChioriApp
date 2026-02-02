using ChioriApp.Models;
using ChioriApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
                MessageBox.Show("Сессия истекла. Пожалуйста, войдите снова.");
                Close();
                return;
            }

            var customer = _context.Customers
                .FirstOrDefault(c => c.UserId == currentUser.UserId);

            if (customer != null)
            {
                lblFullName.Text = $"{customer.FirstName} {customer.LastName} {customer.Patronymic}";
                lblLogin.Text = $"Логин: {currentUser.Username}";
                lblEmail.Text = $"Email: {currentUser.Email}";
                lblPhone.Text = $"Телефон: {currentUser.Phone ?? "Не указан"}";
                lblRegistrationDate.Text = $"Дата регистрации: {customer.RegistrationDate:dd.MM.yyyy}";

                var orders = _context.Orders
                    .Where(o => o.CustomerId == customer.CustomerId)
                    .Select(o => new
                    {
                        o.OrderNumber,
                        o.OrderDate,
                        o.TotalAmount,
                        o.StatusId
                    })
                    .ToList();

                dgOrders.ItemsSource = orders;
            }
            else
            {
                lblFullName.Text = "Клиент не зарегистрирован";
                lblLogin.Text = $"Логин: {currentUser.Username}";
                lblEmail.Text = $"Email: {currentUser.Email}";
                lblPhone.Text = $"Телефон: {currentUser.Phone ?? "Не указан"}";
                dgOrders.ItemsSource = new object[0];
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