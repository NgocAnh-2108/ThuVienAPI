using System.ComponentModel.DataAnnotations;

namespace Phan1.Models.Domain
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public List<Book_Authors> Book_Authors { get; set; }
    }
}
