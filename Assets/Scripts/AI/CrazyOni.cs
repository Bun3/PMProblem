using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyOni : NodeBasedMoveOni
{
	protected override bool IsUseMovementAnimation()
	{
		return false;
	}

	protected override Vector2 GetMoveDirection()
	{
		throw new System.NotImplementedException();
	}

	protected override bool IsWalking()
	{
		throw new System.NotImplementedException();
	}

	protected override void UpdateTargetNodes()
	{
		throw new System.NotImplementedException();
	}

	protected override Node GetTargetNode()
	{
		//To Do
		return null;
	}

	bool IsTargetNode(Node node)
	{
		return node == null || node.bIsBlock || node.OverlappedPortal != null;
	}

	protected override bool ShouldUpdateTargetNodes()
	{
		return false;
	}
}
