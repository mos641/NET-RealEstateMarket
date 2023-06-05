/*
a. Use lecture slides to create your data context. Remember, you need the constructor, and pay attention to inheritance.
b. Make sure in your context you include the Constructor and the DBSet(s) to hold your entity objects. The DBSet variables should have a plural name of entities. Like for Client model:
    i. public DbSet<Client> Clients { get; set; }
c. Make sure the table names are not plural. You can do that by setting the proper names in OnModelCreating method. Like, for the Client model:
    i. modelBuilder.Entity<Client>().ToTable("Client");
d. Subscription should have a composite key of ClientId and BrokerageId, and we need to set that using Fluent API:
    
    modelBuilder.Entity<Subscription>()
                    .HasKey(c => new { c.ClientId, c.BrokerageId });
*/

using RealEstateMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace RealEstateMarket.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Brokerage> Brokerages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Brokerage>().ToTable("Brokerage");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<Subscription>()
                            .HasKey(c => new { c.ClientId, c.BrokerageId });
            modelBuilder.Entity<Advertisement>().ToTable("Advertisement");
        }
    }
}
