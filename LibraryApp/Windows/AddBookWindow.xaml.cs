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
    /// Логика взаимодействия для AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        public AddBookWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string authorName = AuthorBox.Text.Trim();
            string genreName = GenreBox.Text.Trim();

            if (string.IsNullOrEmpty(title)  || string.IsNullOrEmpty(authorName) || string.IsNullOrEmpty(genreName))
            {
                MessageBox.Show("Все поля обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка, есть ли уже автор с таким именем
            var author = DataStore.Authors.FirstOrDefault(a => a.FullName.Equals(authorName, StringComparison.OrdinalIgnoreCase));
            if (author == null)
            {
                author = new Author { FullName = authorName, Country = "Неизвестно" };
                DataStore.AddAuthor(author);
            }

            // Проверка жанра
            var genre = DataStore.Genres.FirstOrDefault(g => g.Name.Equals(genreName, StringComparison.OrdinalIgnoreCase));
            if (genre == null)
            {
                genre = new Genre { Name = genreName };
                DataStore.AddGenre(genre);
            }

            // Создание книги
            var book = new Book
            {
                Title = title,
                AuthorId = author.Id,
                GenreId = genre.Id,
                Year = 2000,
                Author = author,
                Genre = genre
            };
            DataStore.AddBook(book);

            MessageBox.Show("Книга успешно добавлена в каталог.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
