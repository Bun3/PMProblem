using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrazyOni : NodeBasedMoveOni, IOnMoveMapHandler
{

	protected override void UpdateTargetNodes()
	{
		if (currentNode != null && currentNode.Position != transform.position)
			return;

		targetNodes.Clear();

		var startNode = GetMyNode();
		var targetNode = GetTargetNode();

		if (startNode == null || targetNode == null)
			return;

		targetNodes.AddLast(targetNode);
		currentNode = startNode;
	}

	protected override Node GetTargetNode()
	{
		var node = GetMyNode();
		var randomDir = nodeDirs[Random.Range(0, nodeDirs.Length)];

		while (true)
		{
			var tempNode = NodeData.GetNodeByIndex(node.Index + randomDir);
			if (IsInvalidNode(tempNode))
				break;

			node = tempNode;
			if (node.OverlappedPortal != null)
				break;
		}

		return node;
	}

	bool IsInvalidNode(Node node)
	{
		return node == null || node.bIsBlock;
	}

	public override bool IsEnableChaseTarget()
	{
		return true;
	}

	public void OnMoveMap(BaseObject moveObject, Portal movedPortal, Map prevMap, Map currentMap)
	{
		if(moveObject == this)
		{
			ClearNodeDatas();
		}
	}
}
