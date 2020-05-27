using UnityEngine;
using System.Collections;
using System;

public class SceneIndentifier : MonoBehaviour {
	public int id;
	public GameLocation sceneOfLocation;
	public Vector3 spawnPoint;

	public GameObject terrain;
	public GameObject playerHolder;

	public void AssignPlayer(Player player, Vector3 spawn = default) {
		player.location = GameLocation.KingsWay;

		player.transform.position = spawn != default ? spawn : spawnPoint;
		player.transform.SetParent(playerHolder.transform);
	}
}
