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

            foreach (ComboBoxItem item in cmbDeliveryMethod.Items)
            {
                if (item.Tag?.ToString() == order.DeliveryMethodId.ToString())
                {
                    cmbDeliveryMethod.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in cmbPaymentMethod.Items)
            {
                if (item.Tag?.ToString() == order.PaymentMethodId.ToString())
                {
                    cmbPaymentMethod.SelectedItem = item;
                    break;
                }
            }

            txtPickupCode.Text = order.PickupCode ?? "";
            txtComments.Text = order.Comments ?? "";

            dpPlannedDelivery.SelectedDate = order.PlannedDeliveryDate;
            dpActualDelivery.SelectedDate = order.ActualDeliveryDate;

            txtTotalAmount.Text = order.TotalAmount.ToString("F2");
            txtDiscountAmount.Text = order.DiscountAmount.ToString("F2");
            txtFinalAmount.Text = order.FinalAmount.ToString("F2");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var order = _context.Orders.Find(_orderId);
            if (order == null) return;

            try
            {
                order.DeliveryAddress = txtAddress.Text.Trim();
                order.ContactPhone = txtPhone.Text.Trim();
                order.ContactEmail = txtEmail.Text.Trim();

                if (cmbStatus.SelectedItem is ComboBoxItem statusItem &&
                    int.TryParse(statusItem.Tag?.ToString(), out int statusId))
                {
                    order.StatusId = statusId;
                }

                if (cmbDeliveryMethod.SelectedItem is ComboBoxItem deliveryItem &&
                    int.TryParse(deliveryItem.Tag?.ToString(), out int deliveryId))
                {
                    order.DeliveryMethodId = deliveryId;
                }

                if (cmbPaymentMethod.SelectedItem is ComboBoxItem paymentItem &&
                    int.TryParse(paymentItem.Tag?.ToString(), out int paymentId))
                {
                    order.PaymentMethodId = paymentId;
                }

                order.PickupCode = string.IsNullOrWhiteSpace(txtPickupCode.Text) ? null : txtPickupCode.Text.Trim();
                order.Comments = string.IsNullOrWhiteSpace(txtComments.Text) ? null : txtComments.Text.Trim();

                order.PlannedDeliveryDate = dpPlannedDelivery.SelectedDate;
                order.ActualDeliveryDate = dpActualDelivery.SelectedDate;

                if (decimal.TryParse(txtTotalAmount.Text, out decimal total))
                    order.TotalAmount = total;
                if (decimal.TryParse(txtDiscountAmount.Text, out decimal discount))
                    order.DiscountAmount = discount;
                if (decimal.TryParse(txtFinalAmount.Text, out decimal final))
                    order.FinalAmount = final;

                _context.SaveChanges();
                MessageBox.Show("Заказ обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ошибка при сохранении:\n{ex.Message}";
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