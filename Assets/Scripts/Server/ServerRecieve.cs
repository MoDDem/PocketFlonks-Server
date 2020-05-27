using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Security.Cryptography;

public class ServerRecieve {
	public static async void RequestJoinGameData(UserConnection connection, PacketReader packet) {
		Player connectedPlayer = NetworkScript.instance.players[connection.playerID];
		string sessionID = packet.ReadString();
		if(connection.sessionID != sessionID)
			return;

		connection.status = UserStatus.InLoadingScene;

		var dbLocation = await DatabaseRead.GetPlayerLocation(connectedPlayer.dbID);
		if(dbLocation == null) {
			ServerSceneManager.instance.scenes[GameLocation.KingsWay].AssignPlayer(connectedPlayer);
		} else {
			int vectorLength = 0;
			string position = (string) dbLocation["position"];
			var pos = new Vector3(
				Convert.ToInt32(position.Substring(0, vectorLength - 1)),
				Convert.ToInt32(position.Substring(vectorLength, vectorLength)),
				Convert.ToInt32(position.Substring(vectorLength * 2 + 1, vectorLength))
			);

			var location = (GameLocation) Convert.ToInt32(dbLocation["location"]);
			ServerSceneManager.instance.scenes[location].AssignPlayer(connectedPlayer, pos);
		}

		foreach(var player in NetworkScript.instance.players.Values) {
			if(player == null) continue;
			if(player.location != connectedPlayer.location | player.id == connectedPlayer.id) continue;

			ServerSend.SpawnPlayer(NetworkScript.instance.connections[player.connectionID], connectedPlayer);
		}

		ServerSend.JoinLocationData(connection, connectedPlayer);
	}

	public static void GetPlayersInLocation(UserConnection connection, PacketReader packet) {
		Player connectedPlayer = NetworkScript.instance.players[connection.playerID];
		string sessionID = packet.ReadString();
		if(connection.sessionID != sessionID)
			return;

		foreach(var player in NetworkScript.instance.players.Values) {
			if(player == null) continue;
			if(!player.gameObject.activeSelf | player.location != connectedPlayer.location | player.id == connectedPlayer.id) continue;

			ServerSend.SpawnPlayer(NetworkScript.instance.connections[connectedPlayer.id], player);
		}

		connectedPlayer.gameObject.SetActive(true);

		connection.status = UserStatus.InGame;
	}

	public static void PlayerMovement(UserConnection connection, PacketReader packet) {
		int id; Vector2 magnitude;
		try {
			id = packet.ReadInt32();
			magnitude = packet.ReadVector2();
		} catch(Exception) {
			NetworkScript.server.Disconnect(connection.id);
			return;
		}

		if(connection.playerID == id)
			NetworkScript.instance.players[id].moveMagnitude = magnitude;
		else
			ServerSend.StringMessage(connection.id, "Seems like you are trying to replace player id");
	}

	public static void RegisterUser(UserConnection connection, PacketReader packet) {
		string name; string email; string password;
		try {
			name = packet.ReadString();
			email = packet.ReadString();
			password = packet.ReadString();
		} catch(Exception) {
			NetworkScript.server.Disconnect(connection.id);
			return;
		}

		string[] hash = EncryptStringSalt.HashPassword(password);

		DatabaseWrite.RegisterAccount(name, email, hash[0], hash[1]);
	}

	public static async void LoginPlayerByLocal(UserConnection connection, PacketReader packet) {
		string email, password; DeviceType deviceType;
		try {
			email = packet.ReadString();
			password = packet.ReadString();
			deviceType = (DeviceType) packet.ReadInt32();
		} catch(Exception) {
			NetworkScript.server.Disconnect(connection.id);
			return;
		}

		if(NetworkScript.instance.players.Values.Any(x => x && x.email == email)) {
			ServerSend.PlayerLogout(connection, "Current account is in game alredy. Try to logout from account in another device if it used or/and relogin again.");
			return;
		}

		var db = await DatabaseRead.GetAccount(email: email);
		bool isCompare = EncryptStringSalt.ComparePasswordHash(password, (string) db["password_hash"], (string) db["salt"]);

		if(!isCompare) {
			ServerSend.PlayerLogout(connection, "Failed to login. Invalid password");
			return;
		}

		string sessionID = EncryptStringSalt.CreateSalt(18);
		DatabaseWrite.DeleteSession(Convert.ToInt32(db["id"]));
		DatabaseWrite.CreateSession(Convert.ToInt32(db["id"]), sessionID);

		for(int i = 1; i <= NetworkScript.instance.MaxPlayers; i++) {
			if(NetworkScript.instance.players[i] == null) {
				Player player = PlayerManager.instance.InstantiatePlayer();

				player.Initialize(Convert.ToInt32(db["id"]), connection.id, i, (string) db["name"], email, new Vector3(0, 1.6f, 0));
				player.gameObject.SetActive(false);

				connection.sessionID = sessionID;
				connection.playerID = i;
				connection.isLogged = true;
				connection.deviceType = deviceType;

				NetworkScript.instance.players[i] = player;
				ServerSend.PlayerLoginData(connection, player);
				return;
			}
		}

		ServerSend.StringMessage(connection.id, "Server is full!");
		NetworkScript.server.Disconnect(connection.id);
	}

	public static async void LoginPlayerBySession(UserConnection connection, PacketReader packet) {
		string sessionID; DeviceType device;
		try {
			sessionID = packet.ReadString();
			device = (DeviceType) packet.ReadInt32();
		} catch(Exception) {
			NetworkScript.server.Disconnect(connection.id);
			return;
		}

		if(NetworkScript.instance.connections.Values.Any(x => x.sessionID == sessionID)) {
			ServerSend.PlayerLogout(connection, "Current session is in game alredy. Try to login again.");
			return;
		}

		var sessionDB = await DatabaseRead.GetSession(sessionID);

		if(sessionDB != null) {
			var accDB = await DatabaseRead.GetAccount(id: Convert.ToInt32(sessionDB["account_id"]));
			string email = (string) accDB["email"];
			string name = (string) accDB["name"];

			for(int i = 1; i <= NetworkScript.instance.MaxPlayers; i++) {
				if(NetworkScript.instance.players[i] == null) {
					Player player = PlayerManager.instance.InstantiatePlayer();

					player.Initialize(Convert.ToInt32(accDB["id"]), connection.id, i, name, email, new Vector3(0, 0, 0));
					player.gameObject.SetActive(false);

					connection.sessionID = sessionID;
					connection.playerID = i;
					connection.isLogged = true;
					connection.deviceType = device;

					NetworkScript.instance.players[i] = player;
					ServerSend.PlayerLoginData(connection, player);
					return;
				}
			}

			ServerSend.StringMessage(connection.id, "Server is full!");
			NetworkScript.server.Disconnect(connection.id);
		} else
			ServerSend.PlayerLogout(connection, "Can't find active session. Try to login again.");
	}
}
