using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeGenerator : MonoBehaviour
{
	[SerializeField]
	Tilemap GroundTilemap = null;

	public NodeDataObject MakeDataObjectInstance()
	{
		if (GroundTilemap == null)
			return null;

		var dataObject = new NodeDataObject();
		dataObject.InitData(GroundTilemap);

		return dataObject;
	}
}
