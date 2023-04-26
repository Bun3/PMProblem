using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Node
{
	public Node(Vector2 pos, Vector2Int index, bool bIsBlock = false)
	{
		Pos = pos;
		Index = index;
		this.bIsBlock = bIsBlock;
	}

	public Vector2 Pos;
	public Vector2Int Index;
	public bool bIsBlock = false;

	//public static implicit operator Vector2Int(Node node) => node.Index;
	//public static implicit operator Vector2(Node node) => node.Pos;

	public override string ToString()
	{
		return string.Format("Pos:[{0}], Index:[{1}], IsBlock:[{2}]", Pos, Index, bIsBlock);
	}
}

public class NodeDataObject
{
	Node[,] nodes = null;

	public Node[,] Nodes { get => nodes; set => nodes = value; }

	public void InitData(Tilemap groundTilemap)
	{
		if (nodes != null)
			Array.Clear(nodes, 0, nodes.Length);

		nodes = new Node[groundTilemap.cellBounds.size.y, groundTilemap.cellBounds.size.x];

		var cellSize = groundTilemap.layoutGrid.cellSize;
		for (int y = groundTilemap.cellBounds.yMin, i = 0; y < groundTilemap.cellBounds.yMax; y++, i++)
		{
			for (int x = groundTilemap.cellBounds.xMin, j = 0; x < groundTilemap.cellBounds.xMax; x++, j++)
			{
				var cellPos = new Vector3Int(x, y, 0);
				var nodePos = groundTilemap.CellToWorld(cellPos);
				nodePos.x += cellSize.x * 0.5f;
				nodePos.y += cellSize.y * 0.5f;
				nodes[i, j] = new Node(nodePos, new Vector2Int(j, i));

				var size = Mathf.Max(cellSize.x, cellSize.y);
				var collider = Physics2D.OverlapCircle(nodePos, size * 0.3f);
				if (collider != null)
				{
					if (collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
					{
						nodes[i, j].bIsBlock = true;
						Util.DrawDebugBox2D(nodePos, Vector2.one, Color.red, 10.0f);
					}
				}
			}
		}
	}

}
