#nullable disable
using ChioriApp.Models;
using ChioriApp.Services;
using System.Linq;
using System.Windows;

namespace ChioriApp
{
    public partial class CheckoutWindow : Window
    {
        private readonly ChioriContext _context = new();
        private readonly int? _userId;

        public CheckoutWindow(int? userId = null)
        {
            InitializeComponent();
            _userId = userId;
            LoadBasket();
        }

        private void LoadBasket()
        {
            var items = BasketService.Items.Select(i => new
            {
                i.ProductName,
                i.Price,
                i.Quantity,
                Total = i.Price * i.Quantity
            }).ToList();

            dgBasket.ItemsSource = items;
            lblTotal.Text = BasketService.GetTotal().ToString("F2");
        }

        private void BtnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!BasketService.Items.Any())
            {
                MessageBox.Show("Корзина пуста.");
                return;
            }

            var detailsWindow = new CheckoutDetailsWindow();
            if (detailsWindow.ShowDialog() != true)
                return;

            try
            {
                var orderNumber = $"CH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

                Customer customer;
                int? userId = _userId;

                if (userId.HasValue)
                {
                    var safeUserId = userId.Value;
                    customer = _context.Customers.FirstOrDefault(c => c.UserId == safeUserId);
                    if (customer == null)
                    {
                        customer = new Customer
                        {
                            UserId = safeUserId,
                            FirstName = detailsWindow.FirstName,
                            LastName = detailsWindow.LastName,
                            Patronymic = detailsWindow.Patronymic,
                            RegistrationDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
                        };
                        _context.Customers.Add(customer);
                    }
                }
                else
                {
                    customer = new Customer
                    {
                        UserId = _userId.GetValueOrDefault(0),
                        FirstName = detailsWindow.FirstName,
                        LastName = detailsWindow.LastName,
                        Patronymic = detailsWindow.Patronymic,
                        RegistrationDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
                    };
                    _context.Customers.Add(customer);
                }

                _context.SaveChanges();

                var order = new Order
                {
                    OrderNumber = orderNumber,
                    CustomerId = customer.CustomerId,
                    StatusId = 1,
                    DeliveryMethodId = detailsWindow.DeliveryMethodId,
                    PaymentMethodId = detailsWindow.PaymentMethodId,
                    Comments = detailsWindow.Comments,
                    DeliveryAddress = detailsWindow.Address,
                    ContactName = $"{detailsWindow.FirstName} {detailsWindow.LastName}",
                    ContactPhone = detailsWindow.Phone,
                    ContactEmail = detailsWindow.Email,
                    TotalAmount = BasketService.GetTotal(),
                    FinalAmount = BasketService.GetTotal(),
                    OrderDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                foreach (var item in BasketService.Items)
                {
                    decimal price = item.Price;
                    int qty = item.Quantity > 0 ? item.Quantity : 1;
                    decimal subtotal = Math.Round(price * qty, 2);

                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = qty,
                        PriceAtOrder = price,
                    });
                }

                _context.SaveChanges();
                BasketService.Clear();

                MessageBox.Show($"Заказ #{orderNumber} оформлен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                new CatalogWindow().Show();
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Ошибка при оформлении заказа:\n{inner}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            new CatalogWindow().Show();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}