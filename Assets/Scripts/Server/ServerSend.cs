using UnityEngine;
using System.Collections;
using System;

public class ServerSend {
	public static void StringMessage(int toID, string msg) {
		PacketWriter packet;
		using(packet = new PacketWriter()) {
			packet.Write((ushort) ServerHeaders.Text);
			packet.Write(msg);
		}

		NetworkScript.server.Send(toID, packet.GetBytes());
	}

	public static void UserDisconnected(UserConnection con, string reason = null) {
		PacketWriter packet;
		using(packet = new PacketWriter()) {
			packet.Write((ushort) ServerHeaders.PlayerDisconnect);
			packet.Write(con.playerID);
		}
		
		foreach(var connection in NetworkScript.instance.connections.Values) {
			if(connection.status == UserStatus.InMenu)
				continue;

			NetworkScript.server.Send(connection.id, packet.GetBytes());
		}
	}

	public static void PlayerPosition(Player player) {
		PacketWriter packet;
		using(packet = new PacketWriter()) {
			packet.Write((ushort) ServerHeaders.PlayerPosition);
			packet.Write(player.id);
			packet.Write(player.transform.position);
		}

		foreach(var connection in NetworkScript.instance.connections.Values) {
			if(connection.status == UserStatus.InMenu)
				continue;

			NetworkScript.server.Send(connection.id, packet.GetBytes());
		}
	}

	public static void PlayerLoginData(UserConnection connection, Player player = null) {
		PacketWriter packet;
		using(packet = new PacketWriter()) {
			packet.Write((ushort) ServerHeaders.PlayerLoginData);
			packet.Write(connection.sessionID);
			packet.Write(player.id);
			packet.Write(player.email);
			packet.Write(player.username);
		}

		NetworkScript.server.Send(connection.id, packet.GetBytes());
	}

	public static void JoinLocationData(UserConnection connection, Player connectedPlayer) {
		PacketWriter packet;
		using(packet = new PacketWriter()) {
			packet.Write((ushort) ServerHeaders.JoinLocationData);
			packet.Write((int) connectedPlayer.location);
			packet.Write(connectedPlayer.transform.position);
		}

		NetworkScript.server.Send(connection.id, packet.GetBytes());
	}

	public static void PlayerLogout(UserConnection connection, string reason) {
		PacketWriter packet;
		using(packet = new PacketWriter()) {
			packet.Write((ushort) ServerHeaders.PlayerLogout);
			packet.Write(reason);
		}

		NetworkScript.server.Send(connection.id, packet.GetBytes());
	}

	public static void SpawnPlayer(UserConnection connection, Player player) {
		PacketWriter packet;
		using(packet = new PacketWriter()) {
			packet.Write((ushort) ServerHeaders.SpawnPlayerInLocation);
			packet.Write(player.id);
			packet.Write(player.username);
			packet.Write(player.transform.position);
		}

		NetworkScript.server.Send(connection.id, packet.GetBytes());
	}
}
