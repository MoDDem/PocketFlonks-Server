using System;
using UnityEngine;
using Telepathy;
using System.Diagnostics;
using System.Linq;

public class NetworkScript : MonoBehaviour
{
	public int port = 25698;
	public int maxPlayers = 50;

	public static NetworkScript instance;
	public static Server server = new Server();
	public int MaxPlayers { get; private set; }

	public ConnectionDictionary connections = new ConnectionDictionary();
	public PlayerDictionary players = new PlayerDictionary();

    private const string consoleName = "PocketFlonks Server v0.1";
	private long messagesReceived = 0;
	private long dataReceived = 0;
	private Stopwatch stopwatch = Stopwatch.StartNew();

	void Awake() {
		Application.targetFrameRate = 30;
        Application.runInBackground = true;
		QualitySettings.vSyncCount = 0;

		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(this);

		Console.Title = consoleName;
        ConsoleManager.WriteServer("Starting server...");

        SetupServer(maxPlayers);
    }

	private void SetupServer(int maxPlayers) {
        Telepathy.Logger.Log = (a) => { ConsoleManager.WriteServer(a); };
        Telepathy.Logger.LogWarning = (a) => { ConsoleManager.WriteServer(a, MessageType.Warning); };
		Telepathy.Logger.LogError = (a) => { ConsoleManager.WriteServer(a, MessageType.Error); };

		MaxPlayers = maxPlayers;

        server.Start(port);

        for(int i = 1; i <= MaxPlayers; i++) {
            players.Add(i, null);
        }

		ConsoleManager.WriteServer("Server started");
    }

	void FixedUpdate() {
        if(server.Active) {
			while(server.GetNextMessage(out Message msg)) {
				switch(msg.eventType) {
					case Telepathy.EventType.Connected:
                        OnUserConnected(msg);
						break;
					case Telepathy.EventType.Data:
						messagesReceived++;
						dataReceived += msg.data.Length;
                        OnDataReceived(msg);
						break;
					case Telepathy.EventType.Disconnected:
                        OnUserDisconnected(msg);
						break;
				}
			}

#if UNITY_STANDALONE
			if(stopwatch.ElapsedMilliseconds > 1000 * 2) {
				Console.Title = string.Format(consoleName + " | Stats: Online={3} Server in={0} ({1} KB/s)  out={0} ({1} KB/s) ReceiveQueue={2}",
					messagesReceived, dataReceived * 1000 / (stopwatch.ElapsedMilliseconds * 1024), server.ReceiveQueueCount.ToString(), players.Where(x => x.Value != null).Count());

				stopwatch.Stop();
				stopwatch = Stopwatch.StartNew();
				messagesReceived = 0;
				dataReceived = 0;
			}
#endif
		} else {
#if UNITY_STANDALONE
			string serverOfflineTitle = "Server is now offline! Start it to recieve a data.";
			if(Console.Title != serverOfflineTitle)
				Console.Title = serverOfflineTitle;
#endif
		}
	}

	private void OnUserConnected(Message msg) {
		ConsoleManager.WriteServer($"+Player connection id({msg.connectionId}) connected.");

		UserConnection connection = ScriptableObject.CreateInstance<UserConnection>();
		connection.id = msg.connectionId;
		connection.sessionID = null;
		connection.status = UserStatus.InMenu;
		connection.deviceType = DeviceType.Unknown;

		connections.Add(msg.connectionId, connection);
	}

	private void OnDataReceived(Message msg) {
		PacketReader packet = new PacketReader(msg.data);
		ClientHeaders header = (ClientHeaders) packet.ReadUInt16();

		switch(header) {
			case ClientHeaders.Text:
				ConsoleManager.WriteServer($"Player connection id({msg.connectionId}) text: {packet.ReadString()}");
				break;
			case ClientHeaders.RegisterPlayer:
				ServerRecieve.RegisterUser(connections[msg.connectionId], packet);
				break;
			case ClientHeaders.LoginPlayerByLocal:
				ServerRecieve.LoginPlayerByLocal(connections[msg.connectionId], packet);
				break;
			case ClientHeaders.LoginPlayByGoogle:
				break;
			case ClientHeaders.LoginPlayerBySession:
				ServerRecieve.LoginPlayerBySession(connections[msg.connectionId], packet);
				break;
			case ClientHeaders.RequestJoinGameData:
				ServerRecieve.RequestJoinGameData(connections[msg.connectionId], packet);
				break;
			case ClientHeaders.GetPlayersInLocation:
				ServerRecieve.GetPlayersInLocation(connections[msg.connectionId], packet);
				break;
			case ClientHeaders.PlayerMovement:
				ServerRecieve.PlayerMovement(connections[msg.connectionId], packet);
				break;
			default:
				throw new Exception("Looks like you forget to impliment Client Headers data.");
		}
	}

	private void OnUserDisconnected(Message msg) {
		UserConnection connection = connections[msg.connectionId];

		if(connection.isLogged) {
			Destroy(players[connection.playerID].gameObject);
			players[connection.playerID] = null;
		}
		ServerSend.UserDisconnected(connection);

		connections.Remove(msg.connectionId);
		ConsoleManager.WriteServer($"-Player connection id({msg.connectionId}) disconnected.");
	}

	#if UNITY_EDITOR
	void OnGUI() {
		GUI.enabled = !server.Active;
		if(GUI.Button(new Rect(0, 25, 120, 20), "Start Server"))
			server.Start(25698);

		GUI.enabled = server.Active;
		if(GUI.Button(new Rect(130, 25, 120, 20), "Stop Server"))
			server.Stop();

		GUI.enabled = true;
	}
	#endif

	void OnApplicationQuit() {
        server.Stop();
		print("Server stopped!");
    }
}