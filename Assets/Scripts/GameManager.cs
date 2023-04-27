using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GameManager
{
	static Tilemap groundTilemap = null;
	static GameObject playerObject = null;
	public static Tilemap FindGroundTilemap()
	{
		const string groundTilemapTag = "GroundTile";

		if(groundTilemap == null )
		{
			var go = GameObject.FindWithTag(groundTilemapTag);
			if(go != null)
			{
				groundTilemap = go.GetComponent<Tilemap>();
			}
		}

		return groundTilemap;
	}

	public static GameObject FindPlayerObject()
	{
		const string playerTag = "Player";

		if (playerObject == null)
		{
			playerObject = GameObject.FindWithTag(playerTag);
		}

		return playerObject;
	}

}
