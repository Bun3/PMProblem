using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
	public const string GroundTileTag = "GroundTile";
	public const string WallTileTag = "WallTile";
	public const string BoundTileTag = "BoundTile";

	Tilemap groundTilemap = null;
	Tilemap wallTilemap = null;
	Tilemap boundTilemap = null;

	NodeDataObject nodeDataObject = null;

	[SerializeField]
	[ReadOnly]
	List<Portal> myPortals = new List<Portal>();
	Dictionary<Map, int> mapBetweenDepths = new Dictionary<Map, int>();

	public NodeDataObject NodeDataObject { get => nodeDataObject; set => nodeDataObject = value; }
	public Tilemap GroundTilemap { get => groundTilemap; set => groundTilemap = value; }
	public Tilemap WallTilemap { get => wallTilemap; set => wallTilemap = value; }

	private void Awake()
	{
		groundTilemap = GetComponentsInChildren<Tilemap>().FirstOrDefault((comp) => comp.tag == GroundTileTag);
		wallTilemap = GetComponentsInChildren<Tilemap>().FirstOrDefault((comp) => comp.tag == WallTileTag);
		boundTilemap = GetComponentsInChildren<Tilemap>().FirstOrDefault((comp) => comp.tag == BoundTileTag);

		Assert.IsNotNull(groundTilemap);
		Assert.IsNotNull(wallTilemap);
		Assert.IsNotNull(boundTilemap);

		//바운드는 투명 벽 처리
		boundTilemap.color = Color.clear;

		nodeDataObject = new NodeDataObject();
		nodeDataObject.InitData(this);
	}

	[ContextMenu("CollectMyPortals")]
	void CollectMyPortals()
	{
		myPortals = GetComponentsInChildren<Portal>().ToList();
	}

	public Portal GetPortalMoveToMap(Map map)
	{
		if(map == null)
			return null;
		if(this == map)
			return null;

		foreach (Portal portal in myPortals)
		{
			var connectedPortal = portal.ConnectedPortal;
			if (connectedPortal != null && connectedPortal.CurrentMap == map)
				return portal;
		}

		return null;
	}

}
