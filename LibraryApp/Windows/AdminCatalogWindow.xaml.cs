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
    /// Логика взаимодействия для AdminCatalogWindow.xaml
    /// </summary>
    public partial class AdminCatalogWindow : Window
    {
        private List<Book> _allBooks;

        public AdminCatalogWindow()
        {
            InitializeComponent();
            _allBooks = DataStore.Books.ToList();
            BooksListBox.ItemsSource = _allBooks;
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddBookWindow();
            addWindow.ShowDialog();
            RefreshBooks();
        }

        private void ShowUsers_Click(object sender, RoutedEventArgs e)
        {
            var userList = string.Join("\n", DataStore.Users.Select(u => $"{u.Login} ({u.Email}) - {(u.IsAdmin ? "Админ" : "Читатель")}"));
            MessageBox.Show(userList, "Список пользователей", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            DataStore.CurrentUser = null;
            var roleWindow = new RoleSelectionWindow();
            roleWindow.Show();
            this.Close();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string filter = (FilterCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
            string keyword = SearchBox.Text.Trim().ToLower();

            var query = DataStore.Books.AsEnumerable();

            if (filter == "По автору" && !string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => b.Author.FullName.ToLower().Contains(keyword));
            }
            else if (filter == "По жанру" && !string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => b.Genre.Name.ToLower().Contains(keyword));
            }
            else if (filter == "По алфавиту")
            {
                query = query.OrderBy(b => b.Title);
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                // По названию
                query = query.Where(b => b.Title.ToLower().Contains(keyword));
            }

            BooksListBox.ItemsSource = query.ToList();
        }

        private void RefreshBooks()
        {
            _allBooks = DataStore.Books.ToList();
            ApplyFilter();
        }

        private void AddToMyBooks_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var book = button?.Tag as Book;
            if (book == null) return;

            var user = DataStore.CurrentUser;
            if (user == null) return;

            if (user.UserBooks.Any(ub => ub.BookId == book.Id))
            {
                MessageBox.Show("Эта книга уже в вашем списке.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var userBook = new UserBook
            {
                UserId = user.Id,
                BookId = book.Id,
                Book = book,
                Status = ReadingStatus.InPlans,
                Rating = null,
                Comment = null
            };
            DataStore.AddUserBook(userBook);
            MessageBox.Show("Книга добавлена в ваш список.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowReviews_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var book = button?.Tag as Book;
            if (book == null) return;

            var reviewsWindow = new ReviewsWindow(book, isAdmin: true);
            reviewsWindow.ShowDialog();
        }
    }
}
