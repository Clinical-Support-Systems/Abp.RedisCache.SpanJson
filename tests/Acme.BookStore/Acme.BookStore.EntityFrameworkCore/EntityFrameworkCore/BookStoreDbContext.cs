using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Acme.BookStore.EntityFrameworkCore
{
    public class BookStoreDbContext : AbpDbContext
    {
        //Add DbSet properties for your entities...

        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) 
            : base(options)
        {

        }
    }
}
