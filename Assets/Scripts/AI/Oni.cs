using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Oni : Character
{
	protected BaseObject target = null;

	public BaseObject Target {  get { return target; } }

	[SerializeField]
	protected float speed = 1.0f;

	protected override void Awake()
	{
		base.Awake();

		collider2D.isTrigger = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player"))
		{
			GameManager.Instance.OnGameOver();
		}
	}

	private void FixedUpdate()
	{
		if(IsEnableChaseTarget())
		{
			PerformChaseTarget(Time.fixedDeltaTime);
		}
	}

	public abstract bool IsEnableChaseTarget();
	public bool HasTarget()
	{
		return target != null;
	}

	public void SetTarget(BaseObject target)
	{
		if (target == null)
			return;

		this.target = target;
	}

	protected abstract void PerformChaseTarget(float performDeltaTime);

}
