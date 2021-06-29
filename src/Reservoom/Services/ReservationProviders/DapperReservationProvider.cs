using Dapper;
using Reservoom.DbConnections;
using Reservoom.DTOs;
using Reservoom.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservoom.Services.ReservationProviders
{
    public class DapperReservationProvider : IReservationProvider
    {
        private const string GET_ALL_RESERVATIONS_SQL = @"
            SELECT *
            FROM Reservations";

        private readonly SqliteDbConnectionFactory _sqliteDbConnectionFactory;

        public DapperReservationProvider(SqliteDbConnectionFactory sqliteDbConnectionFactory)
        {
            _sqliteDbConnectionFactory = sqliteDbConnectionFactory;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservations()
        {
            using (IDbConnection database = _sqliteDbConnectionFactory.Connect())
            {
                IEnumerable<DapperReservationDTO> reservationDTOs = 
                    await database.QueryAsync<DapperReservationDTO>(GET_ALL_RESERVATIONS_SQL);

                return reservationDTOs.Select(ToReservation);
            }
        }

        private static Reservation ToReservation(DapperReservationDTO dto)
        {
            return new Reservation(new RoomID(dto.FloorNumber, dto.RoomNumber), dto.Username, dto.StartTime, dto.EndTime);
        }
    }
}
