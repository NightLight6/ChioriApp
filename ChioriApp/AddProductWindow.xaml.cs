using ChioriApp.Models;
using ChioriApp.Services;
using System.Windows;

namespace ChioriApp
{
    public partial class AddProductWindow : Window
    {
        private readonly ChioriContext _context = new();

        public AddProductWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string article = txtArticle.Text.Trim();
            string name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(article) || string.IsNullOrWhiteSpace(name))
            {
                lblError.Text = "Артикул и название обязательны.";
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                lblError.Text = "Введите корректную положительную цену.";
                return;
            }

            var product = new Product
            {
                Article = article,
                Name = name,
                RecommendedPrice = price,
                CategoryId = 1,
                ManufacturerId = 1,
                SupplierId = 1,
                UnitId = 1,
                StatusId = 1
            };

            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                MessageBox.Show("Товар добавлен!");
                DialogResult = true;
                Hide();
            }
            catch (System.Exception ex)
            {
                lblError.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}