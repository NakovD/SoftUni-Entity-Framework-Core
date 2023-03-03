namespace BookShop;

using BookShop.Models.Enums;
using Data;
using Initializer;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

public class StartUp
{
    public static void Main()
    {
        using var db = new BookShopContext();
        DbInitializer.ResetDatabase(db);

        var input = 40;

        var result = CountBooks(db, input);

        Console.WriteLine(result);
    }

    public static string GetBooksByAgeRestriction(BookShopContext context, string command)
    {
        var sb = new StringBuilder();

        var books = context.Books
            .AsNoTracking()
            .Select(b => new { b.Title, b.AgeRestriction })
            .ToArray()
            .Where(b => Enum.GetName(typeof(AgeRestriction), b.AgeRestriction)!.ToLower() == command.ToLower())
            .OrderBy(b => b.Title);

        foreach (var book in books)
        {
            sb.AppendLine(book.Title);
        }

        return sb.ToString().Trim();
    }

    public static string GetGoldenBooks(BookShopContext context)
    {
        var sb = new StringBuilder();

        var books = context.Books
            .AsNoTracking()
            .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
            .OrderBy(b => b.BookId)
            .Select(b => new { b.Title })
            .ToArray();

        foreach (var book in books)
        {
            sb.AppendLine(book.Title);
        }


        return sb.ToString().Trim();
    }

    public static string GetBooksByPrice(BookShopContext context)
    {
        var sb = new StringBuilder();

        var books = context.Books
            .AsNoTracking()
            .Where(b => b.Price > 40)
            .Select(b => new { b.Title, Price = b.Price.ToString("f2") })
            .OrderByDescending(b => b.Price)
            .ToArray();

        foreach (var book in books)
        {
            sb.AppendLine($"{book.Title} - ${book.Price}");
        }

        return sb.ToString().Trim();
    }

    public static string GetBooksNotReleasedIn(BookShopContext context, int year)
    {
        var sb = new StringBuilder();

        var books = context.Books
            .AsNoTracking()
            .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
            .OrderBy(b => b.BookId)
            .Select(b => new { b.Title })
            .ToArray();

        foreach (var book in books)
        {
            sb.AppendLine(book.Title);
        }

        return sb.ToString().Trim();
    }

    public static string GetBooksByCategory(BookShopContext context, string input)
    {
        var sb = new StringBuilder();

        var books = context.Books
            .AsNoTracking()
            .Where(b => b.BookCategories.Any(bc => input.ToLower().Contains(bc.Category.Name.ToLower())))
            .Select(b => new { b.Title })
            .OrderBy(b => b.Title)
            .ToList();

        foreach (var book in books)
        {
            sb.AppendLine(book.Title);
        }

        return sb.ToString().Trim();
    }

    public static string GetBooksReleasedBefore(BookShopContext context, string date)
    {
        var sb = new StringBuilder();

        var dateInCorrectFormat = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        var books = context.Books
            .AsNoTracking()
            .Where(b => b.ReleaseDate.HasValue && DateTime.Compare(b.ReleaseDate.Value, dateInCorrectFormat) < 0)
            .OrderByDescending(b => b.ReleaseDate)
            .Select(b => new { b.Title, b.EditionType, b.Price, b.ReleaseDate })
            .ToArray();

        foreach (var book in books)
        {
            sb.AppendLine($"{book.Title} - {Enum.GetName(typeof(EditionType), book.EditionType)} - ${book.Price:F2}");
        }

        return sb.ToString().Trim();
    }

    public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
    {
        var sb = new StringBuilder();

        var authors = context.Authors
            .AsNoTracking()
            .Where(a => !string.IsNullOrEmpty(a.FirstName) && a.FirstName.EndsWith(input.ToLower()))
            .Select(a => new { FullName = a.FirstName + " " + a.LastName })
            .OrderBy(a => a.FullName)
            .ToArray();

        foreach (var author in authors)
        {
            sb.AppendLine(author.FullName);
        }

        return sb.ToString().Trim();
    }

    public static string GetBookTitlesContaining(BookShopContext context, string input)
    {
        var sb = new StringBuilder();

        var books = context.Books
            .AsNoTracking()
            .Where(b => b.Title.ToLower().Contains(input.ToLower()))
            .Select(b => new { b.Title })
            .OrderBy(b => b.Title)
            .ToArray();

        foreach (var book in books)
        {
            sb.AppendLine(book.Title);
        }

        return sb.ToString().Trim();
    }

    public static string GetBooksByAuthor(BookShopContext context, string input)
    {
        var sb = new StringBuilder();

        var books = context.Books
            .AsNoTracking()
            .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
            .OrderBy(b => b.BookId)
            .Select(b => new { b.Title, AuthorName = $"{b.Author.FirstName ?? ""} {b.Author.LastName}" })
            .ToArray();

        foreach (var book in books)
        {
            sb.AppendLine($"{book.Title} ({book.AuthorName})");
        }

        return sb.ToString().Trim();
    }

    public static int CountBooks(BookShopContext context, int lengthCheck)
    {
        var result = context.Books
            .AsNoTracking()
            .Count(b => b.Title.Length > lengthCheck);

        return result;
    }
}


