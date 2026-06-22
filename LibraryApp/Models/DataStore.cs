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
            var genre1 = new Genre { Id = GetNextId(), Name = "Роман" };
            var genre2 = new Genre { Id = GetNextId(), Name = "Фантастика" };
            var genre3 = new Genre { Id = GetNextId(), Name = "Детектив" };
            Genres.AddRange(new[] { genre1, genre2, genre3 });

            // авторы
            var author1 = new Author { Id = GetNextId(), FullName = "Лев Толстой", Country = "Россия" };
            var author2 = new Author { Id = GetNextId(), FullName = "Фёдор Достоевский", Country = "Россия" };
            var author3 = new Author { Id = GetNextId(), FullName = "Джордж Оруэлл", Country = "Великобритания" };
            Authors.AddRange(new[] { author1, author2, author3 });

            // книги
            Books.Add(new Book { Id = GetNextId(), Title = "Война и мир", AuthorId = author1.Id, GenreId = genre1.Id, Year = 1869, Author = author1, Genre = genre1 });
            Books.Add(new Book { Id = GetNextId(), Title = "Преступление и наказание", AuthorId = author2.Id, GenreId = genre1.Id, Year = 1866, Author = author2, Genre = genre1 });
            Books.Add(new Book { Id = GetNextId(), Title = "1984", AuthorId = author3.Id, GenreId = genre2.Id, Year = 1949, Author = author3, Genre = genre2 });
            Books.Add(new Book { Id = GetNextId(), Title = "Скотный двор", AuthorId = author3.Id, GenreId = genre2.Id, Year = 1945, Author = author3, Genre = genre2 });

            // пользователи админ и читатель
            var admin = new User { Id = GetNextId(), Login = "admin", Email = "admin@lib.com", Password = "admin123", IsAdmin = true };
            var reader = new User { Id = GetNextId(), Login = "reader", Email = "reader@mail.com", Password = "reader123", IsAdmin = false };
            Users.AddRange(new[] { admin, reader });

            // несколько книг в список читателя
            var userBook = new UserBook
            {
                Id = GetNextId(),
                UserId = reader.Id,
                BookId = Books[0].Id,
                Status = ReadingStatus.Read,
                Rating = 5,
                Comment = "Великий роман!"
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
