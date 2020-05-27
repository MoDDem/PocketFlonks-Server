using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

class DatabaseRead : DatabaseScript {
	public DatabaseRead() : base() { }

    public static async Task<DbDataReader> GetAccount(string email = null, int id = 0) {
        if(connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        SQLiteCommand cmd;
        if(id == 0) {
            cmd = new SQLiteCommand($"SELECT id, name, email, password_hash, salt FROM Accounts WHERE email=@email LIMIT 1", connection);
            cmd.Parameters.AddWithValue("@email", email);
        } else {
            cmd = new SQLiteCommand($"SELECT id, name, email, password_hash, salt FROM Accounts WHERE id=@id LIMIT 1", connection);
            cmd.Parameters.AddWithValue("@id", id);
        }
        cmd.Prepare();

        var reader = await cmd.ExecuteReaderAsync();
        if(reader.HasRows)
            await reader.ReadAsync();
        else
            throw new Exception("Make something here");

        return reader;
    }

    public static async Task<DbDataReader> GetSession(string sessionID) {
        if(connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        var cmd = new SQLiteCommand($"SELECT id, account_id, text, expires FROM Sessions WHERE text=@sessionID LIMIT 1", connection);
        cmd.Parameters.AddWithValue("@sessionID", sessionID);
        cmd.Prepare();

        var reader = await cmd.ExecuteReaderAsync();
        if(reader.HasRows)
            await reader.ReadAsync();
        else
            return null;

        return reader;
    }

    public static async Task<DbDataReader> GetPlayerLocation(int playerDbID) {
        if(connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        var cmd = new SQLiteCommand($"SELECT id, account_id, location, position FROM PlayerLocation WHERE account_id=@id LIMIT 1", connection);
        cmd.Parameters.AddWithValue("@id", playerDbID);
        cmd.Prepare();

        var reader = await cmd.ExecuteReaderAsync();
        if(reader.HasRows)
            await reader.ReadAsync();
        else
            return null;

        return reader;
    }
}
