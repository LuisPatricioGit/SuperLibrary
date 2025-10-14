using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Helper;

namespace SuperLibrary.Web.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;
    private Random _random;

    public SeedDb(DataContext context, IUserHelper userHelper)
    {
        _context = context;
        _userHelper = userHelper;
        _random = new Random();
    }

    /// <summary>
    /// This method is used to seed the database with initial data.
    /// </summary>
    /// <returns></returns>
    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        // Populate Database with Roles
        await _userHelper.CheckRoleAsync("Admin");
        await _userHelper.CheckRoleAsync("Employee");
        await _userHelper.CheckRoleAsync("Reader");
        await _userHelper.CheckRoleAsync("Anon"); //Anonymous users (Normally not created Used as Anon is Default)

        // Populate Database with Users
        await AddUser("Admin","SuperLibrary", "SuperLibrary.Admin@gmail.com", "SLAdmin", "123456789", "Admin");
        await AddUser("Employee", "SuperLibrary", "SuperLibrary.Employee@gmail.com", "SLEmployee", "123456789", "Employee");
        await AddUser("Reader", "SuperLibrary", "SuperLibrary.Reader@gmail.com", "SLReader", "123456789", "Reader");
        //await AddUser("Anon", "SuperLibrary", "SuperLibrary.Anon@gmail.com", "SLAnon", "123456789", "Anon");

        var user = await _userHelper.GetUserByEmailAsync("SuperLibrary.Admin@gmail.com");

        if (!_context.Books.Any())
        {
            AddBook("DragonBall: Son Goku e os Seus Amigos", "Akira Toriyama", "Devir", user);
            AddBook("Gerónimo Stilton: O Mistério do Olho de Esmeralda", "Gerónimo Stilton", "Editorial Presença", user);
            AddBook("Cherub: O Recruta", "Robert Muchamore", "Porto Editora", user);
            AddBook("Astérix na Lusitânia", "René Gosciny", "Edições Asa", user);
            //AddBook("Sonic The Hedgehog #1: Fallout", "Ian Flynn", "IDW Publishing", user);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// This method is used to add a user to the database.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="userName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task AddUser(string firstName, string lastName, string email, string userName, string phoneNumber, string role)
    {
        var user = await _userHelper.GetUserByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = userName,
                PhoneNumber = phoneNumber,
            };

            var result = await _userHelper.AddUserAsync(user, "Obv!ouslyF@k3");

            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException("Could not Create the User in Seeder");
            }

            await _userHelper.AddUserToRoleAsync(user, role);
        }

        var isInRole = await _userHelper.IsUserInRoleAsync(user, role);
        if (!isInRole)
        {
            await _userHelper.AddUserToRoleAsync(user, role);
        }
    }

    /// <summary>
    /// This method is used to add a book to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="author"></param>
    /// <param name="publisher"></param>
    /// <param name="user"></param>
    private void AddBook(string title, string author, string publisher, User user)
    {
        _context.Books.Add(new Book
        {
            Title = title,
            Author = author,
            Publisher = publisher,
            Copies = _random.Next(1000),
            GenreId = _random.Next(3),
            IsAvailable = true,
            User = user
        });
    }
}