using ChioriApp.Models;
using ChioriApp.Services;
using System.Windows;
using System.Windows.Controls;

namespace ChioriApp
{
    public partial class CatalogWindow : Window
    {
        private readonly ChioriContext _context = new();

        public CatalogWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts(string search = "")
        {
            wrapProducts.Children.Clear();
            var products = _context.Products
                .Where(p => string.IsNullOrEmpty(search) || p.Name.Contains(search))
                .ToList();

            foreach (var p in products)
            {
                var card = new Border
                {
                    Margin = new Thickness(10),
                    Padding = new Thickness(10),
                    Background = System.Windows.Media.Brushes.White,
                    BorderBrush = System.Windows.Media.Brushes.LightGray,
                    BorderThickness = new Thickness(1)
                };

                var stack = new StackPanel();
                stack.Children.Add(new TextBlock { Text = p.Name, FontWeight = FontWeights.Bold });
                stack.Children.Add(new TextBlock { Text = $"Артикул: {p.Article}" });
                stack.Children.Add(new TextBlock { Text = $"Цена: {p.RecommendedPrice:C}" });

                var btn = new Button { Content = "В корзину", Tag = p.ProductId };
                btn.Click += AddToCart_Click;

                stack.Children.Add(btn);
                card.Child = stack;
                wrapProducts.Children.Add(card);
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag is not int productId)
                return;

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                MessageBox.Show("Товар не найден.");
                return;
            }

            var item = new BasketItem
            {
                ProductId = product.ProductId,
                ProductName = product.Name,
                Price = product.RecommendedPrice,
                Quantity = 1
            };

            BasketService.AddItem(item);
            MessageBox.Show($"«{product.Name}» добавлен в корзину!");
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new MainWindow().Show();
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LoadProducts(txtSearch.Text);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts(txtSearch.Text);
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            if (!BasketService.Items.Any())
            {
                MessageBox.Show("Корзина пуста.");
                return;
            }

            this.Hide();
            new CheckoutWindow(AppUser.Current?.UserId ?? 0).Show();
        }
        private void BtnCabinet_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new PersonalCabinetWindow().Show();
        }
    }
}