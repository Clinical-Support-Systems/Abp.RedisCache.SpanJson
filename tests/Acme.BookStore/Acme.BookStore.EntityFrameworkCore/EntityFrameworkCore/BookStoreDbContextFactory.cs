using Acme.BookStore.Configuration;
using Acme.BookStore.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Acme.BookStore.EntityFrameworkCore
{
    /* This class is needed to run EF Core PMC commands. Not used anywhere else */
    public class BookStoreDbContextFactory : IDesignTimeDbContextFactory<BookStoreDbContext>
    {
        public BookStoreDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BookStoreDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            DbContextOptionsConfigurer.Configure(
                builder,
                configuration.GetConnectionString(BookStoreConsts.ConnectionStringName)
            );

            return new BookStoreDbContext(builder.Options);
        }
    }
}