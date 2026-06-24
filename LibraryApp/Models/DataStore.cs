using LibraryApp.Models.LibraryApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LibraryApp.Models
{
    public static class DataStore
    {
        private static int _nextId = 1;

        public static List<User> Users { get; private set; }
        public static List<Book> Books { get; private set; }
        public static List<Author> Authors { get; private set; }
        public static List<Genre> Genres { get; private set; }
        public static List<UserBook> UserBooks { get; private set; }

        // авторизованный пользователь сейчас
        public static User CurrentUser { get; set; }

        static DataStore()
        {
            Users = new List<User>();
            Authors = new List<Author>();
            Genres = new List<Genre>();
            Books = new List<Book>();
            UserBooks = new List<UserBook>();

            // жанры
            var genre1 = new Genre { Id = GetNextId(), Name = "Программирование" };
            var genre2 = new Genre { Id = GetNextId(), Name = "Базы данных" };
            var genre3 = new Genre { Id = GetNextId(), Name = "Иностранный язык" };
            Genres.AddRange(new[] { genre1, genre2, genre3 });

            // авторы
            var author1 = new Author { Id = GetNextId(), FullName = "В. В. Трофимов, Т.А Павловская", Country = "Россия" };
            var author2 = new Author { Id = GetNextId(), FullName = "В. М. Илюшечкин", Country = "Россия" };
            var author3 = new Author { Id = GetNextId(), FullName = "О. Алейникова", Country = "Россия" };
            Authors.AddRange(new[] { author1, author2, author3 });

            // книги
            Books.Add(new Book { Id = GetNextId(), Title = "Алгоритмитизация и программирование", AuthorId = author1.Id, GenreId = genre1.Id, Author = author1, Genre = genre1 });
            Books.Add(new Book { Id = GetNextId(), Title = "Основы использования и проектирования баз данных", AuthorId = author2.Id, GenreId = genre2.Id, Year = 1866, Author = author2, Genre = genre2 });
            Books.Add(new Book { Id = GetNextId(), Title = "Английский язык для технических специальностей", AuthorId = author3.Id, GenreId = genre3.Id, Year = 1949, Author = author3, Genre = genre3 });

            // пользователи админ и читатель
            var admin = new User { Id = GetNextId(), Login = "admin", Email = "admin@mail.com", Password = "admin123", IsAdmin = true };
            var reader = new User { Id = GetNextId(), Login = "reader", Email = "reader@mail.com", Password = "reader123", IsAdmin = false };
            Users.AddRange(new[] { admin, reader });

            // несколько книг в список читателя
            var userBook = new UserBook
            {
                Id = GetNextId(),
                UserId = reader.Id,
                BookId = Books[0].Id,
                Book = Books[0],
                Status = ReadingStatus.Read,
                Rating = 5,
                Comment = "Помогло в изучении программирования."
            };
            UserBooks.Add(userBook);
            reader.UserBooks.Add(userBook);
        }

        private static int GetNextId() => _nextId++;

        public static void AddBook(Book book)
        {
            book.Id = GetNextId();
            Books.Add(book);
        }

        public static void AddAuthor(Author author)
        {
            author.Id = GetNextId();
            Authors.Add(author);
        }

        public static void AddGenre(Genre genre)
        {
            genre.Id = GetNextId();
            Genres.Add(genre);
        }

        public static void AddUserBook(UserBook userBook)
        {
            userBook.Id = GetNextId();
            UserBooks.Add(userBook);
            var user = Users.FirstOrDefault(u => u.Id == userBook.UserId);
            if (user != null) user.UserBooks.Add(userBook);
        }

        public static void RemoveUserBook(UserBook userBook)
        {
            UserBooks.Remove(userBook);
            var user = Users.FirstOrDefault(u => u.Id == userBook.UserId);
            if (user != null) user.UserBooks.Remove(userBook);
        }

        public static User GetUserByLogin(string login) => Users.FirstOrDefault(u => u.Login == login);
    }
}
