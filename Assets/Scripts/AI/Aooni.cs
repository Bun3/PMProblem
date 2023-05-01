using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Aooni : Oni, IOnMoveMapHandler
{
	AStarNode startNode, targetNode, curNode;
	List<AStarNode> openList, closeList;

	Stack<AStarNode> finalPaths;
	AStarNode pathNode;

	NodeDataObject Data { get => currentMap.NodeDataObject; }

	Player originTarget = null;

	static float waitSec = 0.5f;
	static object waitSecond = new WaitForSeconds(waitSec);

	protected override void Awake()
	{
		base.Awake();
		openList = new List<AStarNode>();
		closeList = new List<AStarNode>();
		finalPaths = new Stack<AStarNode>();
	}

	Coroutine chaseTargetCoroutine = null;
	IEnumerator IChaseTarget()
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
		if (target != null)
		{
			StartChase();
		}
	}

	private void OnDisable()
	{
		EndChase();
	}

	void StartChase()
	{
		EndChase();
		chaseTargetCoroutine = StartCoroutine(IChaseTarget());
	}

	void EndChase()
	{
		if (chaseTargetCoroutine != null)
		{
			StopCoroutine(chaseTargetCoroutine);
			chaseTargetCoroutine = null;
		}

		ClearNodeDatas();
	}

	private void FixedUpdate()
	{
		if (pathNode == null || pathNode.Position == transform.position)
		{
			pathNode = null;
			finalPaths.TryPop(out pathNode);
		}

		if (pathNode != null)
		{
			transform.position = Vector2.MoveTowards(transform.position, pathNode.Position, Time.fixedDeltaTime * speed);
		}
	}

	void ClearNodeDatas()
	{
		openList.Clear();
		closeList.Clear();
		finalPaths.Clear();
	}

	static readonly int[][] pathFindDirs = new int[][]
	{
		new int[2] { 1, 0 },
		new int[2] { -1, 0 },
		new int[2] { 0, 1 },
		new int[2] { 0, -1 },
		//´ë°¢¼±
		//new int[2] { 1, 1 },
		//new int[2] { 1, -1 },
		//new int[2] { -1, 1 },
		//new int[2] { -1, -1 },
	};
	bool FindPath()
	{
		ClearNodeDatas();

		if (CanFindPath() == false)
			return false;

		var tempStartNode = Data.GetNodeByWorldPos(transform.position);
		if (tempStartNode == null)
			return false;

		var tempTargetNode = Data.GetNodeByWorldPos(target.transform.position);
		if (tempTargetNode == null)
			return false;

		startNode = pathNode != null ? pathNode : new AStarNode(tempStartNode);
		targetNode = new AStarNode(tempTargetNode);

		if (Vector3.Distance(targetNode.Position, startNode.Position) == 0.0f)
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
				finalPaths.Push(curNode);
				return true;
			}

			foreach (var dir in pathFindDirs)
			{
				AddOpenList(new Vector2Int(curNode.X + dir[0], curNode.Y + dir[1]));
			}
		}

		return false;
	}

	void AddOpenList(Vector2Int index)
	{
		var node = Data.GetNodeByIndex(index);
		if (node == null)
			return;
		if (closeList.Find((astarNode) => { return node == astarNode; }) != null)
			return;

		int cost = curNode.G + (curNode.X - index.x == 0 || curNode.Y - index.y == 0 ? 10 : 14);

		bool bRefreshNode = true;
		AStarNode adjacencyNode = null;
		var openNode = openList.Find((astarNode) => { return node == astarNode; });
		if (openNode != null)
		{
			adjacencyNode = openNode;
			bRefreshNode = cost < adjacencyNode.G;
		}
		else
		{
			adjacencyNode = new AStarNode(node);
			openList.Add(adjacencyNode);
		}

		if (bRefreshNode)
		{
			adjacencyNode.G = cost;
			adjacencyNode.H = (Mathf.Abs(adjacencyNode.X - targetNode.X) + Mathf.Abs(adjacencyNode.Y - targetNode.Y));
			adjacencyNode.Parent = curNode;
		}
	}

	bool CanFindPath()
	{
		if (currentMap == null)
			return false;
		if (target == null)
			return false;
		if (currentMap != target.CurrentMap)
			return false;

		return true;
	}

	bool IsChaseable(BaseObject baseObject)
	{
		return currentMap != null && baseObject != null 
			&& (currentMap == baseObject.CurrentMap || currentMap.GetPortalMoveToMap(baseObject.CurrentMap) != null);
	}

	public void OnMoveMap(BaseObject moveObject, Portal movedPortal, Map prevMap, Map currentMap)
	{
		if (target == null && moveObject is Player && IsChaseable(moveObject))
		{
			SetTarget(moveObject);
			StartChase();
		}

		if(IsChasingTarget() && (target == moveObject || this == moveObject || originTarget == moveObject))
		{
			var tempTarget = target == moveObject ? target : originTarget;

			if (IsChaseable(tempTarget))
			{
				if (this.currentMap.GetPortalMoveToMap(currentMap) is Portal portal)
				{
					originTarget = target as Player;
					SetTarget(portal);
					StartChase();
				}
 				else if (originTarget != null)
				{
 					pathNode = null;
					SetTarget(originTarget);
					StartChase();
					originTarget = null;
				}
			}
			else
			{
				SetTarget(null);
				EndChase();
			}
		}
	}
}
