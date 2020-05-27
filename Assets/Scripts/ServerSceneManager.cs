using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections.Generic;

public class ServerSceneManager : MonoBehaviour {
	public static ServerSceneManager instance;

	public int additionOffset = 20;
	public Dictionary<GameLocation, SceneIndentifier> scenes = new Dictionary<GameLocation, SceneIndentifier>();

	private Vector2Int[] squareOfLands = new Vector2Int[] {
		new Vector2Int(0, 0),
		new Vector2Int(0, 1),
		new Vector2Int(1, 0),
		new Vector2Int(1, 1)
	};
	private int landsLength = 1;
	private int sceneID = 0;

	void Awake() {
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(this);

		for(int i = 1; i < SceneManager.sceneCountInBuildSettings; i++) {
			SceneManager.LoadScene(i, LoadSceneMode.Additive);
			StartCoroutine(OnLoadingEnd(i, SceneManager.GetSceneByBuildIndex(i)));
		}
	}

	private void OffsetScene(GameObject[] gameObjects) {
		SceneIndentifier indentifier = null;
		gameObjects.Single(x => x.TryGetComponent(out indentifier));

		var length = indentifier.terrain.GetComponent<Renderer>().bounds.size.x + additionOffset;
		var width = indentifier.terrain.GetComponent<Renderer>().bounds.size.y + additionOffset;

		indentifier.terrain.transform.position = new Vector3(length * squareOfLands[sceneID].x, indentifier.terrain.transform.position.y, length * squareOfLands[sceneID].y) * landsLength;
		indentifier.playerHolder.transform.position = new Vector3(length * squareOfLands[sceneID].x, indentifier.playerHolder.transform.position.y, length * squareOfLands[sceneID].y) * landsLength;

		if(sceneID != 3)
			sceneID++;
		else {
			sceneID = 0;
			landsLength++;
		}

		scenes.Add(indentifier.sceneOfLocation, indentifier);
	}

	private IEnumerator OnLoadingEnd(int id, Scene scene) {
		while(!scene.isLoaded)
			yield return null;

		OffsetScene(scene.GetRootGameObjects());
	}
}
