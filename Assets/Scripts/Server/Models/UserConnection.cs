using System;
using UnityEngine;

[Serializable]
public class UserConnection : ScriptableObject {
	public DeviceType deviceType;
	public UserStatus status;
	public int id;
	public int playerID;
	public string sessionID;
	public bool isLogged;
}
