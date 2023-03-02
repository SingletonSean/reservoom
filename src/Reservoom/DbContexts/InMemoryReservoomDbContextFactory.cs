using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservoom.DbContexts
{
    public class InMemoryReservoomDbContextFactory : IReservoomDbContextFactory
    {
        private readonly SqliteConnection _connection;

        public InMemoryReservoomDbContextFactory()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
        }

        public ReservoomDbContext CreateDbContext()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(_connection).Options;

            return new ReservoomDbContext(options);
        }
    }
}
