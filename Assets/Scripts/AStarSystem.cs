using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Node
{
	public Node(int x, int y, bool bIsBlock = false) { pos.x = x; pos.y = y; this.bIsBlock = bIsBlock; }

	public Vector2Int pos;
	public bool bIsBlock = false;
	public Node parent = null;

	//G: 시작부터 현재 노드까지 거리, H: 현재 노드부터 타겟까지의 거리(장애물 무시)
	public int G = 0, H = 0;
	//F: G + H
	public int F { get { return G + H; } }

	public static implicit operator Vector2Int(Node node) => node.pos;
	public static implicit operator Vector2(Node node) => node.pos;
	public override string ToString()
	{
		return string.Format("", pos, G, H);
	}
}

[CreateAssetMenu(menuName = "Node Data")]
public class NodeData : ScriptableObject
{
	Tilemap groundTilemap;

	Node[,] nodes = null;

	void InitData(Tilemap tilemap)
	{
		if (tilemap == null)
			return;
		groundTilemap = tilemap;
		nodes = new Node[tilemap.cellBounds.size.y, tilemap.cellBounds.size.x];
	}

	void Func()
	{
		int a = 10;
	}

}

public class AStarSystem : MonoBehaviour
{


	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

	}
}
