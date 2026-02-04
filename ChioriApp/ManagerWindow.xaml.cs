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
            var orders = _context.Orders.ToList();
            var customers = _context.Customers.ToDictionary(c => c.CustomerId);

            dgOrders.ItemsSource = orders.Select(o => new OrderListItem
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                CustomerId = o.CustomerId,
                CustomerFirstName = customers.TryGetValue(o.CustomerId, out var c) ? c.FirstName : "—",
                CustomerLastName = customers.TryGetValue(o.CustomerId, out var c2) ? c2.LastName : "—",
                CustomerPatronymic = customers.TryGetValue(o.CustomerId, out var c3) ? c3.Patronymic ?? "—" : "—",
                ContactPhone = o.ContactPhone,
                ContactEmail = o.ContactEmail,
                DeliveryAddress = o.DeliveryAddress,
                TotalAmount = o.TotalAmount,
                FinalAmount = o.FinalAmount,
                StatusId = o.StatusId,
                PaymentMethodId = o.PaymentMethodId,
                DeliveryMethodId = o.DeliveryMethodId,
                OrderDate = o.OrderDate
            }).ToList();
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
                var editWindow = new EditOrderWindow(item.OrderId);
                bool? result = editWindow.ShowDialog();
                this.Show();

                if (result == true)
                {
                    LoadOrders();
                }
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
                var editWin = new EditProductWindow(item.ProductId);
                bool? result = editWin.ShowDialog();
                this.Show();

                if (result == true)
                {
                    LoadProducts();
                }
            }
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is ProductListItem item)
            {
                if (MessageBox.Show($"Удалить товар '{item.Name}'?\n\nВнимание: все записи в заказах будут удалены!",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;

                try
                {
                    var productId = item.ProductId;

                    var orderItems = _context.OrderItems
                        .Where(oi => oi.ProductId == productId)
                        .ToList();
                    _context.OrderItems.RemoveRange(orderItems);

                    var product = _context.Products.Find(productId);
                    if (product != null)
                    {
                        _context.Products.Remove(product);
                        _context.SaveChanges();

                        LoadProducts();
                        MessageBox.Show("Товар и его записи в заказах удалены.");
                    }
                    else
                    {
                        MessageBox.Show("Товар не найден.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления:\n{ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }
        private void LoadCustomers()
        {
            try
            {
                var badCustomers = _context.Customers.Where(c => c.UserId <= 0).ToList();
                if (badCustomers.Any())
                {
                    _context.Customers.RemoveRange(badCustomers);
                    _context.SaveChanges();
                }

                var customers = _context.Customers
                    .Include(c => c.User)
                    .ToList();

                var customerList = customers.Select(c => new CustomerListItem
                {
                    CustomerId = c.CustomerId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Patronymic = c.Patronymic ?? "—",
                    Email = c.User?.Email ?? "—",
                    Phone = c.User?.Phone ?? "—",
                    OrderCount = _context.Orders.Count(o => o.CustomerId == c.CustomerId)
                }).ToList();

                dgCustomers.ItemsSource = customerList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}");
            }
        }

        private void BtnRefreshCustomers_Click(object sender, RoutedEventArgs e) => LoadCustomers();

        private void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is CustomerListItem item)
            {
                this.Hide();
                var editWindow = new EditCustomerWindow(item.CustomerId);
                bool? result = editWindow.ShowDialog();

                this.Show();

                if (result == true)
                {
                    LoadCustomers();
                }
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
                    var customerId = item.CustomerId;

                    var orderIds = _context.Orders
                        .Where(o => o.CustomerId == customerId)
                        .Select(o => o.OrderId)
                        .ToList();

                    if (orderIds.Any())
                    {
                        var orderItems = _context.OrderItems
                            .Where(oi => orderIds.Contains(oi.OrderId))
                            .ToList();
                        _context.OrderItems.RemoveRange(orderItems);

                        var orders = _context.Orders
                            .Where(o => o.CustomerId == customerId)
                            .ToList();
                        _context.Orders.RemoveRange(orders);
                    }

                    var customer = _context.Customers.Find(customerId);
                    if (customer != null)
                    {
                        _context.Customers.Remove(customer);
                    }

                    _context.SaveChanges();

                    LoadCustomers();
                    MessageBox.Show("Клиент и его заказы удалены.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления:\n{ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is OrderListItem item)
            {
                if (MessageBox.Show($"Удалить заказ #{item.OrderNumber}?\nЭто действие необратимо.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;

                try
                {
                    var order = _context.Orders
                        .Include(o => o.OrderItems)
                        .FirstOrDefault(o => o.OrderId == item.OrderId);

                    if (order == null)
                    {
                        MessageBox.Show("Заказ не найден.");
                        return;
                    }

                    _context.OrderItems.RemoveRange(order.OrderItems);
                    _context.Orders.Remove(order);
                    _context.SaveChanges();

                    LoadOrders();
                    MessageBox.Show("Заказ удалён.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления заказа:\n{ex.InnerException?.Message ?? ex.Message}");
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