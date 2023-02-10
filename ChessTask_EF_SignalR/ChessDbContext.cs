using ChessTask_EF_SignalR.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessTask_EF_SignalR
{
    public class ChessDbContext: DbContext 
    {
        public DbSet<Step> Steps { get; set; } = null!;
        public ChessDbContext(DbContextOptions<ChessDbContext> options):
            base(options)
        {
            Database.EnsureCreated();
        }
    }
}
