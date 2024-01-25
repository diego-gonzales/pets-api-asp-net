using Microsoft.EntityFrameworkCore;

namespace pets_web_api;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Pet> Pets { get; set; }
}
