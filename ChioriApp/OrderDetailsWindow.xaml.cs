// OrderDetailsWindow.xaml.cs
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

            if (order == null) return;

            decimal total = order.OrderItems.Sum(oi => oi.Subtotal);
            order.FinalAmount = total;
            _context.SaveChanges();

            var sb = new StringBuilder();
            sb.AppendLine($"Заказ №{order.OrderId}");
            sb.AppendLine($"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Клиент ID: {order.CustomerId}");
            sb.AppendLine($"Статус: {GetStatusName(order.StatusId)}");
            sb.AppendLine($"Адрес: {order.DeliveryAddress}");
            sb.AppendLine($"Телефон: {order.ContactPhone}");
            sb.AppendLine($"Email: {order.ContactEmail}");
            lblOrderInfo.Text = sb.ToString();

            dgItems.ItemsSource = order.OrderItems.Select(oi => new
            {
                ProductName = oi.Product.Name,
                oi.Quantity,
                oi.PriceAtOrder,
                oi.Subtotal
            }).ToList();
        }

        private string GetStatusName(int statusId)
        {
            return statusId switch
            {
                1 => "Новый",
                2 => "В обработке",
                3 => "Отправлен",
                4 => "Доставлен",
                5 => "Отменён",
                _ => "Неизвестный"
            };
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new ManagerOrdersWindow().Show();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new EditOrderWindow(_orderId).ShowDialog();
            LoadOrderDetails();
        }
    }
}