using ChioriApp.Models;
using ChioriApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Windows;

namespace ChioriApp
{
    public partial class OrderDetailsWindow : Window
    {
        private readonly ChioriContext _context = new();
        private readonly int _orderId;

        public OrderDetailsWindow(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.OrderId == _orderId);

            if (order == null)
            {
                MessageBox.Show("Заказ не найден.");
                return;
            }

            var total = order.OrderItems.Sum(oi => oi.Subtotal);
            if (order.FinalAmount != total)
            {
                order.FinalAmount = total;
                _context.SaveChanges();
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Заказ №{order.OrderNumber}");
            sb.AppendLine($"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Клиент: {order.ContactName}");
            sb.AppendLine($"Телефон: {order.ContactPhone}");
            sb.AppendLine($"Email: {order.ContactEmail}");
            sb.AppendLine($"Адрес доставки: {order.DeliveryAddress}");
            sb.AppendLine($"Статус: {GetStatusName(order.StatusId)}");
            sb.AppendLine($"Способ оплаты: {GetPaymentMethodName(order.PaymentMethodId)}");
            sb.AppendLine($"Способ доставки: {GetDeliveryMethodName(order.DeliveryMethodId)}");
            sb.AppendLine($"Итоговая сумма: {order.FinalAmount:C}");

            lblOrderInfo.Text = sb.ToString();

            dgItems.ItemsSource = order.OrderItems.ToList();
        }

        private string GetStatusName(int statusId) => statusId switch
        {
            1 => "Новый",
            2 => "В обработке",
            3 => "Отправлен",
            4 => "Доставлен",
            5 => "Отменён",
            _ => "Неизвестный"
        };

        private string GetPaymentMethodName(int methodId) => methodId switch
        {
            1 => "Онлайн",
            2 => "Наличными",
            _ => "Не указан"
        };

        private string GetDeliveryMethodName(int methodId) => methodId switch
        {
            1 => "Курьер",
            2 => "Самовывоз",
            _ => "Не указан"
        };

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            new ManagerWindow().Show();
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var editWindow = new EditOrderWindow(_orderId);
            bool? result = editWindow.ShowDialog();
            this.Show();

            if (result == true)
            {
                LoadOrderDetails();
            }
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}