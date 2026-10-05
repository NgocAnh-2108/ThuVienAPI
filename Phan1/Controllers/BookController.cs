using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phan1.CustomActionFilter;
using Phan1.Data;
using Phan1.Models.DTO;
using Phan1.Repositories;
using System.Text.Json;

namespace Phan1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookController> _logger;
        public BookController(AppDbContext dbContext, IBookRepository bookRepository, ILogger<BookController> logger)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
            _logger = logger;
        }
        [HttpGet("get-all-books")]
        [Authorize(Roles = "Read")]
        public IActionResult GetAllBooks([FromQuery] string? filterOn, [FromQuery] string? filterQuery , [FromQuery] string? sortBy, [FromQuery] bool isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            _logger.LogInformation("GetAll Book Action method was invoked");
            _logger.LogWarning("This is a warning log");
            _logger.LogError("This is a error log");
            var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);

            _logger.LogInformation($"Finished GetAllBook request with data { JsonSerializer.Serialize(allBooks)}");
            return Ok(allBooks);
        }

        [HttpGet]
        [Route("get-book-by-id/{id:int}")]
        [Authorize(Roles = "Read")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            return Ok(bookWithIdDTO);
        }
        [HttpPost("add-book")]
        [ValidateModel]
        [Authorize(Roles = "Write")]
        public IActionResult AddBook([FromBody] addBookRequestDTO addBookRequestDTO)
        {
            if (ValidateAddBook(addBookRequestDTO))
            {
                var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
                return Ok(bookAdd);
            }
            return BadRequest(ModelState);

        }

        

        [HttpPut("update-book-by-id/{id:int}")]
        [Authorize(Roles = "Write")]

        public IActionResult UpdateBookById(int id, [FromBody] addBookRequestDTO bookDTO)
        {
            var bookExists = _dbContext.Books.Any(x => x.Id == id);

            if (!bookExists)
            {
                return NotFound(new
                {
                    message = "Book không tồn tại"
                });
            }

            if (!ValidateAddBook(bookDTO))
            {
                return BadRequest(ModelState);
            }

            var updateBook = _bookRepository.UpdateBookById(id, bookDTO);

            return Ok(updateBook);
        }
        [HttpDelete("delete-book-by-id/{id:int}")]
        [Authorize(Roles = "Write")]
        public IActionResult DeleteBookById(int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            return Ok(deleteBook);
        }
        private bool ValidateAddBook(addBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO), $"Book data is required.");
                return false;
            }
            if (string.IsNullOrEmpty(addBookRequestDTO.Description))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Description), $"{nameof(addBookRequestDTO.Description)} cannot be null");
            }
            // kiem tra rating (0,5) 
            if (addBookRequestDTO.Rate < 0 || addBookRequestDTO.Rate > 5)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Rate), $"{nameof(addBookRequestDTO.Rate)} cannot be less than 0 and more than 5");
            }
            var publisherExists = _dbContext.Publishers.Any(x => x.Id == addBookRequestDTO.PublisherId);

            if (!publisherExists)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherId),"PublisherId không tồn tại");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }
    }
}
