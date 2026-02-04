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
            try
            {
                var adminRoleId = _context.Roles.First(r => r.RoleName == "Администратор").RoleId;

                var users = _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.RoleId != adminRoleId)
                    .ToList();

                var customers = _context.Customers
                    .Where(c => c.UserId != 0)
                    .ToDictionary(c => c.UserId);

                var items = new List<AdminUserItem>();

                foreach (var user in users)
                {
                    var cust = customers.TryGetValue(user.UserId, out var c) ? c : null;

                    items.Add(new AdminUserItem
                    {
                        ID = user.UserId,
                        Логин = user.Username,
                        Email = user.Email,
                        Телефон = user.Phone ?? "",
                        Имя = cust?.FirstName ?? "",
                        Фамилия = cust?.LastName ?? "",
                        Отчество = cust?.Patronymic ?? "",
                        Роль = user.Role.RoleName,
                        Статус = user.AccountStatusId == 1 ? "Активен" : "Заблокирован",
                        Дата_регистрации = user.Created_at
                    });
                }

                dgUsers.ItemsSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}");
            }
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
            if (dgUsers.SelectedItem is AdminUserItem item)
            {
                var user = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.UserId == item.ID);

                if (user != null)
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
                else
                {
                    MessageBox.Show("Пользователь не найден.");
                }
            }
        }

        private void MenuDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is AdminUserItem item)
            {
                try
                {
                    using var transaction = _context.Database.BeginTransaction();

                    var user = _context.Users.Find(item.ID);
                    if (user == null)
                    {
                        MessageBox.Show("Пользователь не найден.");
                        return;
                    }

                    var customer = _context.Customers.FirstOrDefault(c => c.UserId == user.UserId);
                    if (customer != null)
                    {

                        var orders = _context.Orders.Where(o => o.CustomerId == customer.CustomerId).ToList();
                        foreach (var order in orders)
                        {
                            _context.OrderItems.RemoveRange(_context.OrderItems.Where(oi => oi.OrderId == order.OrderId));
                            _context.Orders.Remove(order);
                        }

                        _context.Customers.Remove(customer);
                    }

                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    transaction.Commit();

                    MessageBox.Show("Пользователь удалён.");
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}");
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