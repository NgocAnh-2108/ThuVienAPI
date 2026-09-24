using System.ComponentModel.DataAnnotations;

namespace Phan1.Models.Domain
{
    public class Book_Authors
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}