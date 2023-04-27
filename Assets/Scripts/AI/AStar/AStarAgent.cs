using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AStarNode : Node, IComparable<AStarNode>
{
	public AStarNode(Vector2 position) : base(position, new Vector2Int(-1, -1))
	{
	}

	public AStarNode(Node node) : base(node.Position, node.Index, node.bIsBlock)
	{
	}

	public void Clear()
	{
		G = 0;
		H = 0;
	}

	public int CompareTo(AStarNode other)
	{
		return F.CompareTo(other.F); 
	}

	//G: 시작부터 현재 노드까지 거리, H: 현재 노드부터 타겟까지의 거리(장애물 무시)
	public int G = 0, H = 0;
	public AStarNode Parent = null;

	//F: G + H
	public int F { get => G + H; }

}

public class AStarAgent : MonoBehaviour
{
	AStarNode startNode, targetNode, curNode;
	List<AStarNode> openList, closeList;
	Stack<AStarNode> finalPaths;

	AStarSystem systemInstace = null;
	GameObject target = null;

	float agentSpeed = 4;

	static float waitSec = 0.8f;
	static object waitSecond = new WaitForSeconds(waitSec);

	void Awake()
	{
		openList = new List<AStarNode>();
		closeList = new List<AStarNode>();
		finalPaths = new Stack<AStarNode>();

		AStarSystem.AddInitSystemCallback(OnInitSystem);
	}

	void OnInitSystem(AStarSystem system)
	{
		systemInstace = system;
		target = system.Player;
	}

	Coroutine followPlayer = null;
	IEnumerator IFollowPlayer()
	{
		while (true)
		{
			if (FindPath())
			{
				Util.DrawNodeLines(finalPaths.ToList(), Color.magenta, waitSec * 2);
			}
			yield return waitSecond;
		}
	}

	private void OnEnable()
	{
		followPlayer = StartCoroutine(IFollowPlayer());
	}

	private void OnDisable()
	{
		StopCoroutine(followPlayer);
	}

	private void FixedUpdate()
	{
		if(finalPaths.TryPeek(out var node))
		{
			transform.position = Vector2.MoveTowards(transform.position, node.Position, Time.fixedDeltaTime * agentSpeed);
			if((Vector2)transform.position == node.Position)
			{
				finalPaths.Pop();
			}
		}
	}

	int[][] pathFindDirs = new int[][]
	{
		new int[2] { 1, 0 },
		new int[2] { -1, 0 },
		new int[2] { 0, 1 },
		new int[2] { 0, -1 },
		//대각선
		//new int[2] { 1, 1 },
		//new int[2] { 1, -1 },
		//new int[2] { -1, 1 },
		//new int[2] { -1, -1 },
	};
	bool FindPath()
	{
		openList.Clear();
		closeList.Clear();
		finalPaths.Clear();

		if (IsValidAgent() == false)
			return false;

		startNode = new AStarNode(systemInstace.DataObject.GetNode(transform.position));
		targetNode = new AStarNode(systemInstace.DataObject.GetNode(target.transform.position));

		if (Vector2.Distance(targetNode.Position, startNode.Position) == 0.0f)
			return false;

		openList.Add(startNode);

		while (openList.Count > 0)
		{
			openList.Sort();
			curNode = openList[0];
			openList.RemoveAt(0);
			closeList.Add(curNode);

			if (curNode == targetNode)
			{
				//finalPaths.Push(new AStarNode(target.transform.position));
				//curNode = curNode.Parent;
				while (curNode != startNode)
				{
					finalPaths.Push(curNode);
					curNode = curNode.Parent;
				}
				return true;
			}

			foreach (var dir in pathFindDirs)
			{
				AddOpenList(new Vector2Int(curNode.X + dir[0], curNode.Y + dir[1]));
			}
		}

		return false;
	}

	public void AddOpenList(Vector2Int index)
	{
		if (systemInstace.DataObject.IsValidNode(index) == false)
			return;

		var node = systemInstace.DataObject.GetNode(index);
		if (node == null)
			return;
		if (closeList.Find((astarNode) => { return node.Index == astarNode.Index; }) != null)
			return;

		int cost = curNode.G + (curNode.X - index.x == 0 || curNode.Y - index.y == 0 ? 10 : 14);

		bool bRefreshNode = true;
		AStarNode adjacentNode = null;
		var openNode = openList.Find((astarNode) => { return node.Index == astarNode.Index; });
		if (openNode != null)
		{
			adjacentNode = openNode;
			bRefreshNode = cost < adjacentNode.G;
		}
		else
		{
			adjacentNode = new AStarNode(node);
			openList.Add(adjacentNode);
		}

		if (bRefreshNode)
		{
			adjacentNode.G = cost;
			adjacentNode.H = (Mathf.Abs(adjacentNode.X - targetNode.X) + Mathf.Abs(adjacentNode.Y - targetNode.Y));
			adjacentNode.Parent = curNode;
		}
	}

	bool IsValidAgent()
	{
		if (systemInstace == null)
			return false;
		if (target == null)
			return false;

		return true;
	}

}
