using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Priority_Queue;

public class Aooni : Oni, IOnMoveMapHandler
{
	AStarNode targetNode, curNode;
	FastPriorityQueue<AStarNode> openNodes;
	HashSet<AStarNode> closeNodes;
	Stack<AStarNode> finalNodes;

	NodeDataObject Data { get => currentMap.NodeDataObject; }

	Player originTarget = null;

	static float waitSec = 0.5f;
	static object waitSecond = new WaitForSeconds(waitSec);

	Animator animator = null;

	protected override void Awake()
	{
		base.Awake();
		openNodes = new FastPriorityQueue<AStarNode>(5000);
		closeNodes = new HashSet<AStarNode>();
		finalNodes = new Stack<AStarNode>();
		animator = GetComponent<Animator>();
	}

	Coroutine chaseTargetCoroutine = null;
	IEnumerator IChaseTarget()
	{
		while (true)
		{
			bool bResult = FindPath();
			if (bResult)
			{
				Util.DrawNodeLines(finalNodes.ToList(), Color.magenta, waitSec * 2);
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
		AStarNode tempNode = curNode;
		if (curNode == null || curNode.Position == transform.position)
		{
			curNode = null;
			finalNodes.TryPop(out curNode);
		}

		if(curNode != tempNode)
		{
			animator.SetBool("Walking", curNode != null);
		}

		var prevPosition = transform.position;
		if (curNode != null)
		{
			transform.position = Vector2.MoveTowards(transform.position, curNode.Position, Time.fixedDeltaTime * speed);
		}

		Vector2 dir = transform.position - prevPosition;
		dir.Normalize();

		if(dir != Vector2.zero)
		{
			animator.SetFloat("DirX", dir.x);
			animator.SetFloat("DirY", dir.y);
		}
	}

	void ClearNodeDatas()
	{
		openNodes.Clear();
		closeNodes.Clear();
		finalNodes.Clear();
		targetNode = null;
		curNode = null;
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
		if (CanFindPath() == false)
			return false;

		var startNode = curNode != null ? curNode : Data.GetNodeByWorldPos(transform.position);
		if (startNode == null)
			return false;

		var tempTargetNode = Data.GetNodeByWorldPos(target.transform.position);
		if (tempTargetNode == null)
			return false;

		//Check target node changed
		if (tempTargetNode == targetNode)
			return false;

		targetNode = tempTargetNode;

		if (startNode == targetNode)
			return false;

		openNodes.Clear();
		closeNodes.Clear();
		finalNodes.Clear();

		openNodes.Enqueue(startNode, startNode.F);

		while (openNodes.Count > 0)
		{
			curNode = openNodes.Dequeue();
			closeNodes.Add(curNode);

			if (curNode == targetNode)
			{
				while (curNode != startNode)
				{
					finalNodes.Push(curNode);
					curNode = curNode.Parent;
				}
				finalNodes.Push(curNode);
				curNode = null;
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
		bool bConatins = closeNodes.Contains(node);
		if (bConatins)
			return;

		int cost = curNode.G + (curNode.X - index.x == 0 || curNode.Y - index.y == 0 ? 10 : 14);
		bool bRefreshNode = true;
		AStarNode adjacencyNode = null;
		if (openNodes.Contains(node))
		{
			adjacencyNode = node;
			bRefreshNode = cost < adjacencyNode.G;
		}
		else
		{
			adjacencyNode = node;
			openNodes.Enqueue(adjacencyNode, adjacencyNode.F);
		}

		if (bRefreshNode)
		{
			adjacencyNode.G = cost;
			adjacencyNode.H = (Mathf.Abs(adjacencyNode.X - targetNode.X) + Mathf.Abs(adjacencyNode.Y - targetNode.Y));
			adjacencyNode.Parent = curNode;
			openNodes.UpdatePriority(adjacencyNode, adjacencyNode.F);
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
