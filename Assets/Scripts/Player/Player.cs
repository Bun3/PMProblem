using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	protected override void Awake()
	{
		base.Awake();
	}

	protected override bool IsUseMovementAnimation()
	{
		return false;
	}

	protected override Vector2 GetMoveDirection()
	{
		return Vector2.zero;
	}

	protected override bool IsWalking()
	{
		return false;
	}
}
