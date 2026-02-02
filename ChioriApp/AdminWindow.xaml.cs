using ChioriApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

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
            dgUsers.ItemsSource = _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId != adminRoleId)
                .ToList();
        }

        private void DgUsers_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem is Models.User user)
            {
                this.Hide();
                new UserEditWindow(user).Show();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new MainWindow().Show();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}