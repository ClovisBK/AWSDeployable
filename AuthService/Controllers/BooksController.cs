using AuthService.DTOs.BookDtos;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound("Book not found");
            return Ok(book);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest("Search term cannot be empty");

            var books = await _bookService.SearchAsync(searchTerm);
            return Ok(books);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var books = await _bookService.GetByCategoryAsync(categoryId);
            return Ok(books);
        }
        [HttpPost]
        [Authorize(Roles ="Librarian")]
        public async Task<IActionResult> Create([FromBody] BookDto dto)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            var book = await _bookService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new {id = book.Id}, book);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Update(int id, [FromForm] BookDto book)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _bookService.UpdateAsync(id, book);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteAsync(id);
            return NoContent();
        }
    }
}
