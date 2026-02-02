using ChioriApp.Models;
using ChioriApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ChioriApp
{
    public partial class AdminWindow : Window
    {
        private readonly ChioriContext _context = new();

        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var adminRoleId = _context.Roles.First(r => r.RoleName == "Администратор").RoleId;

            var users = _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId != adminRoleId)
                .ToList();

            var customers = _context.Customers
                .Where(c => c.UserId != 0)
                .ToList();

            var dict = customers.ToDictionary(c => c.UserId);

            foreach (var user in users)
            {
                if (dict.TryGetValue(user.UserId, out var cust))
                {
                    user.Customer = cust;
                }
            }

            dgUsers.ItemsSource = users;
        }

        private void DgUsers_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                this.Hide();
                var editWindow = new UserEditWindow(user);
                bool? result = editWindow.ShowDialog();
                this.Show();
                if (result == true)
                {
                    LoadUsers();
                }
            }
        }

        private void MenuEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                this.Hide();
                var editWindow = new UserEditWindow(user);
                bool? result = editWindow.ShowDialog();
                this.Show();
                if (result == true)
                {
                    LoadUsers();
                }
            }
        }

        private void MenuDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                if (MessageBox.Show($"Удалить пользователя:\n{user.Username} ({user.Email})?\n\nБудут удалены все заказы клиента.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (user.Customer != null)
                        {
                            var customerId = user.Customer.CustomerId;
                            var orders = _context.Orders
                                .Where(o => o.CustomerId == customerId)
                                .Include(o => o.OrderItems)
                                .ToList();

                            foreach (var order in orders)
                            {
                                _context.OrderItems.RemoveRange(order.OrderItems);
                                _context.Orders.Remove(order);
                            }
                        }

                        if (user.Customer != null)
                        {
                            _context.Customers.Remove(user.Customer);
                        }

                        _context.Users.Remove(user);
                        _context.SaveChanges();

                        MessageBox.Show("Пользователь удалён.");
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении:\n{ex.Message}");
                    }
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new MainWindow().Show();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}