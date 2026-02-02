using ChioriApp.Services;
using System.Linq;
using System.Windows;

namespace ChioriApp
{
    public partial class ManagerOrdersWindow : Window
    {
        private readonly ChioriContext _context = new();

        public ManagerOrdersWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            dgOrders.ItemsSource = _context.Orders
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    o.TotalAmount,
                    o.StatusId,
                    o.OrderDate,
                    o.ContactPhone,
                    o.DeliveryAddress
                })
                .ToList();
        }

        private void DgOrders_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgOrders.SelectedItem != null)
            {
                var selected = dgOrders.SelectedItem as dynamic;
                int orderId = selected.OrderId;

                this.Close();
                new OrderDetailsWindow(orderId).Show();
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            new ManagerWindow().Show();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}