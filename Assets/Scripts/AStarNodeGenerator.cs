using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarNodeGenerator : MonoBehaviour
{
	[SerializeField]
	Tilemap GroundTilemap;



	[ContextMenu("MakeAndSaveData")]
	void MakeAndSaveData()
	{
		if (GroundTilemap != null)
		{
			var bounds = GroundTilemap.cellBounds;
			var tiles = GroundTilemap.GetTilesBlock(bounds);
			int a = 10;
		}
	}
}
