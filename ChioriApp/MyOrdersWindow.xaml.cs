using ChioriApp.Services;
using System.Linq;
using System.Windows;

namespace ChioriApp
{
    public partial class MyOrdersWindow : Window
    {
        public MyOrdersWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            using var context = new ChioriContext();
            var currentUserId = 1;
            dgOrders.ItemsSource = context.Orders
                .Where(o => o.CustomerId == currentUserId)
                .ToList();
        }
    }
}