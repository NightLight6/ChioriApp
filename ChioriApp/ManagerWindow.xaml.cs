using ChioriApp.Models;
using ChioriApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace ChioriApp
{
    public partial class ManagerWindow : Window
    {
        private readonly ChioriContext _context = new();

        public ManagerWindow()
        {
            InitializeComponent();
            LoadOrders();
            LoadProducts();
            LoadCustomers();
        }
        private void LoadOrders()
        {
            dgOrders.ItemsSource = _context.Orders
                .Select(o => new OrderListItem
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    TotalAmount = o.TotalAmount,
                    StatusId = o.StatusId,
                    OrderDate = o.OrderDate
                })
                .ToList();
        }

        private void BtnRefreshOrders_Click(object sender, RoutedEventArgs e) => LoadOrders();

        private void ViewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is OrderListItem item)
            {
                this.Hide();
                new OrderDetailsWindow(item.OrderId).Show();
            }
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is OrderListItem item)
            {
                this.Hide();
                new EditOrderWindow(item.OrderId).Show();
            }
        }

        private void LoadProducts()
        {
            dgProducts.ItemsSource = _context.Products
                .Select(p => new ProductListItem
                {
                    ProductId = p.ProductId,
                    Article = p.Article,
                    Name = p.Name,
                    Description = p.Description ?? "",
                    RecommendedPrice = p.RecommendedPrice,
                    StatusId = p.StatusId
                })
                .ToList();
        }

        private void BtnRefreshProducts_Click(object sender, RoutedEventArgs e) => LoadProducts();

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddProductWindow();
            if (win.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is ProductListItem item)
            {
                this.Hide();
                new EditProductWindow(item.ProductId).Show();
            }
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is ProductListItem item)
            {
                if (MessageBox.Show($"Удалить товар '{item.Name}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var product = _context.Products.Find(item.ProductId);
                        if (product != null)
                        {
                            _context.Products.Remove(product);
                            _context.SaveChanges();
                            LoadProducts();
                            MessageBox.Show("Товар удалён.");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                }
            }
        }
        private void LoadCustomers()
        {
            var customers = _context.Customers
                .Include(c => c.User)
                .ToList();

            var customerList = customers.Select(c =>
            {
                var orders = _context.Orders.Where(o => o.CustomerId == c.CustomerId).ToList();
                return new CustomerListItem
                {
                    CustomerId = c.CustomerId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.User?.Email ?? "—",
                    Phone = c.User?.Phone ?? "—",
                    OrderCount = orders.Count,
                    OrderIds = string.Join(", ", orders.Select(o => o.OrderId))
                };
            }).ToList();

            dgCustomers.ItemsSource = customerList;
        }

        private void BtnRefreshCustomers_Click(object sender, RoutedEventArgs e) => LoadCustomers();

        private void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is CustomerListItem item)
            {
                this.Hide();
                new EditCustomerWindow(item.CustomerId).Show();
            }
        }

        private void BtnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is CustomerListItem item)
            {
                if (MessageBox.Show($"Удалить клиента:\n{item.FirstName} {item.LastName}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;

                try
                {
                    var customer = _context.Customers
                        .Include(c => c.Orders)
                        .ThenInclude(o => o.OrderItems)
                        .FirstOrDefault(c => c.CustomerId == item.CustomerId);

                    if (customer == null)
                    {
                        MessageBox.Show("Клиент не найден.");
                        return;
                    }

                    foreach (var order in customer.Orders.ToList())
                    {
                        _context.OrderItems.RemoveRange(order.OrderItems);
                        _context.Orders.Remove(order);
                    }

                    _context.Customers.Remove(customer);
                    _context.SaveChanges();

                    LoadCustomers();
                    MessageBox.Show("Клиент и его заказы удалены.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении:\n{ex.Message}");
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new MainWindow().Show();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
        private void BtnCabinet_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new PersonalCabinetWindow().Show();
        }
    }
}