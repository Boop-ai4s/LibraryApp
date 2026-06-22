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
    /// Логика взаимодействия для ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        private Book _book;
        private bool _isAdmin;

        public ReviewsWindow(Book book, bool isAdmin)
        {
            InitializeComponent();
            _book = book;
            _isAdmin = isAdmin;
            DataContext = this; // для привязки Book
            LoadReviews();
        }

        public Book Book => _book;

        private void LoadReviews()
        {
            // Все отзывы для этой книги, у которых есть комментарий или оценка
            var reviews = DataStore.UserBooks
                .Where(ub => ub.BookId == _book.Id && (ub.Comment != null || ub.Rating.HasValue))
                .Select(ub => new ReviewViewModel
                {
                    User = ub.User,
                    Comment = ub.Comment,
                    Rating = ub.Rating,
                    UserBook = ub,
                    IsAdminVisible = _isAdmin ? Visibility.Visible : Visibility.Collapsed
                })
                .ToList();

            ReviewsListBox.ItemsSource = reviews;
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            var user = DataStore.CurrentUser;
            if (user == null) return;

            var userBook = DataStore.UserBooks.FirstOrDefault(ub => ub.UserId == user.Id && ub.BookId == _book.Id);
            if (userBook == null)
            {
                userBook = new UserBook
                {
                    UserId = user.Id,
                    BookId = _book.Id,
                    Book = _book,
                    Status = ReadingStatus.InPlans,
                    Rating = null,
                    Comment = null
                };
                DataStore.AddUserBook(userBook);
            }

            // Диалог ввода комментария
            string comment = Microsoft.VisualBasic.Interaction.InputBox("Введите комментарий к книге:", "Комментарий", userBook.Comment ?? "");
            if (!string.IsNullOrWhiteSpace(comment))
            {
                userBook.Comment = comment;
                LoadReviews();
                MessageBox.Show("Комментарий сохранён.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void DeleteReview_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAdmin)
            {
                MessageBox.Show("У вас нет прав на удаление.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var button = sender as Button;
            var vm = button?.Tag as ReviewViewModel;
            if (vm == null) return;

            if (MessageBox.Show("Удалить этот комментарий и оценку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var userBook = vm.UserBook;
                userBook.Comment = null;
                userBook.Rating = null;
                LoadReviews();
                MessageBox.Show("Отзыв удалён.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        // Вспомогательный класс для отображения
        public class ReviewViewModel
        {
            public User User { get; set; }
            public string Comment { get; set; }
            public int? Rating { get; set; }
            public UserBook UserBook { get; set; }
            public Visibility IsAdminVisible { get; set; }
        }
    }
}
