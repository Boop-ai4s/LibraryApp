using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LibraryApp.Models
{

    namespace LibraryApp
    {
        public enum ReadingStatus
        {
            InPlans,
            Reading,
            Read
        }

        public class Genre
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Author
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Country { get; set; }
        }

        public class Book
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int AuthorId { get; set; }
            public int GenreId { get; set; }
            public int Year { get; set; }

            public Author Author { get; set; }
            public Genre Genre { get; set; }
        }

        public class User
        {
            public int Id { get; set; }
            public string Login { get; set; }          // <=16 символов
            public string Email { get; set; }          // должен содержать @
            public string Password { get; set; }       // <=12 символов
            public bool IsAdmin { get; set; }

            // список книг, добавленных пользователем (для читателя)
            public List<UserBook> UserBooks { get; set; } = new List<UserBook>();
        }

        // книга пользователя связь пользователя с книгой + статус
        public class UserBook
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int BookId { get; set; }
            public ReadingStatus Status { get; set; }
            public int? Rating { get; set; } // 0-5 или отсутствие
            public string Comment { get; set; }

            // свойства заполняются
            public Book Book { get; set; }
            public User User { get; set; }
        }
    }
}
