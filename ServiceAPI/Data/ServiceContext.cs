using Microsoft.EntityFrameworkCore;

namespace ServiceAPI.Data
{
    public class ServiceContext : DbContext
    {
        public DbSet<Service> services { get; set; }
        public DbSet<ServiceState> serviceStates { get; set; }
        public DbSet<State> states { get; set; }
        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options)
        {

        }
    }
}
