using System.Security.AccessControl;
using AuthService.DTOs.BookDtos;
using AuthService.Models;
using AuthService.Repositoriess.Interfaces;
using AuthService.Services.Interfaces;

namespace AuthService.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }

        public async Task<Book> CreateAsync(BookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Copies = dto.Copies,
                AvailableCopies = dto.Copies,
                CategoryId = dto.CategoryId
            };
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.CompleteAsync();
            return book;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return false;

            _unitOfWork.Books.Remove(book);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        

        public async Task<IEnumerable<Book>> GetAllAsync()
            => await _unitOfWork.Books.GetAllWithCategoryAsync();

        public async Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId)
            => await _unitOfWork.Books.GetByCategoryAsync(categoryId);

        public async Task<Book?> GetByIdAsync(int id)
         => await _unitOfWork.Books.GetByIdWithCategoryAsync(id);

        public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
            => await _unitOfWork.Books.SearchAsync(searchTerm);

        public async Task<bool> UpdateAsync(int id, BookDto dto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if(book == null) return false;
            book.Title = dto.Title;
            book.Description = dto.Description;
            book.Author = dto.Author;
            book.ImageUrl = dto.ImageUrl;
            book.CategoryId = dto.CategoryId;
            book.ISBN = dto.ISBN;

            int difference = dto.Copies - book.Copies;
            book.Copies = dto.Copies;
            book.AvailableCopies = Math.Max(0, book.AvailableCopies + difference);

            _unitOfWork.Books.Update(book);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
