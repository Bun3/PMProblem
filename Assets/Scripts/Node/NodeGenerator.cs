using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeGenerator : MonoBehaviour
{
	public NodeDataObject MakeDataObjectInstance()
	{
		var groundTilemap = GameManager.FindGroundTilemap();
		if (groundTilemap == null)
			return null;

		var dataObject = new NodeDataObject();
		dataObject.InitData(groundTilemap);

		return dataObject;
	}
}
