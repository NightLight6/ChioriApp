using ChioriApp.Models;
using ChioriApp.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;

namespace ChioriApp
{
    public partial class EditCustomerWindow : Window
    {
        private readonly ChioriContext _context = new();
        private readonly int _customerId;

        public EditCustomerWindow(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            LoadCustomer();
        }

        private void LoadCustomer()
        {
            var customer = _context.Customers
                .Include(c => c.User)
                .FirstOrDefault(c => c.CustomerId == _customerId);

            if (customer == null)
            {
                MessageBox.Show("Клиент не найден.");
                Close();
                return;
            }

            txtFirstName.Text = customer.FirstName;
            txtLastName.Text = customer.LastName;
            txtEmail.Text = customer.User?.Email ?? "";
            txtPhone.Text = customer.User?.Phone ?? "";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                lblError.Text = "Имя и фамилия обязательны.";
                return;
            }

            var customer = _context.Customers
                .Include(c => c.User)
                .FirstOrDefault(c => c.CustomerId == _customerId);

            if (customer == null) return;

            customer.FirstName = firstName;
            customer.LastName = lastName;

            if (customer.User == null)
            {
                customer.User = new User { Username = $"cust_{_customerId}", AccountStatusId = 1 };
                _context.Users.Add(customer.User);
            }

            customer.User.Email = email;
            customer.User.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone;

            try
            {
                _context.SaveChanges();
                MessageBox.Show("Клиент обновлён!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ошибка: {ex.Message}";
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