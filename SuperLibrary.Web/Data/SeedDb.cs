using System;
using System.Linq;
using System.Threading.Tasks;
using SuperLibrary.Web.Data.Entities;

namespace SuperLibrary.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Books.Any())
            {
                AddBook("DragonBall: Son Goku e os Seus Amigos","Akira Toriyama","Devir");
                AddBook("Gerónimo Stilton: O Mistério do Olho de Esmeralda","Gerónimo Stilton","Editorial Presença");
                AddBook("Cherub: O Recruta","Robert Muchamore","Porto Editora");
                AddBook("Astérix na Lusitânia","René Gosciny","Edições Asa");
            }

            await _context.SaveChangesAsync();
        }

        private void AddBook(string title, string author, string publisher)
        {
            _context.Books.Add(new Book
            {
                Title = title,
                Author = author,
                Publisher = publisher,
                Copies = _random.Next(1000),
                GenreId = _random.Next(3),
                IsAvailable = true

            });
        }
    }
}
