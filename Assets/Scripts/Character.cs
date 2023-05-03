using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Character : BaseObject
{
	protected SpriteRenderer spriteRenderer;
	protected new Collider2D collider2D = null;

	protected Animator animator = null;

	protected Vector3 prevPosition = Vector3.zero;

	protected override void Awake()
	{
		base.Awake();

		spriteRenderer = GetComponent<SpriteRenderer>();
		collider2D = GetComponent<Collider2D>();
		animator = GetComponent<Animator>();
	}

	protected virtual void LateUpdate()
	{
		if(IsUseMovementAnimation() && animator != null)
		{
			bool bWasWalking = animator.GetBool("Walking");
			bool bIsWalking = IsWalking();
			if (bWasWalking != bIsWalking) 
			{
				animator.SetBool("Walking", bIsWalking);
			}
			
			var dir = GetMoveDirection().normalized;
			if (dir != Vector2.zero)
			{
				animator.SetFloat("DirX", dir.x);
				animator.SetFloat("DirY", dir.y);
			}
		}

		prevPosition = transform.position;
	}

	protected virtual bool IsUseMovementAnimation() { return true; }

	protected abstract bool IsWalking();
	protected abstract Vector2 GetMoveDirection();

	public void TurnLeft()
	{
		if (spriteRenderer == null)
			return;

		spriteRenderer.flipX = false;
	}

	public void TurnRight()
	{
		if (spriteRenderer == null)
			return;

		spriteRenderer.flipX = true;
	}

	public void OnSpawn(Map spawnedMap)
	{
		currentMap = spawnedMap;
		GameManager.Instance.ExecuteGameEvent<IOnSpawnObjectHandler>(null, (x, y) => x.OnSpawnObject(this));
	}

}
