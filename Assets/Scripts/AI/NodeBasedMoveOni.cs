using Nito.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class NodeBasedMoveOni : Oni
{
	protected static readonly Vector2Int[] nodeDirs = new Vector2Int[]
	{
		new Vector2Int(1, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1),
		//´ë°¢¼±
	};

	protected LinkedList<Node> targetNodes = new LinkedList<Node>();
	protected Node currentNode = null;

	protected NodeDataObject NodeData
	{
		get
		{
			return currentMap.NodeDataObject;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
	}

	protected override void PerformChaseTarget(float performDeltaTime)
	{
		MoveToTargets();
		UpdateTargetNodes();
	}

	protected Node GetMyNode()
	{
		return NodeData.GetNodeByWorldPos(transform.position);
	}

	protected virtual Node GetTargetNode()
	{
		if(HasTarget())
			return NodeData.GetNodeByWorldPos(target.transform.position);

		return null;
	}

	void MoveToTargets()
	{
		if (currentNode == null || currentNode.Position == transform.position)
		{
			currentNode = null;
			if(targetNodes.Count > 0)
			{
				currentNode = targetNodes.Last.Value;
				targetNodes.RemoveLast();
			}
		}

		if (currentNode != null)
		{
			transform.position = Vector2.MoveTowards(transform.position, currentNode.Position, Time.fixedDeltaTime * speed);
		}
	}

	protected override bool IsWalking()
	{
		return currentNode != null;
	}

	protected override Vector2 GetMoveDirection()
	{
		return transform.position - prevPosition;
	}

	protected abstract void UpdateTargetNodes();

	protected virtual void ClearNodeDatas()
	{
		targetNodes.Clear();
		currentNode = null;
	}

}
