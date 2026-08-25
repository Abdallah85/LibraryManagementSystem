using Domain.Contracts;
using Domain.Entities;
using Domain.Exceptions;
using Services.Specifications;
using ServicesAbstractions;
using Shared;
using Shared.Dtos.ActivityLog;
using Shared.Dtos.Borrowing;
using System.Linq.Expressions;

namespace Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActivityLogService _activityLog;

        public BorrowingService(IUnitOfWork unitOfWork, IActivityLogService activityLog)
        {
            _unitOfWork = unitOfWork;
            _activityLog = activityLog;
        }
        public async Task<ApiResponse<string>> BorrowBookAsync(string userId, BorrowBookDto dto)
        {
            if(string.IsNullOrEmpty(userId))
                throw new BadRequestException("User ID cannot be null or empty.");

            var bookSpec = new GeneralSpecifications<Book>(b => b.Id == dto.BookId);
            var book = await _unitOfWork.GetRepository<Book>().GetAsync(bookSpec);

            if (book is null)
                throw new NotFoundException("Book not found");

            var activeBorrowSpec = new GeneralSpecifications<BorrowingTransaction>(x =>
                x.BookId == dto.BookId &&
                (x.Status == Domain.Enums.BorrowingStatus.Pending ||
                x.Status == Domain.Enums.BorrowingStatus.Borrowed));

            var exists =await _unitOfWork.GetRepository<BorrowingTransaction>()
                    .GetAsync(activeBorrowSpec);

            if (exists is not null)
                throw new BadRequestException("Book is not available");

            var dublicateBorrowSpec = new GeneralSpecifications<BorrowingTransaction>(x =>
                x.BookId == dto.BookId &&
                x.UserId == userId &&
                (x.Status == Domain.Enums.BorrowingStatus.Pending ||
                x.Status == Domain.Enums.BorrowingStatus.Borrowed));

            var dublicateExists = await _unitOfWork.GetRepository<BorrowingTransaction>()
                    .GetAsync(dublicateBorrowSpec);

            if (dublicateExists is not null)
                throw new BadRequestException("You have already borrowed this book");

            var transaction = new BorrowingTransaction
            {
                BookId = dto.BookId,
                UserId = userId,
                DueDate = dto.DueDate,
                Status = Domain.Enums.BorrowingStatus.Pending

            };

            _unitOfWork.GetRepository<BorrowingTransaction>().Add(transaction);
            await _unitOfWork.SaveChangesAsync();


            // Log the borrow request activity
            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = userId,
                Action = "Borrow Request",
                EntityAffected = nameof(BorrowingTransaction),
                EntityId = transaction.Id,
                Details = $"Borrow request submitted for book '{transaction.Book.Title}'"
            });

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Borrow request submitted successfully",
                Data = transaction.Id.ToString()
            };

        }

        public async Task<ApiResponse<PaginatedResponse<BorrowingTransactionResponseDto>>> GetAllTransactionsAsync(BorrowingTransactionFilterDto filterDto)
        {

            Expression<Func<BorrowingTransaction,bool>> predicate = x =>
                (string.IsNullOrEmpty(filterDto.UserId) || x.UserId == filterDto.UserId) &&
                (!filterDto.BookId.HasValue || x.BookId == filterDto.BookId.Value) &&
                (!filterDto.Status.HasValue || x.Status == (Domain.Enums.BorrowingStatus)filterDto.Status.Value) &&
                (string.IsNullOrEmpty(filterDto.SearchTerm) || x.Book.Title.Trim().ToLower().Contains(filterDto.SearchTerm.Trim().ToLower()) 
                || x.User.UserName!.Trim().ToLower().Contains(filterDto.SearchTerm.Trim().ToLower()));

            Expression<Func<BorrowingTransaction, BorrowingTransactionResponseDto>> selector = x => new BorrowingTransactionResponseDto
            {
                Id = x.Id,
                BookId = x.BookId,
                BookTitle = x.Book.Title,
                UserId = x.UserId,
                UserName = x.User.UserName!,
                IssuedByUserId = x.IssuedByUserId,
                IssuedByUserName = x.IssuedByUser.UserName,
                BorrowDate = x.BorrowDate,
                DueDate = x.DueDate,
                ReturnDate = x.ReturnDate,
                Status = (Shared.Enums.BorrowingStatus)x.Status
            };

            var spec = new GeneralSpecifications<BorrowingTransaction>(predicate,filterDto.PageNumber, filterDto.PageSize);
            var transactions = await _unitOfWork.GetRepository<BorrowingTransaction>().GetAllAsync(spec,selector);

            var countSpec = new GeneralSpecifications<BorrowingTransaction>(predicate);
            var totalCount = await _unitOfWork.GetRepository<BorrowingTransaction>().CountAsync(countSpec);

            return new ApiResponse<PaginatedResponse<BorrowingTransactionResponseDto>>
            {
                Data = new PaginatedResponse<BorrowingTransactionResponseDto>(filterDto.PageNumber, filterDto.PageSize, totalCount, transactions.ToList()),
                Success = true,
                Message = "Transactions retrieved successfully"
            };
        }

        public async Task<ApiResponse<string>> ReturnBookAsync(int transactionId, string userId)
        {
            var spec = new GeneralSpecifications<BorrowingTransaction>(x => x.Id == transactionId && x.UserId == userId);
            var transaction = await _unitOfWork.GetRepository<BorrowingTransaction>().GetAsync(spec);

            if (transaction is null)
                throw new NotFoundException("Transaction not found");

            if (transaction.Status != Domain.Enums.BorrowingStatus.Borrowed &&
                transaction.Status != Domain.Enums.BorrowingStatus.Overdue)
            {
                throw new BadRequestException(
                    "Only borrowed books can be returned");
            }

            transaction.Status = Domain.Enums.BorrowingStatus.ReturnPending;

            _unitOfWork.GetRepository<BorrowingTransaction>().Update(transaction);

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                UserId = userId,
                Action = "Return Request",
                Details = $"Return request submitted for book '{transaction.Book.Title}'",
                EntityAffected = nameof(BorrowingTransaction),
                EntityId = transaction.Id
            });

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Return request submitted"
            };
        }

        public async Task<ApiResponse<string>> ReviewBorrowRequestAsync(int transactionId, string librarianId, ReviewBorrowRequestDto dto)
        {

            if(string.IsNullOrEmpty(librarianId))
                throw new BadRequestException("Librarian ID cannot be null or empty.");

            var spec = new GeneralSpecifications<BorrowingTransaction>(x => x.Id == transactionId);
            var transaction =await _unitOfWork.GetRepository<BorrowingTransaction>().GetAsync(spec);

            if (transaction is null)
                throw new NotFoundException("Transaction not found");

            if (transaction.Status != Domain.Enums.BorrowingStatus.Pending)
                throw new BadRequestException(
                    "Transaction already reviewed");

            transaction.IssuedByUserId = librarianId;

            transaction.Status = dto.IsApproved
                ? Domain.Enums.BorrowingStatus.Borrowed
                : Domain.Enums.BorrowingStatus.Rejected;

            _unitOfWork
                .GetRepository<BorrowingTransaction>()
                .Update(transaction);

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                EntityId = transactionId,
                EntityAffected = nameof(BorrowingTransaction),
                Details = dto.IsApproved
                    ? $"Borrow request approved for book '{transaction.Book.Title}'"
                    : $"Borrow request rejected for book '{transaction.Book.Title}'",
                Action = dto.IsApproved ? "Borrow Request Approved" : "Borrow Request Rejected",
                UserId = librarianId
            });

            return new ApiResponse<string>
            {
                Success = true,
                Message = dto.IsApproved
                    ? "Request approved"
                    : "Request rejected"
            };
        }


        public async Task<ApiResponse<string>> ConfirmReturnAsync(int transactionId,string librarianId)
        {
            if(string.IsNullOrEmpty(librarianId))
                throw new BadRequestException("Librarian ID cannot be null or empty.");


            var spec = new GeneralSpecifications<BorrowingTransaction>(
                x => x.Id == transactionId);

            var transaction = await _unitOfWork
                .GetRepository<BorrowingTransaction>()
                .GetAsync(spec);

            if (transaction is null)
                throw new NotFoundException("Transaction not found");

            if (transaction.Status != Domain.Enums.BorrowingStatus.ReturnPending)
                throw new BadRequestException(
                    "Book is not awaiting return approval");

            transaction.Status = Domain.Enums.BorrowingStatus.Returned;
            transaction.ReturnDate = DateTime.UtcNow;
            transaction.IssuedByUserId = librarianId;

            _unitOfWork.GetRepository<BorrowingTransaction>().Update(transaction);

            await _activityLog.LogAsync(new CreateActivityLogDto
            {
                EntityId = transactionId,
                EntityAffected = nameof(BorrowingTransaction),
                Details = $"Return request approved for book '{transaction.Book.Title}'",
                Action = "Return Request Approved",
                UserId = librarianId
            });

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Book returned successfully"
            };
        }
    }
}
