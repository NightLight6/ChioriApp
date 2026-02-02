using System;
using System.Windows;

namespace ChioriApp
{
    public partial class CheckoutDetailsWindow : Window
    {
        public string FirstName => txtFirstName?.Text?.Trim() ?? string.Empty;
        public string LastName => txtLastName?.Text?.Trim() ?? string.Empty;
        public string Phone => txtPhone?.Text?.Trim() ?? string.Empty;
        public string Email => txtEmail?.Text?.Trim() ?? string.Empty;
        public string Address => txtAddress?.Text?.Trim() ?? string.Empty;

        public CheckoutDetailsWindow()
        {
            InitializeComponent();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                lblError.Text = "Имя обязательно.";
                return;
            }
            if (string.IsNullOrWhiteSpace(LastName))
            {
                lblError.Text = "Фамилия обязательна.";
                return;
            }
            if (string.IsNullOrWhiteSpace(Phone) || !System.Text.RegularExpressions.Regex.IsMatch(Phone, @"^\+?\d{10,15}$"))
            {
                lblError.Text = "Телефон должен содержать 10–15 цифр (например: +79001234567).";
                return;
            }
            if (string.IsNullOrWhiteSpace(Email) || !System.Net.Mail.MailAddress.TryCreate(Email, out _))
            {
                lblError.Text = "Введите корректный email.";
                return;
            }
            if (string.IsNullOrWhiteSpace(Address))
            {
                lblError.Text = "Адрес доставки обязателен.";
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}