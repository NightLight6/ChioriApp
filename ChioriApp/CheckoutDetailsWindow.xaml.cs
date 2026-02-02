using System;
using System.Windows;
using System.Windows.Controls;

namespace ChioriApp
{
    public partial class CheckoutDetailsWindow : Window
    {
        public string FirstName => txtFirstName?.Text?.Trim() ?? string.Empty;
        public string LastName => txtLastName?.Text?.Trim() ?? string.Empty;
        public string Patronymic => txtPatronymic?.Text?.Trim() ?? string.Empty;
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
            if (DeliveryMethodId <= 0 || DeliveryMethodId > 2)
            {
                lblError.Text = "Выберите способ доставки.";
                return;
            }
            if (PaymentMethodId <= 0 || PaymentMethodId > 2)
            {
                lblError.Text = "Выберите способ оплаты.";
                return;
            }

            DialogResult = true;
            Close();
        }
        public int DeliveryMethodId =>
    cmbDeliveryMethod.SelectedItem is ComboBoxItem item && int.TryParse(item.Tag?.ToString(), out int id) ? id : 1;

        public int PaymentMethodId =>
            cmbPaymentMethod.SelectedItem is ComboBoxItem item2 && int.TryParse(item2.Tag?.ToString(), out int id2) ? id2 : 1;

        public string Comments => txtComments?.Text?.Trim() ?? string.Empty;
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}