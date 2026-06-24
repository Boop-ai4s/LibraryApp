using LibraryApp.Models;
using LibraryApp.Models.LibraryApp;
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
    /// Логика взаимодействия для MyBooksWindow.xaml
    /// </summary>
    public partial class MyBooksWindow : Window
    {
        public MyBooksWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var user = DataStore.CurrentUser;
            if (user == null)
            {
                MessageBox.Show("Пользователь не авторизован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            // Загрузка книги пользователя
            MyBooksListBox.ItemsSource = user.UserBooks.ToList();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadData();

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveBook_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var userBook = button?.Tag as UserBook;
            if (userBook == null) return;

            if (MessageBox.Show("Удалить книгу из вашего списка?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DataStore.RemoveUserBook(userBook);
                LoadData();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            var userBook = combo?.DataContext as UserBook;
            if (userBook == null) return;
            var selectedItem = combo.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;
            switch (selectedItem.Content.ToString())
            {
                case "В планах": userBook.Status = ReadingStatus.InPlans; break;
                case "Читаю": userBook.Status = ReadingStatus.Reading; break;
                case "Прочитано": userBook.Status = ReadingStatus.Read; break;
            }
        }

        private void AddRating_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var userBook = button?.Tag as UserBook;
            if (userBook == null) return;

            // диалог ввода
            string input = Microsoft.VisualBasic.Interaction.InputBox("Введите оценку (0-5):", "Оценка", userBook.Rating?.ToString() ?? "");
            if (int.TryParse(input, out int rating) && rating >= 0 && rating <= 5)
            {
                userBook.Rating = rating;
                LoadData();
                MessageBox.Show("Оценка сохранена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Оценка должна быть числом от 0 до 5.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var userBook = button?.Tag as UserBook;
            if (userBook == null) return;

            string comment = Microsoft.VisualBasic.Interaction.InputBox("Введите комментарий:", "Комментарий", userBook.Comment ?? "");
            if (!string.IsNullOrWhiteSpace(comment))
            {
                userBook.Comment = comment;
                LoadData();
                MessageBox.Show("Комментарий сохранён.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
