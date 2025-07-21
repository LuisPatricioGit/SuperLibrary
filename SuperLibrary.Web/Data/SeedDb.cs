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

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        await _userHelper.CheckRoleAsync("Admin");
        await _userHelper.CheckRoleAsync("Employee");
        await _userHelper.CheckRoleAsync("Reader");
        await _userHelper.CheckRoleAsync("Anon"); //Anonymous users (Normally not created Used as Anon is Default)

        var user = await _userHelper.GetUserByEmailAsync("SuperLibrary.Admin@gmail.com");
        if (user == null)
        {
            user = new User
            {
                FirstName = "Admin",
                LastName = "SuperLibrary",
                Email = "SuperLibrary.Admin@gmail.com",
                UserName = "SLAdmin",
                PhoneNumber = "123456789",
            };

            var result = await _userHelper.AddUserAsync(user, "123456");
            
            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException("Could not Create the User in Seeder");
            }

            await _userHelper.AddUserToRoleAsync(user, "Admin");
        }

        var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
        if (!isInRole)
        {
            await _userHelper.AddUserToRoleAsync(user, "Admin");
        }

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