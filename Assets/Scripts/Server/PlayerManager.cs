using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager instance;

	public GameObject playerPrefab;

	void Awake() {
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(this);
	}

	public Player InstantiatePlayer() {
		return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
	}
}
