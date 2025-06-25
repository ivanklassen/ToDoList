using Microsoft.EntityFrameworkCore;

namespace ToDoList
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Task> Tasks { get; set; }
    }
}
