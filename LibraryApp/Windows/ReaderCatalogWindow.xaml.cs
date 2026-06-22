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
    /// Логика взаимодействия для ReaderCatalogWindow.xaml
    /// </summary>
    public partial class ReaderCatalogWindow : Window
    {
        public ReaderCatalogWindow()
        {
            InitializeComponent();
            RefreshBooks();
        }

        private void RefreshBooks()
        {
            BooksListBox.ItemsSource = DataStore.Books.ToList();
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
                query = query.Where(b => b.Title.ToLower().Contains(keyword));
            }

            BooksListBox.ItemsSource = query.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();
        private void Search_Click(object sender, RoutedEventArgs e) => ApplyFilter();

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

        private void MyBooks_Click(object sender, RoutedEventArgs e)
        {
            var myBooksWindow = new MyBooksWindow();
            myBooksWindow.ShowDialog();
            RefreshBooks();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            DataStore.CurrentUser = null;
            var roleWindow = new RoleSelectionWindow();
            roleWindow.Show();
            this.Close();
        }

        private void ShowReviews_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var book = button?.Tag as Book;
            if (book == null) return;

            var reviewsWindow = new ReviewsWindow(book, isAdmin: false);
            reviewsWindow.ShowDialog();
        }
    }
}
