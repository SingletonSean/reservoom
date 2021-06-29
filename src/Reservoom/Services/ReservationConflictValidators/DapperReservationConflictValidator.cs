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

namespace Reservoom.Services.ReservationConflictValidators
{
    public class DapperReservationConflictValidator : IReservationConflictValidator
    {
        private const string GET_CONFLICTING_RESERVATION_SQL = @"
            SELECT *
            FROM Reservations
            WHERE FloorNumber = @FloorNumber
            AND RoomNumber = @RoomNumber
            AND StartTime < @EndTime
            AND EndTime > @StartTime";

        private readonly SqliteDbConnectionFactory _sqliteDbConnectionFactory;

        public DapperReservationConflictValidator(SqliteDbConnectionFactory sqliteDbConnectionFactory)
        {
            _sqliteDbConnectionFactory = sqliteDbConnectionFactory;
        }

        public async Task<Reservation> GetConflictingReservation(Reservation reservation)
        {
            using (IDbConnection database = _sqliteDbConnectionFactory.Connect())
            {
                object parameters = new
                {
                    FloorNumber = reservation.RoomID.FloorNumber,
                    RoomNumber = reservation.RoomID.RoomNumber,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime
                };

                DapperReservationDTO reservationDTO =
                    await database.QueryFirstOrDefaultAsync<DapperReservationDTO>(
                        GET_CONFLICTING_RESERVATION_SQL, parameters);

                if (reservationDTO == null)
                {
                    return null;
                }

                return ToReservation(reservationDTO);
            }
        }

        private static Reservation ToReservation(DapperReservationDTO dto)
        {
            return new Reservation(new RoomID(dto.FloorNumber, dto.RoomNumber), dto.Username, dto.StartTime, dto.EndTime);
        }
    }
}
