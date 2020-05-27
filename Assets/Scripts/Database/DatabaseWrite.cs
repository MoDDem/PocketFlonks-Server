using System.Data.SQLite;

class DatabaseWrite : DatabaseScript {
	public DatabaseWrite() : base() { }

    public static void RegisterAccount(string username, string email, string salt, string hash) {
        if(connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        var cmd = new SQLiteCommand(connection) {
            CommandText = "INSERT INTO Accounts(name, email, password_hash, salt) VALUES(@name, @email, @hash, @salt)"
        };

        cmd.Parameters.AddWithValue("@name", username);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@hash", hash);
        cmd.Parameters.AddWithValue("@salt", salt);
        cmd.Prepare();

        cmd.ExecuteNonQuery();

        connection.Close();
    }

    public static void CreateSession(int id, string sessionID) {
        if(connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        var cmd = new SQLiteCommand("INSERT INTO Sessions(account_id, text) VALUES(@account_id, @session)", connection);

        cmd.Parameters.AddWithValue("@account_id", id);
        cmd.Parameters.AddWithValue("@session", sessionID);
        cmd.Prepare();

        cmd.ExecuteNonQuery();
    }

    public static void DeleteSession(int id = 0, string sessionID = null) {
        if(connection.State != System.Data.ConnectionState.Open)
            connection.Open();

		SQLiteCommand cmd;
		if(id == 0) {
            cmd = new SQLiteCommand("DELETE FROM Sessions WHERE text = @session", connection);
            cmd.Parameters.AddWithValue("@session", sessionID);
        } else {
            cmd = new SQLiteCommand("DELETE FROM Sessions WHERE account_id = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
        }
        
        cmd.Prepare();
        cmd.ExecuteNonQuery();
    }
}