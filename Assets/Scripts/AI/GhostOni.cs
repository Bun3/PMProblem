using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GhostOni : Oni, IOnSpawnPlayerHandler
{
	public void OnSpawnPlayer(Player player)
	{
		SetTarget(player);
	}

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

	protected override void PerformChaseTarget(float performDeltaTime)
	{
		transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, performDeltaTime * speed);
	}

	public override bool IsEnableChaseTarget()
	{
		return target != null;
	}

}
