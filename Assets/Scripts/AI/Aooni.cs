using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Priority_Queue;
using UnityEngine.XR;

public class Aooni : NodeBasedMoveOni, IOnMoveMapHandler
{
	FastPriorityQueue<Node> openNodes;
	HashSet<Node> closeNodes;

	Player originTarget = null;

	protected override void Awake()
	{
		base.Awake();
		openNodes = new FastPriorityQueue<Node>(5000);
		closeNodes = new HashSet<Node>();
	}

	bool IsChaseable(BaseObject baseObject)
	{
		return currentMap != null && baseObject != null 
			&& (currentMap == baseObject.CurrentMap || currentMap.GetPortalMoveToMap(baseObject.CurrentMap) != null);
	}

	public void OnMoveMap(BaseObject moveObject, Portal movedPortal, Map prevMap, Map currentMap)
	{
		if (HasTarget() == false && moveObject is Player && IsChaseable(moveObject))
		{
			SetTarget(moveObject);
			ClearNodeDatas();
		}

		if(HasTarget() && (target == moveObject || this == moveObject || originTarget == moveObject))
		{
			var tempTarget = target == moveObject ? target : originTarget;

			if (IsChaseable(tempTarget))
			{
				if (this.currentMap.GetPortalMoveToMap(currentMap) is Portal portal)
				{
					originTarget = target as Player;
					SetTarget(portal);
					ClearNodeDatas();
				}
 				else if (originTarget != null)
				{
					SetTarget(originTarget);
					originTarget = null;
					ClearNodeDatas();
				}
			}
			else
			{
				SetTarget(null);
				ClearNodeDatas();
			}
		}
	}

	protected override void UpdateTargetNodes()
	{
		var startNode = NodeData.GetNodeByWorldPos(transform.position);
		if(currentNode != null)
		{
			startNode = currentNode;
		}

		if (startNode == null)
			return;

		var targetNode = GetTargetNode();
		if(targetNode == null) 
			return;

		if (targetNodes.Count > 0 && targetNodes.First.Value == targetNode)
			return;

		if (startNode == targetNode)
			return;

		if (startNode.bIsBlock || targetNode.bIsBlock)
			return;

		openNodes.Clear();
		closeNodes.Clear();
		targetNodes.Clear();

		startNode.GetData<AstarData>().ClearData();

		openNodes.Enqueue(startNode, startNode.GetData<AstarData>().F);

		while (openNodes.Count > 0)
		{
			currentNode = openNodes.Dequeue();
			closeNodes.Add(currentNode);

			if (currentNode == targetNode)
			{
				while (currentNode != startNode)
				{
					targetNodes.AddLast(currentNode);
					currentNode = currentNode.GetData<AstarData>().Parent;
				}
				targetNodes.AddLast(currentNode);
				currentNode = null;
				Util.DrawNodeLines(targetNodes, Color.magenta);
				return;
			}

			foreach (var dir in nodeDirs)
			{
				AddOpenList(currentNode.Index + dir, ref targetNode.Index);
			}
		}
	}

	void AddOpenList(Vector2Int index, ref Vector2Int targetIndex)
	{
		var node = NodeData.GetNodeByIndex(index);
		if (node == null)
			return;

		bool bConatins = closeNodes.Contains(node);
		if (bConatins)
			return;

		int cost = currentNode.GetData<AstarData>().G + (currentNode.X - index.x == 0 || currentNode.Y - index.y == 0 ? 10 : 14);
		bool bRefreshNode = true;
		Node adjacencyNode = null;
		AstarData adjacencyData = null;
		if (openNodes.Contains(node))
		{
			adjacencyNode = node;
			adjacencyData = node.GetData<AstarData>();
			bRefreshNode = cost < adjacencyData.G;
		}
		else
		{
			adjacencyNode = node;
			adjacencyData = node.GetData<AstarData>();
			openNodes.Enqueue(adjacencyNode, adjacencyData.F);
		}

		if (bRefreshNode)
		{
			adjacencyData.G = cost;
			adjacencyData.H = (Mathf.Abs(adjacencyNode.X - targetIndex.x) + Mathf.Abs(adjacencyNode.Y - targetIndex.y));
			adjacencyData.Parent = currentNode;
			openNodes.UpdatePriority(adjacencyNode, adjacencyData.F);
		}
	}

	protected override void ClearNodeDatas()
	{
		openNodes.Clear();
		closeNodes.Clear();
		base.ClearNodeDatas();
	}

	public override bool IsEnableChaseTarget()
	{
		return CanFindTarget();
	}

	bool CanFindTarget()
	{
		if (currentMap == null)
			return false;
		if (HasTarget() == false)
			return false;
		if (currentMap != target.CurrentMap)
			return false;

		return true;
	}

}
