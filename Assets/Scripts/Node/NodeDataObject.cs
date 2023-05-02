using Nito.Collections;
using Priority_Queue;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

public abstract class NodeAdditionalData
{
	public abstract void ClearData();
};

//public interface INodeAdditionalData<T> where T : notnull, NodeAdditionalData
//{
//	public void ClearData();
//	public int CompareTo(T other);
//}

public sealed class AstarData : NodeAdditionalData
{
	public override void ClearData()
	{
		G = 0;
		H = 0;
		Parent = null;
	}

	//G: 시작부터 현재 노드까지 거리, H: 현재 노드부터 타겟까지의 거리(장애물 무시)
	public int G = 0, H = 0;
	public Node Parent = null;

	//F: G + H
	public int F { get => G + H; }

}

[System.Serializable]
public class Node : FastPriorityQueueNode
{
	public Node(Vector3 position, Vector2Int index, bool bIsBlock = false)
	{
		Position = position;
		Index = index;
		this.bIsBlock = bIsBlock;
	}

	public int X { get => Index.x; set => Index.x = value; }
	public int Y { get => Index.y; set => Index.y = value; }

	public Vector2Int Index;
	public Vector3 Position;
	public bool bIsBlock = false;
	public Map OwnerMap = null;
	public Portal OverlappedPortal = null;

	List<NodeAdditionalData> nodeAdditionalDatas = new List<NodeAdditionalData>();

	public void ClearData()
	{
		foreach (var data in nodeAdditionalDatas)
		{
			data.ClearData();
		}
	}

	public T GetData<T>() where T : NodeAdditionalData, new()
	{
		foreach (var data in nodeAdditionalDatas)
		{
			if(data is T parsedData)
			{
				return parsedData;
			}
		}

		var newData = new T();
		nodeAdditionalDatas.Add(newData);
		return newData;
	}

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

		var node = obj as Node;
		if(node == null)
			return false;

		return Index == node.Index && OwnerMap == node.OwnerMap;
	}

	public override string ToString()
	{
		return string.Format("Pos:[{0}], Index:[{1}], IsBlock:[{2}]", Position, Index, bIsBlock);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Index, Position, OwnerMap);
	}
}

[System.Serializable]
public class AStarNode : Node
{
	public AStarNode(Vector3 position, Vector2Int index, bool bIsBlock = false) : base(position, index, bIsBlock)
	{
	}

	public void Clear()
	{
		G = 0;
		H = 0;
	}

	//G: 시작부터 현재 노드까지 거리, H: 현재 노드부터 타겟까지의 거리(장애물 무시)
	public int G = 0, H = 0;
	public AStarNode Parent = null;

	//F: G + H
	public int F { get => G + H; }

}

public class NodeDataObject
{
	Map ownerMap = null;

	Dictionary<Vector3Int, Node> nodeDictionary;
	Node[,] nodes = null;
	List<Node> randomPointNodeList = null;
	int nodesXSize = 0, nodesYSize = 0;

	Vector3 cellSize = Vector3.zero;
	public float CellMaxSize { get => MathF.Max(cellSize.x, cellSize.y); }

	Vector3 boundMinWorldPos = Vector3.zero;
	public Vector3 BoundMinWorldPos { get => boundMinWorldPos; }
	Vector3 boundMaxWorldPos = Vector3.zero;
	public Vector3 BoundMaxWorldPos { get => boundMaxWorldPos; }

	public bool IsValidIndex(Vector2Int index) => IsValidIndex(index.x, index.y);
	public bool IsValidIndex(int x, int y)
	{
		if (nodes == null)
			return false;
		if (x < 0 || x >= nodesXSize || y < 0 || y >= nodesYSize)
			return false;

		return true;
	}
	public bool IsValidNode(Vector2Int index)
	{
		if (IsValidIndex(index) == false)
			return false;

		if (nodes[index.y, index.x] != null && nodes[index.y, index.x].bIsBlock)
			return false;

		return true;
	}

	public Node GetNodeByWorldPos(Vector2 worldPos)
	{
		var cellPos = ownerMap.GroundTilemap.WorldToCell(worldPos);
		if(nodeDictionary.TryGetValue(cellPos, out var node))
		{
			return node;
		}
		return null;
	}

	public Node GetNodeByIndex(Vector2Int index)
	{
		if (IsValidNode(index))
		{
			return nodes[index.y, index.x];
		}
		return null;
	}

	readonly static int wallLayerMask = LayerMask.NameToLayer("Wall");
	readonly static int portalLayerMask = LayerMask.NameToLayer("Portal");
	public void InitData(Map map)
	{
		if (map == null)
			return;

		ownerMap = map;
		var groundTilemap = map.GroundTilemap;

		nodesXSize = groundTilemap.cellBounds.size.x;
		nodesYSize = groundTilemap.cellBounds.size.y;

		nodes = new Node[nodesYSize, nodesXSize];
		nodeDictionary = new Dictionary<Vector3Int, Node>(nodesXSize * nodesYSize);
		randomPointNodeList = new List<Node>(nodesXSize * nodesYSize);

		cellSize = groundTilemap.layoutGrid.cellSize;
		boundMinWorldPos = groundTilemap.CellToWorld(groundTilemap.cellBounds.min);
		boundMaxWorldPos = groundTilemap.CellToWorld(groundTilemap.cellBounds.max);

		for (int y = groundTilemap.cellBounds.yMin, i = 0; y < groundTilemap.cellBounds.yMax; y++, i++)
		{
			for (int x = groundTilemap.cellBounds.xMin, j = 0; x < groundTilemap.cellBounds.xMax; x++, j++)
			{
				var cellPos = new Vector3Int(x, y, 0);
				var nodePos = groundTilemap.GetCellCenterWorld(cellPos);
				var node = nodes[i, j] = new Node(nodePos, new Vector2Int(j, i));
				node.OwnerMap = map;
				nodeDictionary.Add(cellPos, node);

				bool bAddRandomPoint = true;
				if (groundTilemap.HasTile(cellPos) == false)
				{
					node.bIsBlock = true;
					bAddRandomPoint = false;
				}

				if (map.WallTilemap.HasTile(cellPos) && Physics2D.OverlapCircle(nodePos, CellMaxSize * 0.45f) is Collider2D collider)
				{
					if (collider.gameObject.layer == wallLayerMask)
					{
						bAddRandomPoint = false;
						node.bIsBlock = true;
						Util.DrawDebugBox2D(nodePos, Vector2.one, Color.red, 10.0f);
					}

					//포탈이 포함되어 있는 셀은 랜덤 포인트에서 제외
					if(collider.gameObject.layer == portalLayerMask)
					{
						bAddRandomPoint = false;
						if(collider.gameObject.GetComponent<Portal>() is Portal portal)
						{
							node.OverlappedPortal = portal;
						}
					}
				}

				if(bAddRandomPoint)
				{
					randomPointNodeList.Add(node);
				}
			}
		}
	}

	public Vector3 GetRandomSpawnPosition()
	{
		Assert.IsTrue(randomPointNodeList.Count > 0);

		int randomIndex = UnityEngine.Random.Range(0, randomPointNodeList.Count);
		return randomPointNodeList[randomIndex];
	}

}
