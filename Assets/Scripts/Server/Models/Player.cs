using System;
using UnityEngine;

[Serializable]
public class Player : MonoBehaviour {
	public int dbID;

	public int id;
	public int connectionID;
	public string username;
	public string email;
	public Vector2 moveMagnitude;
	public GameLocation location;

	private float moveSpeed;

	public void Initialize(int dbID, int connectionID, int id, string username, string email, Vector3 spawnPoint) {
		this.connectionID = connectionID;
		this.id = id;
		this.username = username;
		this.email = email;
		this.dbID = dbID;

		moveMagnitude = Vector2.zero;
		transform.position = spawnPoint;
		moveSpeed = 5f * Time.fixedDeltaTime;
	}

	public void SetValues(params ValueTuple<string, object>[] prop) {
		Type sourceType = GetType();

		foreach(var p in prop) {
			var targetObj = sourceType.GetField(p.Item1);
			if(targetObj == null)
				throw new Exception("Thats bad");

			targetObj.SetValue(this, p.Item2);
		}
	}

	void FixedUpdate() {
		if(NetworkScript.instance.connections[connectionID].status == UserStatus.InGame)
			Move();
	}

	private void Move() {
		Vector3 moveDir = transform.right * moveMagnitude.x + transform.forward * moveMagnitude.y;
		transform.position += moveDir * moveSpeed;

		ServerSend.PlayerPosition(this);
	}
}
