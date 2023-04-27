using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Node
{
	public Node(Vector2 position, Vector2Int index, bool bIsBlock = false)
	{
		Position = position;
		Index = index;
		this.bIsBlock = bIsBlock;
	}

	public int X { get => Index.x; set => Index.x = value; }
	public int Y { get => Index.y; set => Index.y = value; }

	public Vector2Int Index;
	public Vector2 Position;
	public bool bIsBlock = false;

	public static implicit operator Vector2(Node node) => node.Position;
	public static implicit operator Vector3(Node node) => node.Position;
	public static bool operator==(Node lhs, Node rhs)
	{
		return Equals(lhs, rhs);
	}
	public static bool operator !=(Node lhs, Node rhs)
	{
		return !Equals(lhs, rhs);
	}
	public override bool Equals(object obj)
	{
		if (obj == null)
			return false;

		if (GetType() != obj.GetType())
			return false;

		Node node = (Node)obj;
		return Index == node.Index;
	}

	public override string ToString()
	{
		return string.Format("Pos:[{0}], Index:[{1}], IsBlock:[{2}]", Position, Index, bIsBlock);
	}
}

public class NodeDataObject
{
	Node[,] nodes = null;

	public Node[,] Nodes { get => nodes; set => nodes = value; }

	Vector3 cellSize = Vector3.zero;
	public float CellMaxSize { get => MathF.Max(cellSize.x, cellSize.y); }

	public bool IsValidIndex(Vector2Int index) => IsValidIndex(index.x, index.y);
	public bool IsValidIndex(int x, int y)
	{
		if (nodes == null)
			return false;
		if (x < 0 || x >= nodes.GetLength(1) || y < 0 || y >= nodes.GetLength(0))
			return false;

		return true;
	}
	public bool IsValidNode(Vector2Int index)
	{
		if (IsValidIndex(index) == false)
			return false;
		if (GetNode(index).bIsBlock)
			return false;

		return true;
	}

	Vector2Int PositionToIndex(Vector2 worldPos)
	{
		int x = (int)worldPos.x;
		int y = (int)worldPos.y;
		float decimalX = worldPos.x - x;
		float decimalY = worldPos.y - y;

		if (Mathf.Abs(decimalX) >= (cellSize.x * 0.5f))
			x++;
		if (Mathf.Abs(decimalY) >= (cellSize.y * 0.5f))
			y++;

		return new Vector2Int(x, y);
	}

	public Node GetNode(Vector2 worldPos)
	{
		return GetNode(PositionToIndex(worldPos));
	}

	public Node GetNode(Vector2Int index)
	{
		if (IsValidIndex(index))
		{
			return nodes[index.y, index.x];
		}
		return null;
	}

	public void InitData(Tilemap groundTilemap)
	{
		if (nodes != null)
			Array.Clear(nodes, 0, nodes.Length);

		nodes = new Node[groundTilemap.cellBounds.size.y, groundTilemap.cellBounds.size.x];

		cellSize = groundTilemap.layoutGrid.cellSize;

		for (int y = groundTilemap.cellBounds.yMin, i = 0; y < groundTilemap.cellBounds.yMax; y++, i++)
		{
			for (int x = groundTilemap.cellBounds.xMin, j = 0; x < groundTilemap.cellBounds.xMax; x++, j++)
			{
				var cellPos = new Vector3Int(x, y, 0);
				var nodePos = groundTilemap.CellToWorld(cellPos);
				nodePos.x += cellSize.x * 0.5f;
				nodePos.y += cellSize.y * 0.5f;
				nodes[i, j] = new Node(nodePos, new Vector2Int(j, i));

				var collider = Physics2D.OverlapCircle(nodePos, CellMaxSize * 0.3f);
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
