using Microsoft.EntityFrameworkCore;

namespace Acme.BookStore.EntityFrameworkCore
{
    public static class DbContextOptionsConfigurer
    {
        public static void Configure(
            DbContextOptionsBuilder<BookStoreDbContext> dbContextOptions, 
            string connectionString
            )
        {
            /* This is the single point to configure DbContextOptions for BookStoreDbContext */
            dbContextOptions.UseSqlServer(connectionString);
        }
    }
}
