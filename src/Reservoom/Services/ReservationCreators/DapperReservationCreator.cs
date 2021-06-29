using Dapper;
using Reservoom.DbConnections;
using Reservoom.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservoom.Services.ReservationCreators
{
    public class DapperReservationCreator : IReservationCreator
    {
        private const string INSERT_RESERVATION_SQL = @"
            INSERT INTO
            Reservations (Id, FloorNumber, RoomNumber, Username, StartTime, EndTime)
            VALUES (@Id, @FloorNumber, @RoomNumber, @Username, @StartTime, @EndTime)";

        private readonly SqliteDbConnectionFactory _sqliteDbConnectionFactory;

        public DapperReservationCreator(SqliteDbConnectionFactory sqliteDbConnectionFactory)
        {
            _sqliteDbConnectionFactory = sqliteDbConnectionFactory;
        }

        public async Task CreateReservation(Reservation reservation)
        {
            using (IDbConnection database = _sqliteDbConnectionFactory.Connect())
            {
                object parameters = new
                {
                    Id = Guid.NewGuid().ToString(),
                    FloorNumber = reservation.RoomID.FloorNumber,
                    RoomNumber = reservation.RoomID.RoomNumber,
                    Username = reservation.Username,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                };

                await database.ExecuteAsync(INSERT_RESERVATION_SQL, parameters);
            }
        }
    }
}
