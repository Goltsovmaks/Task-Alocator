using Microsoft.EntityFrameworkCore;


namespace Task_Alocator.Models
{
    public class EventContext: DbContext
    {
        public DbSet<Models.User> Users{ get; set; }
        public DbSet<Event> Events { get; set; }


        public EventContext(DbContextOptions<EventContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
