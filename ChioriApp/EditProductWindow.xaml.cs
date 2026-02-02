using ChioriApp.Models;
using ChioriApp.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ChioriApp
{
    public partial class EditProductWindow : Window
    {
        private readonly ChioriContext _context = new();
        private readonly int _productId;

        public EditProductWindow(int productId)
        {
            InitializeComponent();
            _productId = productId;
            LoadProduct();
        }

        private void LoadProduct()
        {
            var product = _context.Products.Find(_productId);
            if (product == null)
            {
                MessageBox.Show("Товар не найден.");
                Close();
                return;
            }

            txtArticle.Text = product.Article;
            txtName.Text = product.Name;
            txtDescription.Text = product.Description ?? "";
            txtPrice.Text = product.RecommendedPrice.ToString("F2");

            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Tag?.ToString() == product.StatusId.ToString())
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                lblError.Text = "Введите корректную положительную цену.";
                return;
            }

            if (int.TryParse(cmbStatus.SelectedValue?.ToString(), out int statusId))
            {
                var product = _context.Products.Find(_productId);
                if (product != null)
                {
                    product.Name = txtName.Text.Trim();
                    product.Description = txtDescription.Text.Trim();
                    product.RecommendedPrice = price;
                    product.StatusId = statusId;

                    try
                    {
                        _context.SaveChanges();
                        MessageBox.Show("Товар обновлён!");
                        DialogResult = true;
                        Close();
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = $"Ошибка: {ex.Message}";
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить этот товар?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var product = _context.Products.Find(_productId);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                    MessageBox.Show("Товар удалён.");
                    DialogResult = true;
                    Close();
                }
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