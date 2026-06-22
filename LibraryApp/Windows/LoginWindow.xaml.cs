using LibraryApp.Models;
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

namespace LibraryApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool _isAdmin;

        public LoginWindow(bool isAdmin)
        {
            InitializeComponent();
            _isAdmin = isAdmin;
            Title = _isAdmin ? "Авторизация администратора" : "Авторизация читателя";
        }

        private void LoginBox_TextChanged(object sender, RoutedEventArgs e)
        {
            bool allFilled = !string.IsNullOrWhiteSpace(LoginBox.Text) &&
                             !string.IsNullOrWhiteSpace(EmailBox.Text) &&
                             PasswordBox.Password.Length > 0;
            LoginButton.IsEnabled = allFilled;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password;

            string error = null;
            if (login.Length > 16)
                error = "Логин должен содержать 16 символов или меньше.";
            else if (!email.Contains("@"))
                error = "Email должен содержать символ '@'.";
            else if (password.Length > 12)
                error = "Пароль должен содержать 12 символов или меньше.";

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Поиск пользователя
            var user = DataStore.GetUserByLogin(login);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.Email != email || user.Password != password)
            {
                MessageBox.Show("Неверный email или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка роли
            if (user.IsAdmin != _isAdmin)
            {
                MessageBox.Show($"Вы выбрали роль '{(_isAdmin ? "Администратор" : "Читатель")}', но у этого пользователя другая роль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataStore.CurrentUser = user;

            if (_isAdmin)
            {
                var catalog = new AdminCatalogWindow();
                catalog.Show();
            }
            else
            {
                var catalog = new ReaderCatalogWindow();
                catalog.Show();
            }
            this.Close();
        }
    }
}