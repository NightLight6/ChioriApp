using ChioriApp.Models;
using ChioriApp.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ChioriApp
{
    public partial class EditOrderWindow : Window
    {
        private readonly ChioriContext _context = new();
        private readonly int _orderId;

        public EditOrderWindow(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            LoadOrder();
        }

        private void LoadOrder()
        {
            var order = _context.Orders.Find(_orderId);
            if (order == null)
            {
                MessageBox.Show("Заказ не найден.");
                Close();
                return;
            }

            txtAddress.Text = order.DeliveryAddress;
            txtPhone.Text = order.ContactPhone;
            txtEmail.Text = order.ContactEmail;

            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Tag?.ToString() == order.StatusId.ToString())
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var order = _context.Orders.Find(_orderId);
            if (order == null) return;

            order.DeliveryAddress = txtAddress.Text.Trim();
            order.ContactPhone = txtPhone.Text.Trim();
            order.ContactEmail = txtEmail.Text.Trim();

            if (cmbStatus.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Tag?.ToString(), out int statusId))
            {
                order.StatusId = statusId;
            }

            try
            {
                _context.SaveChanges();
                MessageBox.Show("Заказ обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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