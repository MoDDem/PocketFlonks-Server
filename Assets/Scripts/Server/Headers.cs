public enum ServerHeaders : ushort {
	Text,

	PlayerLoginData,
	PlayerDisconnect,
	PlayerLogout,

	PlayerPosition,
	PlayerRotation,

	JoinLocationData,
	SpawnPlayerInLocation
}

public enum ClientHeaders : ushort {
	Text,

	RegisterPlayer,
	LoginPlayerByLocal,
	LoginPlayByGoogle,
	LoginPlayerBySession,

	RequestJoinGameData,
	GetPlayersInLocation,

	PlayerMovement
}