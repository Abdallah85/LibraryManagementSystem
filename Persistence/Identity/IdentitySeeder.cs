using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;


namespace Persistence.Identity
{
    public static class IdentitySeeder
    {
        private static readonly string[] RoleNames = { "Administrator", "Librarian", "Staff", "Member" };


        public static async Task InitIdentityAsync(WebApplication app)
        {
            await SeedRolesAsync(app);
            await SeedLanguagesAsync(app);
            await SeedPublishersAsync(app);
            await SeedCategoriesAsync(app);
            await SeedAuthorsAsync(app);
            await SeedBooksAsync(app);
            await SeedBookAuthorsAsync(app);
            await SeedBookCategoriesAsync(app);
            await SeedMemberUsersAsync(app);
            await SeedBorrowingTransactionsAsync(app);
        }

        public static async Task SeedRolesAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            if (!roleManager.Roles.Any())
            {
                foreach (var roleName in RoleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                        await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
        }

        public static async Task SeedLanguagesAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Set<Language>().Any())
                return;

            var languages = new List<Language>
            {
                new Language { Name = "English", Code = "en", CreatedAt = DateTime.UtcNow },
                new Language { Name = "Spanish", Code = "es", CreatedAt = DateTime.UtcNow },
                new Language { Name = "French", Code = "fr", CreatedAt = DateTime.UtcNow },
                new Language { Name = "German", Code = "de", CreatedAt = DateTime.UtcNow },
                new Language { Name = "Italian", Code = "it", CreatedAt = DateTime.UtcNow },
                new Language { Name = "Arabic", Code = "ar", CreatedAt = DateTime.UtcNow },
                new Language { Name = "Chinese", Code = "zh", CreatedAt = DateTime.UtcNow },
                new Language { Name = "Japanese", Code = "ja", CreatedAt = DateTime.UtcNow }
            };

            await context.Set<Language>().AddRangeAsync(languages);
            await context.SaveChangesAsync();
        }

        public static async Task SeedPublishersAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Set<Publisher>().Any())
                return;

            var publishers = new List<Publisher>
            {
                new Publisher
                {
                    Name = "Penguin Books",
                    Address = "80 Strand, London WC2R 0RL, UK",
                    ContactEmail = "info@penguin.co.uk",
                    Website = "www.penguin.co.uk",
                    CreatedAt = DateTime.UtcNow
                },
                new Publisher
                {
                    Name = "Hachette Book Group",
                    Address = "1290 Avenue of the Americas, New York, NY 10104, USA",
                    ContactEmail = "info@hachette.com",
                    Website = "www.hachettebookgroup.com",
                    CreatedAt = DateTime.UtcNow
                },
                new Publisher
                {
                    Name = "Simon & Schuster",
                    Address = "1230 Avenue of the Americas, New York, NY 10020, USA",
                    ContactEmail = "info@simonandschuster.com",
                    Website = "www.simonandschuster.com",
                    CreatedAt = DateTime.UtcNow
                },
                new Publisher
                {
                    Name = "Random House",
                    Address = "1745 Broadway, New York, NY 10019, USA",
                    ContactEmail = "info@randomhouse.com",
                    Website = "www.randomhouse.com",
                    CreatedAt = DateTime.UtcNow
                },
                new Publisher
                {
                    Name = "Oxford University Press",
                    Address = "Great Clarendon Street, Oxford OX2 6DP, UK",
                    ContactEmail = "info@oup.com",
                    Website = "www.oup.com",
                    CreatedAt = DateTime.UtcNow
                },
                new Publisher
                {
                    Name = "Cambridge University Press",
                    Address = "University Printing House, Cambridge CB2 8BS, UK",
                    ContactEmail = "info@cambridge.org",
                    Website = "www.cambridge.org",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Set<Publisher>().AddRangeAsync(publishers);
            await context.SaveChangesAsync();
        }

        public static async Task SeedCategoriesAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Set<Category>().Any())
                return;

            var categories = new List<Category>
            {
                new Category { Name = "Fiction", Description = "Imaginative storytelling and novels", CreatedAt = DateTime.UtcNow },
                new Category { Name = "Science Fiction", Description = "Futuristic and speculative fiction", CreatedAt = DateTime.UtcNow },
                new Category { Name = "Mystery", Description = "Crime and detective stories", CreatedAt = DateTime.UtcNow },
                new Category { Name = "Romance", Description = "Love stories and romantic fiction", CreatedAt = DateTime.UtcNow },
                new Category { Name = "History", Description = "Historical narratives and accounts", CreatedAt = DateTime.UtcNow },
                new Category { Name = "Biography", Description = "Life stories and autobiographies", CreatedAt = DateTime.UtcNow },
                new Category { Name = "Science & Technology", Description = "Scientific and technical knowledge", CreatedAt = DateTime.UtcNow },
                new Category { Name = "Self-Help", Description = "Personal development and improvement", CreatedAt = DateTime.UtcNow },
                new Category { Name = "Children", Description = "Books for children", CreatedAt = DateTime.UtcNow },
                new Category { Name = "Fantasy", Description = "Fantasy worlds and magical tales", CreatedAt = DateTime.UtcNow }
            };

            await context.Set<Category>().AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        public static async Task SeedAuthorsAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Set<Author>().Any())
                return;

            var authors = new List<Author>
            {
                new Author { FullName = "William Shakespeare", Bio = "English playwright and poet, widely regarded as the world's pre-eminent dramatist", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Jane Austen", Bio = "English novelist known for romantic fiction", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Leo Tolstoy", Bio = "Russian writer best known for the epic novels War and Peace and Anna Karenina", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Charles Dickens", Bio = "English writer and social critic", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Mark Twain", Bio = "American writer and humorist", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "George Orwell", Bio = "English novelist and political commentator", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Stephen King", Bio = "American author known for horror fiction", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "J.K. Rowling", Bio = "British author of the Harry Potter series", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Isaac Asimov", Bio = "American science fiction author and biochemist", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Agatha Christie", Bio = "English writer known for detective novels", CreatedAt = DateTime.UtcNow }
            };

            await context.Set<Author>().AddRangeAsync(authors);
            await context.SaveChangesAsync();
        }

        public static async Task SeedBooksAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Set<Book>().Any())
                return;

            var books = new List<Book>
            {
                new Book
                {
                    ISBN = "978-0743273565",
                    Title = "The Great Gatsby",
                    Edition = "First Edition",
                    Summary = "A classic American novel set in the Jazz Age",
                    PublicationYear = 1925,
                    CoverImageUrl = "https://example.com/great-gatsby.jpg",
                    Status = BookStatus.InLibrary,
                    LanguageId = 1, // English
                    PublisherId = 1, // Penguin Books
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0451524935",
                    Title = "1984",
                    Edition = "Reprint",
                    Summary = "A dystopian social science fiction novel",
                    PublicationYear = 1949,
                    CoverImageUrl = "https://example.com/1984.jpg",
                    Status = BookStatus.CheckedOut,
                    LanguageId = 1, // English
                    PublisherId = 2, // Hachette Book Group
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0141439761",
                    Title = "Pride and Prejudice",
                    Edition = "Classic Edition",
                    Summary = "A romantic novel by Jane Austen",
                    PublicationYear = 1813,
                    CoverImageUrl = "https://example.com/pride-prejudice.jpg",
                    Status = BookStatus.InLibrary,
                    LanguageId = 1, // English
                    PublisherId = 3, // Simon & Schuster
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0451523746",
                    Title = "War and Peace",
                    Edition = "Oxford World Classics",
                    Summary = "A historical novel by Leo Tolstoy",
                    PublicationYear = 1869,
                    CoverImageUrl = "https://example.com/war-peace.jpg",
                    Status = BookStatus.InLibrary,
                    LanguageId = 1, // English
                    PublisherId = 5, // Oxford University Press
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0451530786",
                    Title = "The Adventures of Tom Sawyer",
                    Edition = "Dover Thrift",
                    Summary = "A novel by Mark Twain about a young boy's adventures",
                    PublicationYear = 1876,
                    CoverImageUrl = "https://example.com/tom-sawyer.jpg",
                    Status = BookStatus.InLibrary,
                    LanguageId = 1, // English
                    PublisherId = 4, // Random House
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0451518491",
                    Title = "The Hobbit",
                    Edition = "Anniversary Edition",
                    Summary = "A fantasy novel by J.R.R. Tolkien",
                    PublicationYear = 1937,
                    CoverImageUrl = "https://example.com/hobbit.jpg",
                    Status = BookStatus.InLibrary,
                    LanguageId = 1, // English
                    PublisherId = 1, // Penguin Books
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0451331181",
                    Title = "Foundation",
                    Edition = "50th Anniversary",
                    Summary = "A science fiction novel by Isaac Asimov",
                    PublicationYear = 1951,
                    CoverImageUrl = "https://example.com/foundation.jpg",
                    Status = BookStatus.InLibrary,
                    LanguageId = 1, // English
                    PublisherId = 2, // Hachette Book Group
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0451529770",
                    Title = "The Shining",
                    Edition = "Special Edition",
                    Summary = "A horror novel by Stephen King",
                    PublicationYear = 1977,
                    CoverImageUrl = "https://example.com/shining.jpg",
                    Status = BookStatus.InLibrary,
                    LanguageId = 1, // English
                    PublisherId = 3, // Simon & Schuster
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0451532558",
                    Title = "Harry Potter and the Philosopher's Stone",
                    Edition = "US Edition",
                    Summary = "The first book in the Harry Potter series",
                    PublicationYear = 1998,
                    CoverImageUrl = "https://example.com/hp1.jpg",
                    Status = BookStatus.CheckedOut,
                    LanguageId = 1, // English
                    PublisherId = 4, // Random House
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    ISBN = "978-0451532009",
                    Title = "Murder on the Orient Express",
                    Edition = "Classic",
                    Summary = "A mystery novel by Agatha Christie",
                    PublicationYear = 1934,
                    CoverImageUrl = "https://example.com/murder-orient.jpg",
                    Status = BookStatus.InLibrary,
                    LanguageId = 1, // English
                    PublisherId = 5, // Oxford University Press
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Set<Book>().AddRangeAsync(books);
            await context.SaveChangesAsync();
        }

        public static async Task SeedBookAuthorsAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Set<BookAuthor>().Any())
                return;

            var bookAuthors = new List<BookAuthor>
            {
                new BookAuthor { BookId = 3, AuthorId = 2 }, // Pride and Prejudice - Jane Austen
                new BookAuthor { BookId = 4, AuthorId = 3 }, // War and Peace - Leo Tolstoy
                new BookAuthor { BookId = 5, AuthorId = 5 }, // Tom Sawyer - Mark Twain
                new BookAuthor { BookId = 2, AuthorId = 6 }, // 1984 - George Orwell
                new BookAuthor { BookId = 8, AuthorId = 7 }, // The Shining - Stephen King
                new BookAuthor { BookId = 9, AuthorId = 8 }, // Harry Potter - J.K. Rowling
                new BookAuthor { BookId = 7, AuthorId = 9 }, // Foundation - Isaac Asimov
                new BookAuthor { BookId = 10, AuthorId = 10 }, // Murder on the Orient Express - Agatha Christie
            };

            await context.Set<BookAuthor>().AddRangeAsync(bookAuthors);
            await context.SaveChangesAsync();
        }

        public static async Task SeedBookCategoriesAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Set<BookCategory>().Any())
                return;

            var bookCategories = new List<BookCategory>
            {
                new BookCategory { BookId = 1, CategoryId = 1 }, // Great Gatsby - Fiction
                new BookCategory { BookId = 2, CategoryId = 1 }, // 1984 - Fiction
                new BookCategory { BookId = 2, CategoryId = 2 }, // 1984 - Science Fiction
                new BookCategory { BookId = 3, CategoryId = 4 }, // Pride and Prejudice - Romance
                new BookCategory { BookId = 4, CategoryId = 5 }, // War and Peace - History
                new BookCategory { BookId = 5, CategoryId = 1 }, // Tom Sawyer - Fiction
                new BookCategory { BookId = 6, CategoryId = 10 }, // The Hobbit - Fantasy
                new BookCategory { BookId = 7, CategoryId = 2 }, // Foundation - Science Fiction
                new BookCategory { BookId = 8, CategoryId = 1 }, // The Shining - Fiction
                new BookCategory { BookId = 9, CategoryId = 10 }, // Harry Potter - Fantasy
                new BookCategory { BookId = 10, CategoryId = 3 }, // Murder on the Orient Express - Mystery
            };

            await context.Set<BookCategory>().AddRangeAsync(bookCategories);
            await context.SaveChangesAsync();
        }

        public static async Task SeedMemberUsersAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            // Check if test users already exist
            if (await userManager.FindByEmailAsync("member1@library.com") != null)
                return;

            var memberUsers = new List<(User user, string password, string role)>
            {
                (new User
                {
                    UserName = "john_doe",
                    Email = "member1@library.com",
                    EmailConfirmed = true,
                    IsActive = true,
                    IsMember = true,
                    MembershipDate = DateTime.UtcNow.AddMonths(-6),
                    Status = MembershipStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }, "SecurePass123!", "Member"),
                (new User
                {
                    UserName = "jane_smith",
                    Email = "member2@library.com",
                    EmailConfirmed = true,
                    IsActive = true,
                    IsMember = true,
                    MembershipDate = DateTime.UtcNow.AddMonths(-3),
                    Status = MembershipStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }, "SecurePass123!", "Member"),
                (new User
                {
                    UserName = "bob_wilson",
                    Email = "member3@library.com",
                    EmailConfirmed = true,
                    IsActive = true,
                    IsMember = true,
                    MembershipDate = DateTime.UtcNow.AddMonths(-12),
                    Status = MembershipStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }, "SecurePass123!", "Member"),
                (new User
                {
                    UserName = "librarian_admin",
                    Email = "librarian@library.com",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, "SecurePass123!", "Librarian")
            };

            foreach (var (user, password, role) in memberUsers)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        public static async Task SeedBorrowingTransactionsAsync(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            if (context.Set<BorrowingTransaction>().Any())
                return;

            var librarianUser = await userManager.FindByEmailAsync("librarian@library.com");
            var member1 = await userManager.FindByEmailAsync("member1@library.com");
            var member2 = await userManager.FindByEmailAsync("member2@library.com");
            var member3 = await userManager.FindByEmailAsync("member3@library.com");

            if (librarianUser == null || member1 == null || member2 == null || member3 == null)
                return;

            var transactions = new List<BorrowingTransaction>
            {
                new BorrowingTransaction
                {
                    BookId = 2, // 1984 (checked out)
                    UserId = member1.Id,
                    IssuedByUserId = librarianUser.Id,
                    BorrowDate = DateTime.UtcNow.AddDays(-15),
                    DueDate = DateTime.UtcNow.AddDays(-1),
                    ReturnDate = null,
                    Status = BorrowingStatus.Overdue,
                    FineAmount = 5.0m,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    CreatedBy = librarianUser.Id
                },
                new BorrowingTransaction
                {
                    BookId = 9, // Harry Potter (checked out)
                    UserId = member2.Id,
                    IssuedByUserId = librarianUser.Id,
                    BorrowDate = DateTime.UtcNow.AddDays(-10),
                    DueDate = DateTime.UtcNow.AddDays(4),
                    ReturnDate = null,
                    Status = BorrowingStatus.Borrowed,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    CreatedBy = librarianUser.Id
                },
                new BorrowingTransaction
                {
                    BookId = 3, // Pride and Prejudice (returned)
                    UserId = member3.Id,
                    IssuedByUserId = librarianUser.Id,
                    BorrowDate = DateTime.UtcNow.AddDays(-30),
                    DueDate = DateTime.UtcNow.AddDays(-15),
                    ReturnDate = DateTime.UtcNow.AddDays(-14),
                    Status = BorrowingStatus.Returned,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = librarianUser.Id,
                    UpdatedAt = DateTime.UtcNow.AddDays(-14),
                    UpdatedBy = librarianUser.Id
                },
                new BorrowingTransaction
                {
                    BookId = 4, // War and Peace (returned)
                    UserId = member1.Id,
                    IssuedByUserId = librarianUser.Id,
                    BorrowDate = DateTime.UtcNow.AddDays(-45),
                    DueDate = DateTime.UtcNow.AddDays(-30),
                    ReturnDate = DateTime.UtcNow.AddDays(-29),
                    Status = BorrowingStatus.Returned,
                    CreatedAt = DateTime.UtcNow.AddDays(-45),
                    CreatedBy = librarianUser.Id,
                    UpdatedAt = DateTime.UtcNow.AddDays(-29),
                    UpdatedBy = librarianUser.Id
                },
                new BorrowingTransaction
                {
                    BookId = 1, // Great Gatsby (returned)
                    UserId = member2.Id,
                    IssuedByUserId = librarianUser.Id,
                    BorrowDate = DateTime.UtcNow.AddDays(-60),
                    DueDate = DateTime.UtcNow.AddDays(-45),
                    ReturnDate = DateTime.UtcNow.AddDays(-44),
                    Status = BorrowingStatus.Returned,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    CreatedBy = librarianUser.Id,
                    UpdatedAt = DateTime.UtcNow.AddDays(-44),
                    UpdatedBy = librarianUser.Id
                }
            };

            await context.Set<BorrowingTransaction>().AddRangeAsync(transactions);
            await context.SaveChangesAsync();
        }
    }
}
