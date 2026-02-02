using ChioriApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChioriApp
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public User? User { get; private set; }
        private List<Role> _roles;

        public AddUserWindow(List<Role> roles)
        {
            InitializeComponent();
            _roles = roles;
            cmbRole.ItemsSource = roles;
            if (roles.Count > 0) cmbRole.SelectedIndex = 0;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var selectedRole = cmbRole.SelectedItem as Role;
            if (selectedRole == null)
            {
                MessageBox.Show("Выберите роль.");
                return;
            }

            User = new User
            {
                Username = txtUsername.Text.Trim(),
                PasswordHash = txtPassword.Password.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                AccountStatusId = 1,
                RoleId = selectedRole.RoleId
            };

            DialogResult = true;
            Hide();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Hide();
        }
    }
}
