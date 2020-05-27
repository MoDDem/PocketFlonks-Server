using UnityEngine;
using Console = ConsoleManager;
using System.Data.SQLite;
using System.Diagnostics;

class DatabaseScript : MonoBehaviour {
    private const string path = @"D:\CodeProjects\PocketFlonks Database\GameDatabase.db";

    protected static SQLiteConnection connection;

    void Awake() {
		Console.WriteDB("Starting database...");
        /*
        ProcessStartInfo info = new ProcessStartInfo("mongod", "--dbpath=\"D:\\CodeProjects\\PocketFlonks Database\\mongo\"");
        Process.Start(info);
        */
        connection = new SQLiteConnection($"Data Source={path};Version=3;");

        if(connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        var cmd = new SQLiteCommand("SELECT SQLITE_VERSION()", connection);
        string version = cmd.ExecuteScalar().ToString();
		Console.WriteDB("SQLite version: " + version);

        connection.Close();
    }
}
