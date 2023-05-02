using Nito.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class NodeBasedMoveOni : Oni
{
	protected static readonly int[][] nodeDirs = new int[][]
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

	protected Deque<Node> targetNodes = new Deque<Node>(1000);
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
		if (MoveToTargets() || ShouldUpdateTargetNodes())
		{
			UpdateTargetNodes();
		}
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

	bool MoveToTargets()
	{
		if (currentNode == null || currentNode.Position == transform.position)
		{
			currentNode = null;
			if(targetNodes.Count == 0)
			{
				return true;
			}
			else
			{
				currentNode = targetNodes.RemoveFromBack();
			}
		}

		if (currentNode != null)
		{
			transform.position = Vector2.MoveTowards(transform.position, currentNode.Position, Time.fixedDeltaTime * speed);
		}

		return false;
	}

	//타겟을 새로 업데이트 해야만 하는 조건 정의
	protected abstract bool ShouldUpdateTargetNodes();

	protected abstract void UpdateTargetNodes();

}
