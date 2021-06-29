using Dapper;
using Reservoom.DbConnections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservoom.DbInitializers
{
    public class SqliteDbInitializer
    {
        private const string CREATE_RESERVATIONS_TABLE_SQL = @"
            CREATE TABLE IF NOT EXISTS Reservations (
                Id TEXT PRIMARY KEY, 
                FloorNumber INTEGER,
                RoomNumber INTEGER,
                Username TEXT,
                StartTime TEXT,
                EndTime TEXT
            )";

        private readonly SqliteDbConnectionFactory _sqliteDbConnectionFactory;

        public SqliteDbInitializer(SqliteDbConnectionFactory sqliteDbConnectionFactory)
        {
            _sqliteDbConnectionFactory = sqliteDbConnectionFactory;
        }

        public void Initialize()
        {
            using (IDbConnection database = _sqliteDbConnectionFactory.Connect())
            {
                database.Execute(CREATE_RESERVATIONS_TABLE_SQL);
            }
        }
    }
}
