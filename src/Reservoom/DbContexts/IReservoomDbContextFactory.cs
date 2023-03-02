namespace Reservoom.DbContexts
{
    public interface IReservoomDbContextFactory
    {
        ReservoomDbContext CreateDbContext();
    }
}