using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class TiledLevelLoader : MonoBehaviour {

	public TextAsset tiledFile;

	public GameObject[] tilePrefabs;

	void Start() {
		if (tiledFile != null) {
			loadLevel(tiledFile.text);
		}
	}

	public void loadLevel(string levelString) {

		JSONNode rootNode = JSON.Parse(levelString);
		int width = rootNode["width"].AsInt;
		int height = rootNode["height"].AsInt;

		float xOffset = -(width*PlayerMovement.TILE_SIZE)/2f;
		xOffset = Mathf.RoundToInt(xOffset / PlayerMovement.TILE_SIZE)*PlayerMovement.TILE_SIZE;
		float yOffset = -(height*PlayerMovement.TILE_SIZE)/2f;
		yOffset = Mathf.RoundToInt(yOffset / PlayerMovement.TILE_SIZE)*PlayerMovement.TILE_SIZE;

        JSONArray layers = rootNode["layers"].AsArray;
        foreach (JSONNode layer in layers) {
            JSONArray data = layer["data"].AsArray;
            int x = 0;
            int y = height - 1;
            for (int i = 0; i < data.Count; i++)
            {
                int tileIndex = data[i].AsInt;
                if (tileIndex > 0)
                {
                    GameObject spawnedObj = Instantiate(tilePrefabs[tileIndex - 1]);
                    spawnedObj.transform.parent = transform;
                    spawnedObj.transform.position = new Vector3(PlayerMovement.TILE_SIZE * x + xOffset, PlayerMovement.TILE_SIZE * y + yOffset);
                }
                x++;
                if (x >= width)
                {
                    x = 0;
                    y--;
                }
            }
        }
	}

}
