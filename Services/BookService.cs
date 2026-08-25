using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Services.Specifications;
using ServicesAbstractions;
using Shared;
using Shared.Dtos.Book;
using System.Linq.Expressions;

namespace Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public BookService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<string>> CreateBookAsync(CreateBookDto dto)
        {
            var isbnSpec = new GeneralSpecifications<Book>(b => b.ISBN == dto.ISBN);
            var existingBook = await _unitOfWork.GetRepository<Book>().GetAsync(isbnSpec);

            if (existingBook is not null)
                throw new BadRequestException($"Book with ISBN '{dto.ISBN}' already exists");

            await ValidateLanguageAsync(dto.LanguageId);
            await ValidatePublisherAsync(dto.PublisherId);
            await ValidateAuthorsExistAsync(dto.AuthorIds);
            await ValidateCategoriesExistAsync(dto.CategoryIds);

            var imageUrls = new List<string>();
            if (dto.Images is not null && dto.Images.Any())
            {
                _fileStorageService.Validate(dto.Images);
                imageUrls = await _fileStorageService.SaveFilesAsync(dto.Images, "book-covers");

            }

            var book = new Book
            {
                ISBN = dto.ISBN,
                Title = dto.Title,
                Edition = dto.Edition,
                Summary = dto.Summary,
                PublicationYear = dto.PublicationYear,
                ImageUrls = imageUrls,
                LanguageId = dto.LanguageId,
                PublisherId = dto.PublisherId,
                Status = (Domain.Enums.BookStatus)dto.Status,
                BookAuthors = dto.AuthorIds.Select(a => new BookAuthor { AuthorId = a }).ToList(),
                BookCategories = dto.CategoryIds.Select(c => new BookCategory { CategoryId = c }).ToList(),
                
            };

            _unitOfWork.GetRepository<Book>().Add(book);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Book created successfully"
            };
        }

        public async Task<ApiResponse<string>> UpdateBookAsync(int bookId, UpdateBookDto dto)
        {
            var spec = new GeneralSpecifications<Book>(b => b.Id == bookId);
            var existingBook = await _unitOfWork.GetRepository<Book>().GetAsync(spec);

            if (existingBook is null) throw new NotFoundException($"Book with id {bookId} not found");

            if (!string.IsNullOrWhiteSpace(dto.ISBN) && dto.ISBN != existingBook.ISBN)
            {
                var isbnSpec = new GeneralSpecifications<Book>(b => b.ISBN == dto.ISBN && b.Id != bookId);
                var isbnConflict = await _unitOfWork.GetRepository<Book>().GetAsync(isbnSpec);
                if (isbnConflict is not null) throw new BadRequestException($"Book with ISBN '{dto.ISBN}' already exists");
            }

            if (dto.LanguageId.HasValue) await ValidateLanguageAsync(dto.LanguageId.Value);
            if (dto.PublisherId.HasValue) await ValidatePublisherAsync(dto.PublisherId.Value);
            if (dto.AuthorIds is not null) await ValidateAuthorsExistAsync(dto.AuthorIds);
            if (dto.CategoryIds is not null) await ValidateCategoriesExistAsync(dto.CategoryIds);


            if(dto.Images is not null && dto.Images.Any())
            {
                _fileStorageService.Validate(dto.Images);
                _fileStorageService.DeleteFiles(existingBook.ImageUrls);
                var newImageUrls = await _fileStorageService.SaveFilesAsync(dto.Images, "book-covers");
                existingBook.ImageUrls.Clear();
                existingBook.ImageUrls.AddRange(newImageUrls);
            };

            existingBook.ISBN = dto.ISBN ?? existingBook.ISBN;
            existingBook.Title = dto.Title ?? existingBook.Title;
            existingBook.Edition = dto.Edition ?? existingBook.Edition;
            existingBook.Summary = dto.Summary ?? existingBook.Summary;
            existingBook.PublicationYear = dto.PublicationYear ?? existingBook.PublicationYear;
            existingBook.Status = (Domain.Enums.BookStatus)(dto.Status ?? (Shared.Enums.BookStatus)existingBook.Status);
            existingBook.LanguageId = dto.LanguageId ?? existingBook.LanguageId;
            existingBook.PublisherId = dto.PublisherId ?? existingBook.PublisherId;

            _unitOfWork.GetRepository<Book>().Update(existingBook);

            if (dto.AuthorIds is not null)
                await ReplaceBookAuthorsAsync(bookId, dto.AuthorIds);

            if (dto.CategoryIds is not null)
                await ReplaceBookCategoriesAsync(bookId, dto.CategoryIds);

            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Book updated successfully"
            };
        }

        public async Task<ApiResponse<string>> DeleteBookAsync(int bookId)
        {
            var spec = new GeneralSpecifications<Book>(b => b.Id == bookId);
            var existingBook = await _unitOfWork.GetRepository<Book>().GetAsync(spec);

            if (existingBook is null) throw new NotFoundException($"Book with id {bookId} not found");

            if(existingBook.ImageUrls is not null && existingBook.ImageUrls.Any())
            {
                _fileStorageService.DeleteFiles(existingBook.ImageUrls);
            }

            _unitOfWork.GetRepository<Book>().Delete(existingBook);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<string>
            {
                Data = string.Empty,
                Success = true,
                Message = "Book deleted successfully"
            };
        }

        public async Task<ApiResponse<BookResponseDto>> GetBookByIdAsync(int bookId)
        {
            var spec = new GeneralSpecifications<Book>(b => b.Id == bookId);
            var book = await _unitOfWork.GetRepository<Book>().GetAsync(spec, BookSelector);

            if (book is null) throw new NotFoundException($"Book with id {bookId} not found");

            book.CoverImageUrls = book.CoverImageUrls.Select(_fileStorageService.BuildAbsoluteUrl).ToList();

            return new ApiResponse<BookResponseDto>
            {
                Data = book,
                Success = true,
                Message = "Book retrieved successfully"
            };
        }

        public async Task<ApiResponse<PaginatedResponse<BookResponseDto>>> GetAllBooksAsync(BookSearchFilterDto filterDto)
        {


            Expression<Func<Book, bool>> criteria = b =>
                (string.IsNullOrWhiteSpace(filterDto.SearchTerm) ||
                    b.Title.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()) ||
                    b.ISBN.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()) ||
                    b.BookAuthors.Any(ba => ba.Author.FullName.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim())) ||
                    b.BookCategories.Any(bc => bc.Category.Name.ToLower().Trim().Contains(filterDto.SearchTerm.ToLower().Trim()))) &&
                (!filterDto.LanguageId.HasValue || b.LanguageId == filterDto.LanguageId) &&
                (!filterDto.CategoryId.HasValue || b.BookCategories.Any(bc => bc.CategoryId == filterDto.CategoryId)) &&
                (!filterDto.AuthorId.HasValue || b.BookAuthors.Any(ba => ba.AuthorId == filterDto.AuthorId));

            var spec = new GeneralSpecifications<Book>(criteria, filterDto.PageNumber, filterDto.PageSize);
            var books = await _unitOfWork.GetRepository<Book>().GetAllAsync(spec, BookSelector);

            //build image urls for each book
            foreach (var book in books)
                book.CoverImageUrls = book.CoverImageUrls.Select(_fileStorageService.BuildAbsoluteUrl).ToList();

            var countSpec = new GeneralSpecifications<Book>(criteria);
            var totalCount = await _unitOfWork.GetRepository<Book>().CountAsync(countSpec);

            return new ApiResponse<PaginatedResponse<BookResponseDto>>
            {
                Data = new PaginatedResponse<BookResponseDto>(filterDto.PageNumber, filterDto.PageSize, totalCount, books),
                Success = true,
                Message = "Books retrieved successfully"
            };
        }

        private static readonly Expression<Func<Book, BookResponseDto>> BookSelector = b => new BookResponseDto
        {
            Id = b.Id,
            ISBN = b.ISBN,
            Title = b.Title,
            Edition = b.Edition,
            Summary = b.Summary,
            PublicationYear = b.PublicationYear,
            CoverImageUrls = b.ImageUrls,
            Status = b.Status.ToString(),
            LanguageId = b.LanguageId,
            LanguageName = b.Language.Name,
            PublisherId = b.PublisherId,
            PublisherName = b.Publisher.Name,
            Authors = b.BookAuthors.Select(ba => new BookAuthorDto { Id = ba.Author.Id, Name = ba.Author.FullName }).ToList(),
            Categories = b.BookCategories.Select(bc => new BookCategoryDto { Id = bc.Category.Id, Name = bc.Category.Name }).ToList()
        };

        private async Task ValidateLanguageAsync(int languageId)
        {
            var spec = new GeneralSpecifications<Language>(l => l.Id == languageId);
            var language = await _unitOfWork.GetRepository<Language>().GetAsync(spec);
            if (language is null) throw new NotFoundException($"Language with id {languageId} not found");
        }

        private async Task ValidatePublisherAsync(int publisherId)
        {
            var spec = new GeneralSpecifications<Publisher>(p => p.Id == publisherId);
            var publisher = await _unitOfWork.GetRepository<Publisher>().GetAsync(spec);
            if (publisher is null) throw new NotFoundException($"Publisher with id {publisherId} not found");
        }

        private async Task ValidateAuthorsExistAsync(List<int> authorIds)
        {
            if (authorIds is null || !authorIds.Any()) return;

            var distinctIds = authorIds.Distinct().ToList();
            var spec = new GeneralSpecifications<Author>(a => distinctIds.Contains(a.Id));
            var count = await _unitOfWork.GetRepository<Author>().CountAsync(spec);

            if (count != distinctIds.Count)
                throw new BadRequestException("One or more authors do not exist");
        }

        private async Task ValidateCategoriesExistAsync(List<int> categoryIds)
        {
            if (categoryIds is null || !categoryIds.Any()) return;

            var distinctIds = categoryIds.Distinct().ToList();
            var spec = new GeneralSpecifications<Category>(c => distinctIds.Contains(c.Id));
            var count = await _unitOfWork.GetRepository<Category>().CountAsync(spec);

            if (count != distinctIds.Count)
                throw new BadRequestException("One or more categories do not exist");
        }

        private async Task ReplaceBookAuthorsAsync(int bookId, List<int> authorIds)
        {
            var spec = new GeneralSpecifications<BookAuthor>(ba => ba.BookId == bookId);
            var currentLinks = await _unitOfWork.GetRepository<BookAuthor>().GetAllAsync(spec);

            _unitOfWork.GetRepository<BookAuthor>().DeleteRange(currentLinks);

            _unitOfWork.GetRepository<BookAuthor>().AddRange(authorIds.Distinct().Select(authorId => new BookAuthor { BookId = bookId, AuthorId = authorId }));
        }

        private async Task ReplaceBookCategoriesAsync(int bookId, List<int> categoryIds)
        {
            var spec = new GeneralSpecifications<BookCategory>(bc => bc.BookId == bookId);
            var currentLinks = await _unitOfWork.GetRepository<BookCategory>().GetAllAsync(spec);

            _unitOfWork.GetRepository<BookCategory>().DeleteRange(currentLinks);

            _unitOfWork.GetRepository<BookCategory>().AddRange(categoryIds.Distinct().Select(categoryId => new BookCategory { BookId = bookId, CategoryId = categoryId }));
        }
    }
}
